using System;
using System.Threading.Tasks;

namespace SeeSharpHttpLiveStreaming.Playlist.Loaders
{
    /// <summary>
    /// Async interface for fetching media from the source.
    /// </summary>
    internal interface IMediaLoader : IDisposable
    {
        /// <summary>
        /// Loads media from the uri specified by the <paramref name="segment"/> asynchronously.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <returns>
        /// Task that reprerents the asynchronous operation.
        /// </returns>
        Task<byte[]> LoadAsync(MediaSegment segment);
    }
}
