namespace SeeSharpHttpLiveStreaming.Playlist.Loaders
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPlaylistLoaderFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        IPlaylistLoader Create();
    }

}
