using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents the media playlist.
    /// </summary>
    public sealed class MediaPlaylist : PlaylistBase
    {

        private readonly List<MediaSegment> _mediaSegments = new List<MediaSegment>(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlaylist"/> class.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        public MediaPlaylist(IEnumerable<PlaylistLine> playlist)
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
        /// Deserializes a <see cref="MediaPlaylist"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        private void Parse(IEnumerable<PlaylistLine> content)
        {
            try
            {
                // Algorithm:
                // 1. Parse tags that are not media segment tags as usual
                // 2. If the tag is a media segment tag:
                // 2.1 Create media segment
                // 2.2 Feed media segment as long as it takes tags
                // 2.3 When media segment rejects to parse tag, add media segment to the list of media segments and create a new media segment and go to 2.1
                // 3. Add media segment to the list of media segments if it was created and has child tags

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
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("Failed to deserialize {0} class.", typeof(MediaPlaylist).Name), ex);
            }
        }

        private void ProcessMediaSegment(PlaylistLine line, ref MediaSegment mediaSegment)
        {
            if (mediaSegment == null)
            {
                mediaSegment = new MediaSegment();
                if (!mediaSegment.ReadTag(line, Version))
                {
                    throw new InvalidOperationException("Media segment did not accept tag even though the segment was empty.");
                }
                _mediaSegments.Add(mediaSegment);
            }
            else if (!mediaSegment.ReadTag(line, Version))
            {
                mediaSegment = null;
                ProcessMediaSegment(line, ref mediaSegment);
            }
        }
    }
}
