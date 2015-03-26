using System;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.MasterOrMedia
{
    /// <summary>
    /// The EXT-X-INDEPENDENT-SEGMENTS tag indicates that all media samples
    /// in a Media Segment can be decoded without information from other
    /// segments. It applies to every Media Segment in the Playlist.
    ///
    /// Its format is:
    /// 
    /// #EXT-X-INDEPENDENT-SEGMENTS
    /// 
    /// If the EXT-X-INDEPENDENT-SEGMENTS tag appears in a Master Playlist,
    /// it applies to every Media Segment in every Media Playlist in the
    /// Master Playlist.
    /// </summary>
    public class IndependentSegments : BaseTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndependentSegments"/> class.
        /// </summary>
        internal IndependentSegments()
        {
            // since this has no attributes, lets default UsingdefaultCtor as false
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-INDEPENDENT-SEGMENTS"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXIndependentSegments; }
        }

        /// <summary>
        /// Gets a value indicating whether this tag has attributes.
        /// </summary>
        public override bool HasAttributes
        {
            get { return false; }
        }

        /// <summary>
        /// Validates that the <paramref name="content"/> is empty or <b>null</b>.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public override void Deserialize(string content, int version)
        {
            if (!string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("The EXT-X-INDEPENDENT-SEGMENTS tag must not have attributes.");
            }
        }
    }
}
