using System.Globalization;
using System.Runtime.Serialization;
using SeeSharpLiveStreaming.Utils;

namespace SeeSharpLiveStreaming.Playlist.Tags.BasicTags
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
    public class ExtXVersion : BaseTag
    {
        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType => TagType.ExtXVersion;

        /// <summary>
        /// Gets the protocol compatibility level version number.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="content"></param>
        public override void Deserialize(string content)
        {
            content.RequireNotNull("content");
            if (content == string.Empty)
            {
                Version = 0;
            }
            else
            {
                int version;
                if (!int.TryParse(content, NumberStyles.Integer, CultureInfo.InvariantCulture, out version))
                {
                    throw new SerializationException("Faild to parse version number attribute of the EXT-X-VERSION tag.");
                }
                Version = version;
            }
        }
    }
}
