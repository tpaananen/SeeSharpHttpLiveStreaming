using System;
using System.IO;
using System.Runtime.Serialization;
using SeeSharpLiveStreaming.Playlist.Tags;
using SeeSharpLiveStreaming.Utils;

namespace SeeSharpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents a base class for playlists.
    /// </summary>
    public abstract class PlaylistBase : ISerializable
    {

        /// <summary>
        /// Specifies the reader that holds the content of the playlist.
        /// </summary>
        protected readonly string Playlist;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistBase" /> class.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        protected PlaylistBase(string playlist)
        {
            Require.RequireNotNull(playlist, "playlist");
            Playlist = playlist;
        }

        /// <summary>
        /// Creates a specific playlist depending on content of the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        /// <returns>
        /// The <see cref="PlaylistBase"/> instance.
        /// </returns>
        public static PlaylistBase Create(TextReader reader)
        {
            TagParser.ReadFirstLine(reader);
            string playlist = reader.ReadToEnd();
            string secondLine = TagParser.ReadWhileNonEmptyLine(playlist, 1);
            string tag = TagParser.ParseTag(secondLine);

            return CreatePlaylistByTag(tag, playlist);
        }

        /// <summary>
        /// Creates the playlist by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="playlist">The playlist.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the second tag is invalid. The playlist cannot be parsed.
        /// </exception>
        private static PlaylistBase CreatePlaylistByTag(string tag, string playlist)
        {
            if (Tag.IsMasterTag(tag))
            {
                return new MasterPlaylist(playlist);
            }
            if (Tag.IsMediaPlaylistTag(tag))
            {
                return new MediaPlaylist(playlist);
            }

            throw new ArgumentException("Invalid second tag in reader. Cannot create a playlist instance.");
        }

        /// <summary>
        /// When overridden in a derived class deserializes an instance of <see cref="PlaylistBase"/>.
        /// </summary>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        public abstract void Deserialize();
    }
}
