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
        /// Creates a specific playlist depending on content of the <paramref name="content" />.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>
        /// The <see cref="PlaylistBase" /> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="content"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="content"/> is empty string.
        /// </exception>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        internal static PlaylistBase Create(string content)
        {
            content.RequireNotEmpty("content");
            try
            {
                IReadOnlyCollection<PlaylistLine> playlist = TagParser.ReadLines(content);
                string firstTag = GetFirstNonCommonTag(playlist);
                return CreatePlaylistByTag(firstTag, playlist);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to create playlist.", ex);
            }
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
