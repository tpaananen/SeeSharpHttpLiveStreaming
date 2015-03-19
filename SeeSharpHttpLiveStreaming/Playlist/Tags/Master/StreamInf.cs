using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Master
{
    /// <summary>
    /// The EXT-X-STREAM-INF tag specifies a Variant Stream, which is a set
    /// of Renditions which can be combined to play the presentation.The
    /// attributes of the tag provide information about the Variant Stream.
    /// The EXT-X-STREAM-INF tag identifies the next URI line in the Playlist
    /// as a Rendition of the Variant Stream.
    /// Its format is:
    /// #EXT-X-STREAM-INF:&lt;attribute-list&gt;
    /// </summary>
    public class StreamInf : StreamInfBaseTag
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamInf"/> class.
        /// </summary>
        public StreamInf()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamInf"/> class.
        /// </summary>
        /// <param name="bandwidth">The bandwidth.</param>
        /// <param name="averageBandwidth">The average bandwidth.</param>
        /// <param name="codecs">The codecs.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="audio">The audio.</param>
        /// <param name="video">The video.</param>
        /// <param name="subtitles">The subtitles.</param>
        /// <param name="closedCaptions">The closed captions.</param>
        public StreamInf(long bandwidth, long averageBandwidth, IEnumerable<string> codecs,
                         Resolution resolution, string audio, string video, string subtitles, string closedCaptions)
        {
            Bandwidth = bandwidth;
            AverageBandwidth = averageBandwidth;
            Resolution = resolution;
            Audio = audio ?? string.Empty;
            Video = video ?? string.Empty;
            Subtitles = subtitles ?? string.Empty;
            ClosedCaptions = string.IsNullOrEmpty(closedCaptions) ? "NONE" : closedCaptions;
            if (codecs != null)
            {
                _codecs.AddRange(codecs);
            }
        }

        /// <summary>
        /// The value is a quoted-string. It MUST match the value of the GROUP-
        /// ID attribute of an EXT-X-MEDIA tag elsewhere in the Master Playlist
        /// whose TYPE attribute is AUDIO. It indicates the set of audio
        /// Renditions that MAY be used when playing the presentation. See
        /// Section 4.3.4.2.1. of the specification.
        /// The AUDIO attribute is OPTIONAL.
        /// </summary>
        public string Audio { get; private set; }

        /// <summary>
        /// The value is a quoted-string. It MUST match the value of the GROUP-
        /// ID attribute of an EXT-X-MEDIA tag elsewhere in the Master Playlist
        /// whose TYPE attribute is SUBTITLES. It indicates the set of subtitle
        /// Renditions that MAY be used when playing the presentation.See
        /// Section 4.3.4.2.1.
        /// The SUBTITLES attribute is OPTIONAL.
        /// </summary>
        public string Subtitles { get; private set; }

        /// <summary>
        /// The value can be either a quoted-string or an enumerated-string with
        /// the value NONE.If the value is a quoted-string, it MUST match the
        /// value of the GROUP-ID attribute of an EXT-X-MEDIA tag elsewhere in
        /// the Playlist whose TYPE attribute is CLOSED-CAPTIONS, and indicates
        /// the set of closed-caption Renditions that MAY be used when playlist
        /// the presentation. See Section 4.3.4.2.1.
        /// If the value is the enumerated-string value NONE, all EXT-X-STREAM-
        /// INF tags MUST have this attribute with a value of NONE, indicating
        /// that there are no closed captions in any Variant Stream in the Master
        /// Playlist. Having closed captions in one Variant Stream but not
        /// another can trigger playback inconsistencies.
        /// The CLOSED-CAPTIONS attribute is OPTIONAL.
        /// </summary>
        public string ClosedCaptions { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has closed captions.
        /// </summary>
        public bool HasClosedCaptions
        {
            get
            {
                return ClosedCaptions != null &&
                       !ClosedCaptions.Equals("NONE", StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-STREAM-INF"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXStreamInf; }
        }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>..
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        public override void Deserialize(string content, int version)
        {
            content.RequireNotEmpty("content");
            try
            {
                base.Deserialize(content, version);
                ParseAudio(content);
                ParseSubtitles(content);
                ParseClosedCaptions(content);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to deserialize EXT-X-STREAM-INF tag.", ex);
            }
        }

        /// <summary>
        /// Serializes the attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void SerializeAttributes(IPlaylistWriter writer)
        {
            bool hasPreviousAttributes;
            SerializeBaseAttributes(writer, out hasPreviousAttributes);
            WriteAudio(writer, ref hasPreviousAttributes);
            WriteSubtitles(writer, ref hasPreviousAttributes);
            WriteClosedCaptions(writer, ref hasPreviousAttributes);
        }

        private void WriteAudio(IPlaylistWriter writer, ref bool hasPreviousAttributes)
        {
            const string template = "AUDIO=\"{0}\"";
            if (!string.IsNullOrEmpty(Audio))
            {
                WriteAttributeSeparator(writer, hasPreviousAttributes);
                writer.Write(string.Format(template, Audio));
            }
        }

        private void WriteSubtitles(IPlaylistWriter writer, ref bool hasPreviousAttributes)
        {
            const string template = "SUBTITLES=\"{0}\"";
            if (!string.IsNullOrEmpty(Subtitles))
            {
                WriteAttributeSeparator(writer, hasPreviousAttributes);
                writer.Write(string.Format(template, Subtitles));
            }
        }

        private void WriteClosedCaptions(IPlaylistWriter writer, ref bool hasPreviousAttributes)
        {
            WriteAttributeSeparator(writer, hasPreviousAttributes);
            if (ClosedCaptions != "NONE")
            {
                const string template = "CLOSED-CAPTIONS=\"{0}\"";
                writer.Write(string.Format(template, ClosedCaptions));
            }
            else
            {
                const string value = "CLOSED-CAPTIONS=NONE";
                writer.Write(value);
            }
        }

        #region Parsing of attribute list

        private void ParseAudio(string content)
        {
            const string name = "AUDIO";
            Audio = ValueParser.ParseQuotedString(name, content, false);
        }

        private void ParseSubtitles(string content)
        {
            const string name = "SUBTITLES";
            Subtitles = ValueParser.ParseQuotedString(name, content, false);
        }

        private void ParseClosedCaptions(string content)
        {
            const string name = "CLOSED-CAPTIONS";
            var value = ValueParser.ParseQuotedString(name, content, false);
            ClosedCaptions = value == string.Empty ? "NONE" : value;
        }

        #endregion
    }
}
