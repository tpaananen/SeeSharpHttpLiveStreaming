namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media
{
    /// <summary>
    /// Represents a media playlist base tag.
    /// </summary>
    internal abstract class MediaBaseTag : BaseTag
    {
        /// <summary>
        /// When overridden in a derived class adds the tag properties to the playlist.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        internal abstract void AddToPlaylist(MediaPlaylist playlist);
    }
}
