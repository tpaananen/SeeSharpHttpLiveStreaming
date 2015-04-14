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
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The <see cref="PlaylistBase" /> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="content" /> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="content" /> is empty string.</exception>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        internal static PlaylistBase Create(string content, Uri uri)
        {
            content.RequireNotEmpty("content");
            uri.RequireNotNull("uri");
            try
            {
                IReadOnlyCollection<PlaylistLine> playlist = TagParser.ReadLines(content, uri);
                return CreatePlaylist(playlist, uri);
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
            return playlist.FirstOrDefault(x => !Tag.IsBasicTag(x.Tag) && !Tag.IsMasterOrMediaTag(x.Tag)).Tag ?? string.Empty;
        }

        /// <summary>
        /// Creates the playlist by tags in the playlist.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <param name="baseUri">The base URI.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when playlist type cannot be determined by the tags in the <paramref name="playlist" />.</exception>
        private static PlaylistBase CreatePlaylist(IReadOnlyCollection<PlaylistLine> playlist, Uri baseUri)
        {
            string tag = GetFirstNonCommonTag(playlist);
            if (Tag.IsMasterTag(tag))
            {
                return new MasterPlaylist(playlist, baseUri);
            }
            if (Tag.IsMediaPlaylistTag(tag))
            {
                return new MediaPlaylist(playlist, baseUri);
            }

            throw new ArgumentException("Invalid second tag. Cannot create a playlist instance.");
        }
    }
}
