using System;
using System.Globalization;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment
{
    /// <summary>
    /// Represents the EXT-X-BYTERANGE tag.
    /// </summary>
    /// <remarks>
    /// The EXT-X-BYTERANGE tag indicates that a Media Segment is a sub-range
    /// of the resource identified by its URI. It applies only to the next
    /// URI line that follows it in the Playlist. Its format is:
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
    internal class ByteRange : BaseTag, IEquatable<ByteRange>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteRange"/> class.
        /// </summary>
        internal ByteRange()
        {
            UsingDefaultCtor = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteRange"/> class.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="startIndex">The start index.</param>
        public ByteRange(long length, long startIndex = 0)
        {
            ValidateLength(length);
            ValidateStartIndex(startIndex);
            Length = length;
            StartIndex = startIndex;
        }

        /// <summary>
        /// Gets the name of the tag.
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
        /// Gets the default byte range of 0@0.
        /// </summary>
        public static ByteRange Default
        {
            get
            {
                return new ByteRange();
            }
        }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>..
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public override void Deserialize(string content, int version)
        {
            if (version < 4)
            {
                throw new IncompatibleVersionException(TagName, version, 4);
            }

            content.RequireNotEmpty("content");
            try
            {
                var split = content.Split('@');
                if (split.Length > 2)
                {
                    throw new FormatException("Invalid format in EXT-X-BYTERANGE value.");
                }
                var length = ValueParser.ParseInt(split[0]);
                ValidateLength(length);
                Length = length;

                if (split.Length == 2)
                {
                    var startIndex = ValueParser.ParseInt(split[1]);
                    ValidateStartIndex(startIndex);
                    StartIndex = startIndex;
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse EXT-X-BYTERANGE tag.", ex);
            }
        }

        /// <summary>
        /// Serializes the attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void SerializeAttributes(IPlaylistWriter writer)
        {
            InternalSerializeAttributes(writer);
        }

        /// <summary>
        /// Serializes the attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void InternalSerializeAttributes(IPlaylistWriter writer)
        {
            writer.Write(Length.ToString(CultureInfo.InvariantCulture));
            if (StartIndex > 0)
            {
                writer.Write("@");
                writer.Write(StartIndex.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ByteRange other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Length == other.Length && StartIndex == other.StartIndex;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((ByteRange) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Length.GetHashCode() * 397) ^ StartIndex.GetHashCode();
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        public static bool operator ==(ByteRange left, ByteRange right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        public static bool operator !=(ByteRange left, ByteRange right)
        {
            return !Equals(left, right);
        }

        private static void ValidateStartIndex(long startIndex)
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", startIndex, "The start index cannot be negative.");
            }
        }

        private static void ValidateLength(long length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "The length cannot be zero or negative value.");
            }
        }
    }
}
