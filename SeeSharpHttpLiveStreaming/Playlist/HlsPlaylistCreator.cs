using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SeeSharpHttpLiveStreaming.Playlist.Loaders;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents the playlist creator. This class can be used to 
    /// read the content from the source and deserialize the playlist file.
    /// </summary>
    public sealed class HlsPlaylistCreator
    {
        private readonly IPlaylistLoaderFactory _factory = new PlaylistLoaderFactory();

        /// <summary>
        /// Creates the playlist from <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The playlist instance loaded and serialized from the <paramref name="uri"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="uri"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown when loading of the content of the playlist fails.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Thrown when (de)serialization of the content fails.
        /// </exception>
        public IHlsPlaylist CreateFrom(Uri uri)
        {
            var loader = new HlsPlaylistReader(_factory);
            var content = loader.Read(uri);
            return HlsPlaylistParser.Parse(content);
        }

        /// <summary>
        /// Creates the playlist from <paramref name="uri"/> asynchronously.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The playlist instance loaded and serialized from the <paramref name="uri"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="uri"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown when loading of the content of the playlist fails.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Thrown when (de)serialization of the content fails.
        /// </exception>
        public async Task<IHlsPlaylist> CreateFromAsync(Uri uri)
        {
            var loader = new HlsPlaylistReader(_factory);
            var content = await loader.ReadAsync(uri).ConfigureAwait(false);
            return HlsPlaylistParser.Parse(content);
        }
    }
}
