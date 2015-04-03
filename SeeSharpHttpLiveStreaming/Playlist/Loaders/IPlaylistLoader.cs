using System;
using System.IO;
using System.Threading.Tasks;

namespace SeeSharpHttpLiveStreaming.Playlist.Loaders
{
    /// <summary>
    /// Represents interface for playlist loaders. 
    /// Playlist loader should handle loading of the playlist from the provided URI.
    /// </summary>
    public interface IPlaylistLoader
    {
        /// <summary>
        /// Loads the content from the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The content loaded from the <paramref name="uri"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="uri"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown when the content cannot be loaded from the source.
        /// </exception>
        string Load(Uri uri);

        /// <summary>
        /// Loads the content from the <paramref name="uri"/> asynchronously.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The task that represents the asynchronous operation. 
        /// The result of the task contains the content loaded 
        /// from the <paramref name="uri"/> when the task completes.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="uri"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown when the content cannot be loaded from the source.
        /// </exception>
        Task<string> LoadAsync(Uri uri);
    }
}
