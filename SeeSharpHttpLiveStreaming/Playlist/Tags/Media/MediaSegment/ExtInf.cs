using System;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment
{
    /// <summary>
    /// Represents Media Segment tag EXTINF.
    /// </summary>
    /// <remarks>
    /// The EXTINF tag specifies the duration of a Media Segment. It applies
    /// only to the next Media Segment. This tag is REQUIRED for each Media
    /// Segment. Its format is:
    /// # EXTINF:&lt;duration&gt;,&lt;title&gt;
    /// 
    /// where duration is a decimal-integer or decimal-floating-point number
    /// (as described in Section 4.2) that specifies the duration of the
    /// Media Segment in seconds. Durations that are reported as integers
    /// SHOULD be rounded to the nearest integer. Durations MUST be integers
    /// if the compatibility version number is less than 3 to support older
    /// clients. Durations SHOULD be floating-point if the compatibility
    /// version number is 3 or greater. The remainder of the line following
    /// the comma is an optional human-readable informative title of the
    /// Media Segment.
    /// </remarks>
    public class ExtInf : BaseTag
    {
        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXTINF"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtInf; }
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public decimal Duration { get; private set; }

        /// <summary>
        /// Gets the information.
        /// </summary>
        public string Information { get; private set; }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>..
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public override void Deserialize(string content, int version)
        {
            content.RequireNotEmpty("content");
            try
            {
                var split = content.Split(',');
                if (split.Length != 2)
                {
                    throw new SerializationException("The EXTINF tag did not contain valid value. Len: " + split.Length);
                }
                
                var durationString = split[0];
                var duration = ValueParser.ParseDecimal(durationString);
                
                Duration = version < 3 ? Math.Round(duration, 0, MidpointRounding.AwayFromZero) : duration;
                Information = split[1];
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse EXTINF tag.", ex);
            }
        }
    }
}
