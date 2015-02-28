namespace SeeSharpLiveStreaming.Playlist
{
    public interface IHlsPlaylist : ISerializable
    {

        /// <summary>
        /// Gets the playlist type.
        /// </summary>
        PlaylistType PlaylistType { get; }

        /// <summary>
        /// Gets the content type.
        /// </summary>
        ContentType ContentType { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Gets the playlist.
        /// </summary>
        PlaylistBase Playlist { get; }
    }
}