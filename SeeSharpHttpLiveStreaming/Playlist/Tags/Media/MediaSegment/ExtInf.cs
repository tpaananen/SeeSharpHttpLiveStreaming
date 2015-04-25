using System;
using System.Globalization;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.Writers;

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
    internal class ExtInf : BaseTag
    {
        /// <summary>
        /// Defines the version when decimals can be accepted in <see cref="Duration"/>.
        /// </summary>
        private const int MinVersionForDecimalDuration = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtInf"/> class.
        /// </summary>
        internal ExtInf()
        {
            UsingDefaultCtor = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtInf" /> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="information">The information.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="duration" /> is negative.</exception>
        public ExtInf(decimal duration, string information, int version)
        {
            if (version < 0)
            {
                throw new ArgumentOutOfRangeException("version", version, "The version cannot be negative.");
            }
            try
            {
                Duration = GetDuration(duration, version);
            }
            catch (SerializationException ex)
            {
                throw new ArgumentOutOfRangeException("duration", duration, ex.Message);
            }
            Information = information ?? string.Empty;
        }

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
                var durationString = split[0];
                var duration = decimal.Parse(durationString, CultureInfo.InvariantCulture);
                
                Duration = GetDuration(duration, version);
                Information = split.Length > 1 ? split[1] : string.Empty;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse EXTINF tag.", ex);
            }
        }

        /// <summary>
        /// Serializes the attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void SerializeAttributes(IPlaylistWriter writer)
        {
            writer.Write(Duration.ToString(CultureInfo.InvariantCulture));
            if (!string.IsNullOrEmpty(Information))
            {
                writer.Write(",");
                writer.Write(Information);
            }
        }

        private static decimal GetDuration(decimal duration, int version)
        {
            if (duration <= 0)
            {
                throw new SerializationException("The duration cannot be negative or zero.");
            }
            return version < MinVersionForDecimalDuration ? Math.Round(duration, 0, MidpointRounding.AwayFromZero) : duration;
        }
    }
}
