using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents a media segment.
    /// </summary>
    /// <remarks>
    /// Each Media Segment is specified by a series of Media Segment tags
    /// followed by a URI. Some Media Segment tags apply to just the next
    /// segment; others apply to all subsequent segments until another
    /// instance of the same tag.
    /// 
    /// A Media Segment tag MUST NOT appear in a Master Playlist. Clients
    /// SHOULD fail to parse Playlists that contain both Media Segment Tags
    /// and Master Playlist tags (Section 4.3.4).
    /// </remarks>
    public class MediaSegment
    {
        private const string Discontinuity = "#EXT-X-DISCONTINUITY";

        private readonly List<string> _tags = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSegment" /> class.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="version">The version.</param>
        internal MediaSegment(PlaylistLine line, int version)
        {
            if (line.Tag != Discontinuity)
            {
                CreateTag(line, version);
            }
            else
            {
                CreatedByDiscontinuity = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the segment was created by discontinuity tag.
        /// </summary>
        public bool CreatedByDiscontinuity { get; private set; }

        /// <summary>
        /// Gets the duration of the segment.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public decimal Duration { get; private set; }

        /// <summary>
        /// Gets the information.
        /// </summary>
        public string Information { get; private set; }

        /// <summary>
        /// Gets the byte range.
        /// </summary>
        public ByteRange ByteRange { get; private set; }

        /// <summary>
        /// Gets the URI. This parsed from the line holding EXT-X-INF tag.
        /// </summary>
        public Uri Uri { get; private set; }

        // TBD: map, key and program date

        /// <summary>
        /// Gets the tags of the segment.
        /// </summary>
        public IReadOnlyCollection<string> Tags
        {
            get
            {
                return new ReadOnlyCollection<string>(_tags);
            }
        }

        /// <summary>
        /// Reads the tag and either accepts or rejects it.
        /// If the segment rejects the tag, parser should create a new segment
        /// which will accept the tag.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="version">The version.</param>
        /// <returns>
        ///   <b>True</b> is the tag was accepted; otherwise, <b>false</b>.
        /// </returns>
        public bool ReadTag(PlaylistLine line, int version)
        {
            /* TODO: Current parsing implementetation does not satisfy statement below:
              
             * Each Media Segment is specified by a series of Media Segment tags
               followed by a URI. Some Media Segment tags apply to just the next
               segment; 
               others apply to all subsequent segments until another
               instance of the same tag.
             
               - Key?
               - Map?
               - ProgramDateTime?
             */

            if (line.Tag == Discontinuity || _tags.Contains(line.Tag))
            {
                return false;
            }

            CreateTag(line, version);
            return true;
        }

        private void CreateTag(PlaylistLine line, int version)
        {
            var tag = TagFactory.Create(line, version);
            if (line.Uri != null)
            {
                Uri = line.Uri;
            }

            if (tag.TagType == TagType.ExtXByteRange)
            {
                ByteRange = (ByteRange) tag;
            }
            else if (tag.TagType == TagType.ExtInf)
            {
                var inf = (ExtInf) tag;
                Information = inf.Information;
                Duration = inf.Duration;
            }
            // TBD: program date time, key, map
            _tags.Add(tag.TagName);
        }
    }
}
