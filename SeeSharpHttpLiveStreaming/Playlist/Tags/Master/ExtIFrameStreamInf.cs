using System;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Master
{
    /// <summary>
    /// Represents a EXT-X-I-FRAME-STREAM-INF tag.
    /// </summary>
    /// <remarks>
    /// The EXT-X-I-FRAME-STREAM-INF tag identifies a Media Playlist file
    /// containing the I-frames of a multimedia presentation.  It stands
    /// alone, in that it does not apply to a particular URI in the Master
    /// Playlist.  Its format is:
    /// #EXT-X-I-FRAME-STREAM-INF:&lt;attribute-list&gt;
    /// 
    /// Every EXT-X-I-FRAME-STREAM-INF tag MUST include a BANDWIDTH attribute
    /// and a URI attribute.
    /// 
    /// The provisions in Section 4.3.4.2.1 also apply to EXT-X-I-FRAME-
    /// STREAM-INF tags with a VIDEO attribute.
    /// </remarks>
    public class ExtIFrameStreamInf : StreamInfBaseTag
    {
        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-I-FRAME-STREAM-INF"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXIFrameStreamInf; }
        }

        /// <summary>
        /// The value is a quoted-string containing a URI that identifies the
        /// I-frame Playlist file. 
        /// </summary>
        public Uri Uri { get; private set; }

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
                base.Deserialize(content, version);
                ParseUri(content);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse EXT-X-I-FRAME-STREAM-INF tag.", ex);
            }
        }

        private void ParseUri(string content)
        {
            const string name = "URI";
            var value = ValueParser.ParseQuotedString(name, content, true);
            Uri = new Uri(value);
        }
    }
}
