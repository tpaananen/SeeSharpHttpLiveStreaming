namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Master
{
    /// <summary>
    /// Represents a master tag.
    /// </summary>
    internal abstract class MasterBaseTag : BaseTag
    {
        /// <summary>
        /// When overridden in a derived class adds the tag properties to playlist properties.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        internal abstract void AddToPlaylist(MasterPlaylist playlist);
    }
}
