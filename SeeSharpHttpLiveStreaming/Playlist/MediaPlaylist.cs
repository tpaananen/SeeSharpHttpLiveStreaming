using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents the media playlist.
    /// </summary>
    internal sealed class MediaPlaylist : PlaylistBase
    {

        private readonly List<MediaSegment> _mediaSegments = new List<MediaSegment>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlaylist" /> class.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <param name="baseUri">The base URI.</param>
        public MediaPlaylist(IEnumerable<PlaylistLine> playlist, Uri baseUri)
            : base(baseUri)
        {
            Parse(playlist);
        }

        /// <summary>
        /// Gets the media segments.
        /// </summary>
        public IReadOnlyCollection<MediaSegment> MediaSegments
        {
            get { return new ReadOnlyCollection<MediaSegment>(_mediaSegments); }
        }

        /// <summary>
        /// Gets the type of the playlist. Defaults to <see cref="MediaPlaylistType.None"/>.
        /// </summary>
        public MediaPlaylistType PlaylistType { get; internal set; }

        /// <summary>
        /// Gets the sequence number.
        /// </summary>
        public long SequenceNumber { get; internal set; }

        /// <summary>
        /// Gets the max duration of each segment in the playlist.
        /// </summary>
        public long TargetDuration { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the EXT-X-I-FRAMES-ONLY tag is present.
        /// This indicates that each media segment must have <see cref="ByteRange"/>
        /// tag present.
        /// </summary>
        public bool IntraFramesOnly { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this is the final playlist / segment.
        /// When the playlist contains the final segment of the presentation, there 
        /// must be a #EXT-X-ENDLIST tag.
        /// </summary>
        public bool IsFinal { get; internal set; }

        /// <summary>
        /// Gets or sets the discontinuity sequence.
        /// </summary>
        public long DiscontinuitySequence { get; internal set; }

        /// <summary>
        /// Deserializes a <see cref="MediaPlaylist"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        private void Parse(IEnumerable<PlaylistLine> content)
        {
            MediaSegment mediaSegment = null;
            foreach (var line in content)
            {
                if (!Tag.IsMasterTag(line.Tag))
                {
                    if (Tag.IsMediaSegmentTag(line.Tag))
                    {
                        ProcessMediaSegment(line, ref mediaSegment);
                    }
                    else
                    {
                        ProcessSingleLine(line);
                    }
                }
                else
                {
                    throw new SerializationException("The tag " + line.Tag + " is a master playlist tag. Media playlist tag must not contain master playlist tags.");
                }
            }

            ValidateMediaSegments();
        }

        private void ValidateMediaSegments()
        {
            if (IntraFramesOnly && _mediaSegments.Any(d => d.ByteRange == null))
            {
                throw new SerializationException("The EXT-X-I-FRAMES-ONLY tag is present but byte range tag is missing from the media segment.");
            }
        }

        private void ProcessMediaSegment(PlaylistLine line, ref MediaSegment mediaSegment)
        {
            if (mediaSegment == null)
            {
                mediaSegment = new MediaSegment();
            }

            if (!mediaSegment.ReadTag(line, BaseUri, Version))
            {
                _mediaSegments.Add(mediaSegment);
                mediaSegment = null;
            }
        }

        /// <summary>
        /// Processes the playlist line.
        /// </summary>
        /// <param name="line">The playlist line.</param>
        protected override BaseTag ProcessSingleLine(PlaylistLine line)
        {
            var tag = base.ProcessSingleLine(line);
            var mediaTag = tag as MediaBaseTag;
            if (mediaTag != null)
            {
                mediaTag.AddToPlaylist(this);
            }

            return tag;
        }
    }
}
