using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment
{
    /// <summary>
    /// Represents the EXT-X-DISCONTINUITY tag.
    /// </summary>
    /// <remarks>
    /// The EXT-X-DISCONTINUITY tag indicates a discontinuity between the
    /// Media Segment that follows it and the one that preceded it.
    ///
    /// Its format is:
    ///
    /// #EXT-X-DISCONTINUITY
    ///
    /// The EXT-X-DISCONTINUITY tag MUST be present if there is a change in
    /// any of the following characteristics:
    ///
    /// o  file format
    /// o  number, type and identifiers of tracks
    /// o  timestamp sequence
    ///
    /// The EXT-X-DISCONTINUITY tag SHOULD be present if there is a change in
    /// any of the following characteristics:
    /// 
    /// o  encoding parameters
    /// o  encoding sequence
    /// 
    /// See Section 3, Section 6.2.1, and Section 6.3.3 for more information
    /// about the EXT-X-DISCONTINUITY tag.
    /// </remarks>
    public class Discontinuity : BaseTag
    {
        /// <summary>
        /// Gets the name of the tag, for example EXT-X-MEDIA.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-DISCONTINUITY"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXDiscontinuity; }
        }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>..
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public override void Deserialize(string content, int version)
        {
            if (!string.IsNullOrEmpty(content))
            {
                throw new SerializationException("The EXT-X-DISCONTINUITY tag must not have any attributes.");
            }
        }
    }
}
