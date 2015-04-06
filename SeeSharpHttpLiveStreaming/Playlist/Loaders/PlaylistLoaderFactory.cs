namespace SeeSharpHttpLiveStreaming.Playlist.Loaders
{
    /// <summary>
    /// Represents the playlist loader factory to create concrete 
    /// instances of the <see cref="IPlaylistLoader"/> interface.
    /// </summary>
    internal sealed class PlaylistLoaderFactory : IPlaylistLoaderFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IPlaylistLoader"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="IPlaylistLoader"/> instance.
        /// </returns>
        public IPlaylistLoader Create()
        {
            return new PlaylistLoader();
        }
    }
}
