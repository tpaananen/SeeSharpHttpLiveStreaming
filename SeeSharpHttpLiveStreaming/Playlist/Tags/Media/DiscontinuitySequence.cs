using System;
using System.Globalization;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media
{

    /// <summary>
    /// Represents the EXT-X-DISCONTINUITY-SEQUENCE tag.
    /// </summary>
    /// <remarks>
    /// The EXT-X-DISCONTINUITY-SEQUENCE tag allows synchronization between
    /// different Renditions of the same Variant Stream or different Variant
    /// Streams that have EXT-X-DISCONTINUITY tags in their Media Playlists.
    ///
    /// Its format is:
    ///
    /// #EXT-X-DISCONTINUITY-SEQUENCE:&lt;number&gt;
    /// 
    /// If the Media Playlist does not contain an EXT-X-DISCONTINUITY-
    /// SEQUENCE tag, then the Discontinuity Sequence Number of the first
    /// Media Segment in the Playlist SHALL be considered to be 0.
    ///
    /// The EXT-X-DISCONTINUITY-SEQUENCE tag MUST appear before the first
    /// Media Segment in the Playlist.
    ///
    /// The EXT-X-DISCONTINUITY-SEQUENCE tag MUST appear before any EXT-
    /// X-DISCONTINUITY tag.
    ///
    /// See Section 6.2.1 and Section 6.2.2 for more information about
    /// setting the value of the EXT-X-DISCONTINUITY-SEQUENCE tag.
    /// </remarks>
    internal class DiscontinuitySequence : BaseTag
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscontinuitySequence"/> class.
        /// </summary>
        internal DiscontinuitySequence()
        {
            UsingDefaultCtor = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscontinuitySequence"/> class.
        /// </summary>
        /// <param name="number">The number.</param>
        public DiscontinuitySequence(long number)
        {
            if (number < 0)
            {
                throw new ArgumentException("The number must be non-negative decimal integer.");
            }
            Number = number;
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-DISCONTINUITY-SEQUENCE"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXDiscontinuitySequence; }
        }

        /// <summary>
        /// Gets the Discontinuity Sequence Number.
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
                throw new SerializationException("Failed to parse " + TagName + " tag.", ex);
            }
        }

        /// <summary>
        /// Serializes the attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void SerializeAttributes(IPlaylistWriter writer)
        {
            writer.Write(Number.ToString(CultureInfo.InvariantCulture));
        }
    }
}
