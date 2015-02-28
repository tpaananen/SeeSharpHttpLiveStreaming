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

        private readonly string _playlist;

        /// <summary>
        /// Initializes a new instance of the <see cref="HlsPlaylist"/> class.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        public HlsPlaylist(string playlist)
        {
            Require.RequireNotNull(playlist, "playlist");
            _playlist = playlist;
        }

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
        /// <exception cref="SerializationException">
        /// Thrown when the serialization fails.
        /// </exception>
        public void Deserialize()
        {
            if (Playlist != null)
            {
                throw new InvalidOperationException("The playlist is already deserialized.");
            }

            using (var reader = new StringReader(_playlist))
            {
                Playlist = PlaylistBase.Create(reader);
            }
        }
    }
}
