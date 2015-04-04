using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents the playlist factory.
    /// </summary>
    internal static class PlaylistFactory
    {
        /// <summary>
        /// Creates a specific playlist depending on content of the <paramref name="playlist" />.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <returns>
        /// The <see cref="PlaylistBase" /> instance.
        /// </returns>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        internal static PlaylistBase Create(IReadOnlyCollection<PlaylistLine> playlist)
        {
            playlist.RequireNotEmpty("playlist");

            var firstTag = GetFirstNonCommonTag(playlist);
            return CreatePlaylistByTag(firstTag, playlist);
        }

        /// <summary>
        /// Gets the first non common tag.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <returns></returns>
        private static string GetFirstNonCommonTag(IEnumerable<PlaylistLine> playlist)
        {
            return playlist.FirstOrDefault(x => !Tag.IsBasicTag(x.Tag)).Tag ?? string.Empty;
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
        private static PlaylistBase CreatePlaylistByTag(string tag, IReadOnlyCollection<PlaylistLine> playlist)
        {
            if (Tag.IsMasterTag(tag))
            {
                return new MasterPlaylist(playlist);
            }
            if (Tag.IsMediaPlaylistTag(tag))
            {
                return new MediaPlaylist(playlist);
            }

            throw new ArgumentException("Invalid second tag. Cannot create a playlist instance.");
        }
    }
}
