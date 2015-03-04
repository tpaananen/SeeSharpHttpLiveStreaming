using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace SeeSharpLiveStreaming.Playlist.Tags.Media.MediaSegment
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
        private readonly List<BaseTag> _tags = new List<BaseTag>();

        /// <summary>
        /// Gets the URI of the segment.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets the optional encryption elements.
        /// </summary>
        public IEncryption Encryption { get; private set; }

        /// <summary>
        /// Gets the tags of the segment.
        /// </summary>
        public IReadOnlyCollection<BaseTag> Tags
        {
            get
            {
                return new ReadOnlyCollection<BaseTag>(_tags);
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
            // 1. revision, accept all tags that are not the same to the same segment
            // 2. revision, refactor when tests are passing

            if (_tags.Count == 0)
            {
                _tags.Add(BaseTag.Create(line, version));
                return true;
            }

            if (_tags.Any(x => x.TagName == line.Tag))
            {
                return false;
            }

            _tags.Add(BaseTag.Create(line, version));
            return true;
        }
    }
}
