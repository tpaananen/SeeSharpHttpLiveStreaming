using System;
using System.IO;
using System.Threading.Tasks;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist.Loaders
{
    /// <summary>
    /// Represents the playlist loader by providing both synchronous and 
    /// asynchronous operations to read the file contents.
    /// </summary>
    internal class HlsPlaylistReader
    {
        private readonly IPlaylistLoaderFactory _playlistLoaderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HlsPlaylistReader"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public HlsPlaylistReader(IPlaylistLoaderFactory factory)
        {
            factory.RequireNotNull("factory");
            _playlistLoaderFactory = factory;
        }

        /// <summary>
        /// Reads the playlist from the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The playlist file as a string.
        /// </returns>
        /// <exception cref="IOException">
        /// Thrown when the playlist file cannot be read.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="uri"/> is <b>null</b>.
        /// </exception>
        public string Read(Uri uri)
        {
            IPlaylistLoader loader = _playlistLoaderFactory.Create();
            return loader.Load(uri);
        }

        /// <summary>
        /// Reads the playlist from the <paramref name="uri"/> asynchronously.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The task that represents the asynchronous operation. The result of the task
        /// will contain the playlist file when the task completes.
        /// </returns>
        /// <exception cref="IOException">
        /// Thrown when the playlist file cannot be read.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="uri"/> is <b>null</b>.
        /// </exception>
        public Task<string> ReadAsync(Uri uri)
        {
            IPlaylistLoader loader = _playlistLoaderFactory.Create();
            return loader.LoadAsync(uri);
        }
    }
}
