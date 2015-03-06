using System;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment
{
    /// <summary>
    /// Represents the EXT-X-BYTERANGE tag.
    /// </summary>
    /// <remarks>
    /// The EXT-X-BYTERANGE tag indicates that a Media Segment is a sub-range
    /// of the resource identified by its URI.  It applies only to the next
    /// URI line that follows it in the Playlist.  Its format is:
    ///
    /// #EXT-X-BYTERANGE:|n|[@|o|]
    ///
    /// where n is a decimal-integer indicating the length of the sub-range
    /// in bytes. If present, o is a decimal-integer indicating the start of
    /// the sub-range, as a byte offset from the beginning of the resource.
    /// If o is not present, the sub-range begins at the next byte following
    /// the sub-range of the previous Media Segment.
    ///
    /// If o is not present, a previous Media Segment MUST appear in the
    /// Playlist file and MUST be a sub-range of the same media resource, or
    /// the Media Segment is undefined and parsing the Playlist MUST fail.
    ///
    /// A Media Segment without an EXT-X-BYTERANGE tag consists of the entire
    /// resource identified by its URI.
    ///
    /// Use of the EXT-X-BYTERANGE tag REQUIRES a compatibility version
    /// number of 4 or greater.
    /// </remarks>
    public class ByteRange : BaseTag
    {
        /// <summary>
        /// Gets the name of the tag, for example EXT-X-MEDIA.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-BYTERANGE"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXByteRange; }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        /// Gets the start index. This is optional and defaults to zero.
        /// </summary>
        public long StartIndex { get; private set; }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public override void Deserialize(string content, int version)
        {
            if (version < 4)
            {
                throw new IncompatibleVersionException(this, version, 4);
            }

            content.RequireNotEmpty("content");
            try
            {
                var split = content.Split('@');
                if (split.Length == 0 || split.Length > 2)
                {
                    throw new FormatException("Invalid format in EXT-X-BYTERANGE value.");
                }
                Length = ValueParser.ParseInt(split[0]);
                if (split.Length == 2)
                {
                    StartIndex = ValueParser.ParseInt(split[1]);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse EXT-X-BYTERANGE tag.", ex);
            }
        }
    }
}
