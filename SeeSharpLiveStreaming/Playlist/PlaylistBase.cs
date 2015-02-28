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
            string content = reader.ReadToEnd();
            string secondLine = TagParser.ReadWhileNonEmptyLine(content, 1);
            string tag = TagParser.ParseTag(secondLine);

            var playlist = CreatePlaylistByTag(tag);
            playlist.Deserialize(content);
            return playlist;
        }

        /// <summary>
        /// Creates the playlist by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the second tag is invalid. The playlist cannot be parsed.
        /// </exception>
        private static PlaylistBase CreatePlaylistByTag(string tag)
        {
            if (Tag.IsMasterTag(tag))
            {
                return new MasterPlaylist();
            }
            if (Tag.IsMediaPlaylistTag(tag))
            {
                return new MediaPlaylist();
            }

            throw new ArgumentException("Invalid second tag in reader. Cannot create a playlist instance.");
        }

        /// <summary>
        /// When overridden in a derived class deserializes an instance of <see cref="PlaylistBase"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        public abstract void Deserialize(string content);
    }
}
