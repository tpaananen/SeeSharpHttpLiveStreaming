namespace SeeSharpHttpLiveStreaming.Playlist.Loaders
{
    /// <summary>
    /// Represents playlist loader factory interface.
    /// </summary>
    internal interface IPlaylistLoaderFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IPlaylistLoader"/>.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="IPlaylistLoader"/>.
        /// </returns>
        IPlaylistLoader Create();
    }

}
