using System;
using System.Globalization;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment
{
    /// <summary>
    /// Represents a EXT-X-PROGRAM-DATE-TIME tag. 
    /// </summary>
    /// <remarks>
    /// The EXT-X-PROGRAM-DATE-TIME tag associates the first sample of a
    /// Media Segment with an absolute date and/or time.  It applies only to
    /// the next Media Segment.
    ///
    /// The date/time representation is ISO/IEC 8601:2004 [ISO_8601] and
    /// SHOULD indicate a time zone and fractional parts of seconds:
    ///
    /// #EXT-X-PROGRAM-DATE-TIME:&lt;YYYY-MM-DDThh:mm:ssZ&gt;
    /// 
    /// For example:
    /// 
    /// #EXT-X-PROGRAM-DATE-TIME:2010-02-19T14:54:23.031+08:00
    /// 
    /// EXT-X-PROGRAM-DATE-TIME tags SHOULD provide millisecond accuracy.
    /// </remarks>
    public class ProgramDateTime : BaseTag
    {
        /// <summary>
        /// Gets the name of the tag, for example EXT-X-MEDIA.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-PROGRAM-DATE-TIME"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXProgramDateTime; }
        }

        /// <summary>
        /// Gets the program date time.
        /// </summary>
        /// <remarks>
        /// The date/time representation is ISO/IEC 8601:2004 [ISO_8601] and
        /// SHOULD indicate a time zone and fractional parts of seconds:
        ///
        /// #EXT-X-PROGRAM-DATE-TIME:&lt;YYYY-MM-DDThh:mm:ssZ&gt;
        /// 
        /// For example:
        /// 
        /// #EXT-X-PROGRAM-DATE-TIME:2010-02-19T14:54:23.031+08:00
        /// 
        /// EXT-X-PROGRAM-DATE-TIME tags SHOULD provide millisecond accuracy.
        /// </remarks>
        public DateTimeOffset DateTime { get; private set; }

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
                DateTime = DateTimeOffset.Parse(content, CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                throw new SerializationException("Failed to parse EXT-X-PROGRAM-DATE-TIME tag.", ex);
            }
        }
    }
}
