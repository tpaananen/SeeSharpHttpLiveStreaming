using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Master
{
    /// <summary>
    /// Base class for stream inf tags.
    /// </summary>
    public abstract class StreamInfBaseTag : BaseTag
    {
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// The codec list.
        /// </summary>
        protected readonly List<string> _codecs = new List<string>(); 

        /// <summary>
        /// The value is a quoted-string. It MUST match the value of the GROUP-
        /// ID attribute of an EXT-X-MEDIA tag elsewhere in the Master Playlist
        /// whose TYPE attribute is VIDEO. It indicates the set of video
        /// Renditions that MAY be used when playing the presentation.See
        /// Section 4.3.4.2.1. of the specification.
        /// The VIDEO attribute is OPTIONAL.
        /// </summary>
        public string Video { get; protected set; }

        /// <summary>
        /// The value is a decimal-integer of bits per second. It represents the
        /// peak segment bit rate of the Variant Stream.
        /// If all the Media Segments in a Variant Stream have already been
        /// created, the BANDWIDTH value MUST be the largest sum of peak segment
        /// bit rates that is produced by any playable combination of Renditions.
        /// (For a Variant Stream with a single Media Playlist, this is just the
        /// peak segment bit rate of that Media Playlist.) An inaccurate value
        /// can cause playback stalls or prevent clients from playing the
        /// variant.
        /// If the Master Playlist is to be made available before all Media
        /// Segments in the presentation have been encoded, the BANDWIDTH value
        /// SHOULD be the BANDWIDTH value of a representative period of similar
        /// content, encoded using the same settings.
        /// Every EXT-X-STREAM-INF tag MUST include the BANDWIDTH attribute.
        /// </summary>
        public long Bandwidth { get; protected set; }

        /// <summary>
        /// The value is a decimal-integer of bits per second. It represents the
        /// average segment bit rate of the Variant Stream.
        /// If all the Media Segments in a Variant Stream have already been
        /// created, the AVERAGE-BANDWIDTH value MUST be the largest sum of
        /// average segment bit rates that is produced by any playable
        /// combination of Renditions. (For a Variant Stream with a single Media
        /// Playlist, this is just the average segment bit rate of that Media
        /// Playlist.) An inaccurate value can cause playback stalls or prevent
        /// clients from playing the variant.
        /// If the Master Playlist is to be made available before all Media
        /// Segments in the presentation have been encoded, the AVERAGE-BANDWIDTH
        /// value SHOULD be the AVERAGE-BANDWIDTH value of a representative
        /// period of similar content, encoded using the same settings.
        /// The AVERAGE-BANDWIDTH attribute is OPTIONAL.
        /// </summary>
        public long AverageBandwidth { get; protected set; }

        /// <summary>
        /// The value is a quoted-string containing a comma-separated list of
        /// formats, where each format specifies a media sample type that is
        /// present in one or more Renditions specified by the Variant Stream.
        /// Valid format identifiers are those in the ISO Base Media File Format
        /// Name Space defined by The ’Codecs’ and ’Profiles’ Parameters for
        /// "Bucket" Media Types [RFC6381].
        /// Every EXT-X-STREAM-INF tag SHOULD include a CODECS attribute.
        /// </summary>
        public IReadOnlyCollection<string> Codecs 
        {
            get
            {
                return new ReadOnlyCollection<string>(_codecs);
            } 
        }

        /// <summary>
        /// The value is a decimal-resolution describing the optimal pixel
        /// resolution at which to display all the video in the Variant Stream.
        /// The RESOLUTION attribute is OPTIONAL but is recommended if the
        /// Variant Stream includes video.
        /// </summary>
        public Resolution Resolution { get; protected set; }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        public override void Deserialize(string content, int version)
        {
            content.RequireNotEmpty("content");
            ParseVideo(content);
            ParseBandwidth(content);
            ParseAverageBandwidth(content);
            ParseCodecs(content);
            ParseResolution(content);
        }

        /// <summary>
        /// Serializes the attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="hasPreviousAttributes">if set to <c>true</c> has previous attributes.</param>
        protected void SerializeBaseAttributes(IPlaylistWriter writer, out bool hasPreviousAttributes)
        {
            hasPreviousAttributes = false;
            WriteVideo(writer, ref hasPreviousAttributes);
            WriteBandwith(writer, ref hasPreviousAttributes);
            WriteAverageBandwith(writer, ref hasPreviousAttributes);
            WriteCodecs(writer, ref hasPreviousAttributes);
            WriteResolution(writer, ref hasPreviousAttributes);
        }

        private void WriteVideo(IPlaylistWriter writer, ref bool hasPreviousAttributes)
        {
            if (string.IsNullOrEmpty(Video))
            {
                return;
            }
            const string name = "VIDEO=\"{0}\"";
            WriteAttributeSeparator(writer, hasPreviousAttributes);
            var value = string.Format(name, Video);
            writer.Write(value);
            hasPreviousAttributes = true;
        }

        private void WriteBandwith(IPlaylistWriter writer, ref bool hasPreviousAttributes)
        {
            const string name = "BANDWIDTH=";
            WriteAttributeSeparator(writer, hasPreviousAttributes);
            writer.Write(name + Bandwidth.ToString(CultureInfo.InvariantCulture));
            hasPreviousAttributes = true;
        }

        private void WriteAverageBandwith(IPlaylistWriter writer, ref bool hasPreviousAttributes)
        {
            if (AverageBandwidth == 0)
            {
                return;
            }
            const string name = "AVERAGE-BANDWIDTH=";
            WriteAttributeSeparator(writer, hasPreviousAttributes);
            writer.Write(name + AverageBandwidth.ToString(CultureInfo.InvariantCulture));
            hasPreviousAttributes = true;
        }

        private void WriteCodecs(IPlaylistWriter writer, ref bool hasPreviousAttributes)
        {
            if (_codecs.Count == 0)
            {
                return;
            }
            const string name = "CODECS=\"{0}\"";
            WriteAttributeSeparator(writer, hasPreviousAttributes);
            var value = string.Join(",", _codecs);
            writer.Write(string.Format(name, value));
            hasPreviousAttributes = true;
        }

        private void WriteResolution(IPlaylistWriter writer, ref bool hasPreviousAttributes)
        {
            if (Resolution == Resolution.Default)
            {
                return;
            }
            const string name = "RESOLUTION={0}x{1}";
            WriteAttributeSeparator(writer, hasPreviousAttributes);
            var value = string.Format(name, Resolution.X, Resolution.Y);
            writer.Write(value);
            hasPreviousAttributes = true;
        }

        /// <summary>
        /// Parses the video attribute.
        /// </summary>
        /// <param name="content">The content.</param>
        protected virtual void ParseVideo(string content)
        {
            const string name = "VIDEO";
            Video = ValueParser.ParseQuotedString(name, content, false);
        }

        /// <summary>
        /// Parses the bandwidth attribute.
        /// </summary>
        /// <param name="content">The content.</param>
        protected virtual void ParseBandwidth(string content)
        {
            const string name = "BANDWIDTH";
            Bandwidth = ValueParser.ParseInt(name, content, true);
        }

        /// <summary>
        /// Parses the average bandwidth attribute.
        /// </summary>
        /// <param name="content">The content.</param>
        protected virtual void ParseAverageBandwidth(string content)
        {
            const string name = "AVERAGE-BANDWIDTH";
            AverageBandwidth = ValueParser.ParseInt(name, content, false);
        }

        /// <summary>
        /// Parses the codecs attribute.
        /// </summary>
        /// <param name="content">The content.</param>
        protected virtual void ParseCodecs(string content)
        {
            const string name = "CODECS";
            _codecs.AddRange(ValueParser.ParseSeparatedQuotedString(name, content, false)); // SHOULD
        }

        /// <summary>
        /// Parses the resolution attribute.
        /// </summary>
        /// <param name="content">The content.</param>
        protected virtual void ParseResolution(string content)
        {
            const string name = "RESOLUTION";
            Resolution = ValueParser.ParseResolution(name, content, false); // RECOMMENDED IF VIDEO
        }
    }
}
