using System;
using System.IO;
using System.Runtime.Serialization;
using SeeSharpLiveStreaming.Playlist.Tags;
using SeeSharpLiveStreaming.Utils;

namespace SeeSharpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents HTTP Live streaming playlist wrapper.
    /// </summary>
    public class HlsPlaylist : IHlsPlaylist
    {

        /// <summary>
        /// Gets the playlist.
        /// </summary>
        public PlaylistBase Playlist { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this playlist is a master playlist.
        /// </summary>
        public bool IsMaster => Playlist is MasterPlaylist;

        /// <summary>
        /// Gets the playlist type.
        /// </summary>
        public PlaylistType PlaylistType { get; private set; }

        /// <summary>
        /// Gets the content type.
        /// </summary>
        public ContentType ContentType { get; private set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">
        /// Thrown when the serialization fails.
        /// </exception>
        public void Deserialize(string content)
        {
            content.RequireNotNull("content");

            if (Playlist != null)
            {
                throw new InvalidOperationException("The playlist is already deserialized.");
            }

            using (var reader = new StringReader(content))
            {
                Playlist = PlaylistBase.Create(reader);
            }
        }
    }
}
