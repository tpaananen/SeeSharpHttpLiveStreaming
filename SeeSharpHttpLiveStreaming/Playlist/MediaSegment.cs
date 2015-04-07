using System;
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
    internal class MediaSegment
    {

        // TODO: create constructor that takes key and map as optional parameters when there is test data available

        /// <summary>
        /// Gets the duration of the segment.
        /// </summary>
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
        /// Gets the URI.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets the program date time.
        /// </summary>
        public ProgramDateTime ProgramDateTime { get; private set; }

        // TBD: map, key

        /// <summary>
        /// Reads the tag and either accepts or rejects it.
        /// If the segment rejects the tag, parser should create a new segment
        /// which will accept the tag.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="version">The version.</param>
        /// <returns>
        /// When this method returns <b>false</b> it indicates that the
        /// segment is ready and a new segment should be created for
        /// next lines.
        /// </returns>
        public bool ReadTag(PlaylistLine line, Uri baseUri, int version)
        {
            // Accept tags as long there is a URI on the line
            CreateTag(line, baseUri, version);
            return line.Uri == null;
        }

        private void CreateTag(PlaylistLine line, Uri baseUri, int version)
        {
            var tag = TagFactory.Create(line, baseUri, version);
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
            else if (tag.TagType == TagType.ExtXProgramDateTime)
            {
                ProgramDateTime = tag as ProgramDateTime;
            }
            // TBD: key, map
        }
    }
}
