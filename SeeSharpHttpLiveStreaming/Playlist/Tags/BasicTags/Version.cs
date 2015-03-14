using System;
using System.Globalization;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.BasicTags
{
    /// <summary>
    /// The EXT-X-VERSION tag indicates the compatibility version of the
    /// Playlist file, its associated media, and its server.
    /// The EXT-X-VERSION tag applies to the entire Playlist file. Its
    /// format is:
    /// #EXT-X-VERSION:&lt;n&gt;
    /// where n is an integer indicating the protocol compatibility version
    /// number.
    /// It MUST appear in all Playlists containing tags or attributes that
    /// are not compatible with protocol version 1 to support
    /// interoperability with older clients. Section 7 specifies the minimum
    /// value of the compatibility version number for any given Playlist
    /// file.
    /// A Playlist file MUST NOT contain more than one EXT-X-VERSION tag. If
    /// a client encounters a Playlist with multiple EXT-X-VERSION tags, it
    /// SHOULD fail to parse it.
    /// </summary>
    public class Version : BaseTag
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Version"/> class.
        /// </summary>
        public Version()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Version"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        internal Version(int version)
        {
            VersionNumber = version;
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-VERSION"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXVersion; }
        }

        /// <summary>
        /// Gets the protocol compatibility level version number.
        /// </summary>
        public int VersionNumber { get; private set; }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>..
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the <paramref name="version"/> is not zero.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Thrown when the parsing of an integer fails.
        /// </exception>
        public override void Deserialize(string content, int version)
        {
            if (version != 0)
            {
                throw new InvalidOperationException("The version number must be zero when deserializing EXT-X-VERSION tag.");
            }

            content.RequireNotNull("content");
            if (content == string.Empty)
            {
                VersionNumber = 1;
            }
            else
            {
                if (!int.TryParse(content, NumberStyles.Integer, CultureInfo.InvariantCulture, out version))
                {
                    throw new SerializationException("Faild to parse version number attribute of the EXT-X-VERSION tag.");
                }
                VersionNumber = version;
            }
        }

        /// <summary>
        /// Serializes the tag to the <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Serialize(IPlaylistWriter writer)
        {
            var line = TagName + Tag.TagEndMarker + VersionNumber.ToString(CultureInfo.InvariantCulture);
            writer.WriteLine(line);
        }
    }
}
