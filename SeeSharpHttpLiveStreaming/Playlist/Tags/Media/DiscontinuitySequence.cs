using System;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media
{

    /// <summary>
    /// Represents the EXT-X-DISCONTINUITY-SEQUENCE tag.
    /// </summary>
    /// <remarks>
    /// The EXT-X-DISCONTINUITY-SEQUENCE tag allows synchronization between
    /// different Renditions of the same Variant Stream or different Variant
    /// Streams that have EXT-X-DISCONTINUITY tags in their Media Playlists.
    ///
    /// Its format is:
    ///
    /// #EXT-X-DISCONTINUITY-SEQUENCE:&lt;number&gt;
    /// 
    /// If the Media Playlist does not contain an EXT-X-DISCONTINUITY-
    /// SEQUENCE tag, then the Discontinuity Sequence Number of the first
    /// Media Segment in the Playlist SHALL be considered to be 0.
    ///
    /// The EXT-X-DISCONTINUITY-SEQUENCE tag MUST appear before the first
    /// Media Segment in the Playlist.
    ///
    /// The EXT-X-DISCONTINUITY-SEQUENCE tag MUST appear before any EXT-
    /// X-DISCONTINUITY tag.
    ///
    /// See Section 6.2.1 and Section 6.2.2 for more information about
    /// setting the value of the EXT-X-DISCONTINUITY-SEQUENCE tag.
    /// </remarks>
    public class DiscontinuitySequence : BaseTag
    {
        /// <summary>
        /// Gets the name of the tag, for example EXT-X-MEDIA.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-DISCONTINUITY-SEQUENCE"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXDiscontinuitySequence; }
        }

        /// <summary>
        /// Gets the Discontinuity Sequence Number.
        /// </summary>
        public long Number { get; private set; }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public override void Deserialize(string content, int version)
        {
            content.RequireNotEmpty("content");
            try
            {
                Number = ValueParser.ParseInt(content);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse " + TagName.Substring(1) + " tag.", ex);
            }
        }
    }
}
