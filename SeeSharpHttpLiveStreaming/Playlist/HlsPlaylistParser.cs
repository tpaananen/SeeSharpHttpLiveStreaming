namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents a HTTP Live streaming playlist parser.
    /// </summary>
    public static class HlsPlaylistParser
    {
        /// <summary>
        /// Parses the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static IHlsPlaylist Parse(string content)
        {
            var playlist = new HlsPlaylist();
            playlist.Parse(content);
            return playlist;
        }
    }
}
