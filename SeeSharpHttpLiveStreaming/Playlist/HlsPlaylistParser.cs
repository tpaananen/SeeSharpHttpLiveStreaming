using System;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents a HTTP Live streaming playlist parser.
    /// </summary>
    internal static class HlsPlaylistParser
    {
        /// <summary>
        /// Parses the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="baseUri">The base URI.</param>
        /// <returns>
        /// The <see cref="IHlsPlaylist"/>.
        /// </returns>
        internal static IHlsPlaylist Parse(string content, Uri baseUri)
        {
            var playlist = new HlsPlaylist(baseUri);
            playlist.Parse(content);
            return playlist;
        }
    }
}
