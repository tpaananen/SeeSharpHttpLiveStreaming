using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.Writers;

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
    internal class ExtIFrameStreamInf : StreamInfBaseTag
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamInf"/> class.
        /// </summary>
        internal ExtIFrameStreamInf()
        {
            UsingDefaultCtor = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtIFrameStreamInf" /> class.
        /// </summary>
        /// <param name="bandwidth">The bandwidth.</param>
        /// <param name="averageBandwidth">The average bandwidth.</param>
        /// <param name="codecs">The codecs.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="video">The video.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="uri" /> is <b>null</b>.</exception>
        public ExtIFrameStreamInf(long bandwidth, long averageBandwidth, IEnumerable<string> codecs,
                         Resolution resolution, Uri uri, string video)
        {
            uri.RequireNotNull("uri");
            Bandwidth = bandwidth;
            AverageBandwidth = averageBandwidth;
            Resolution = resolution;
            Video = video ?? string.Empty;
            Uri = uri;
            if (codecs != null)
            {
                _codecs.AddRange(codecs);
            }
        }

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
                Uri = ParseUri("URI", content, true);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse EXT-X-I-FRAME-STREAM-INF tag.", ex);
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
            WriteUri(writer, "URI", Uri, ref hasPreviousAttributes);
        }
    }
}
