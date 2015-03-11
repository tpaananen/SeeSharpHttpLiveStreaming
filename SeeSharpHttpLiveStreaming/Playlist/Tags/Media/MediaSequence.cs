using System;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media
{
    /// <summary>
    /// Represents the EXT-X-MEDIA-SEQUENCE tag.
    /// </summary>
    /// <remarks>
    /// The EXT-X-MEDIA-SEQUENCE tag indicates the Media Sequence Number of
    /// the first Media Segment that appears in a Playlist file. Its format is:
    /// 
    /// #EXT-X-MEDIA-SEQUENCE:&lt;number&gt;
    /// 
    /// where number is a decimal-integer.
    ///
    /// If the Media Playlist file does not contain an EXT-X-MEDIA-SEQUENCE
    /// tag then the Media Sequence Number of the first Media Segment in the
    /// playlist SHALL be considered to be 0. A client MUST NOT assume that
    /// segments with the same Media Sequence Number in different Media
    /// Playlists contain matching content - see Section 6.3.2.
    ///
    /// A URI for a Media Segment is not required to contain its Media
    /// Sequence Number.
    ///
    /// See Section 6.2.1 and Section 6.3.5 for more information on setting
    /// the EXT-X-MEDIA-SEQUENCE tag.
    /// </remarks>
    public class MediaSequence : BaseTag
    {
        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-MEDIA-SEQUENCE"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXMediaSequence; }
        }

        /// <summary>
        /// Gets the number.
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
                throw new SerializationException("Failed to parse EXT-X-MEDIA-SEQUENCE tag.", ex);
            }
        }
    }
}
