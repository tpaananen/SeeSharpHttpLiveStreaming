using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist.Loaders
{
    /// <summary>
    /// Represents the playlist loader.
    /// </summary>
    internal class PlaylistLoader : IPlaylistLoader
    {
        /// <summary>
        /// Loads the content from the <paramref name="uri" />.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The content loaded from the <paramref name="uri" />
        /// </returns>
        /// <exception cref="System.IO.IOException">Failed to load playlist content.</exception>
        public string Load(Uri uri)
        {
            uri.RequireNotNull("uri");
            var client = new WebClient();
            try
            {
                return client.DownloadString(uri);
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to load playlist content.", ex);
            }
            finally
            {
                client.Dispose();
            }
        }

        /// <summary>
        /// Loads the content from the <paramref name="uri" /> asynchronously.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The task that represents the asynchronous operation.
        /// The result of the task contains the content loaded
        /// from the <paramref name="uri" /> when the task completes.
        /// </returns>
        /// <exception cref="System.IO.IOException">Failed to load playlist content.</exception>
        public async Task<string> LoadAsync(Uri uri)
        {
            uri.RequireNotNull("uri");
            var client = new WebClient();
            try
            {
                return await client.DownloadStringTaskAsync(uri).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to load playlist content.", ex);
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
