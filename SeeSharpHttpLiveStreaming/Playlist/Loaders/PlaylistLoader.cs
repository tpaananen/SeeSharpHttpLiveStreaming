using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist.Loaders
{
    /// <summary>
    /// Represents the playlist loader.
    /// </summary>
    internal class PlaylistLoader : IPlaylistLoader
    {

        internal static readonly IReadOnlyCollection<string> ValidFileExtensions
            = new ReadOnlyCollection<string>(new [] { ".m3u8", ".m3u" });

        internal static readonly IReadOnlyCollection<string> ValidContentTypes 
            = new ReadOnlyCollection<string>(new [] { "application/vnd.apple.mpegurl", "audio/mpegurl" });

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
                var content = client.DownloadString(uri);
                ValidateContentByUriOrHeaders(uri, client.ResponseHeaders);
                return content;
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
                var content = await client.DownloadStringTaskAsync(uri).ConfigureAwait(false);
                ValidateContentByUriOrHeaders(uri, client.ResponseHeaders);
                return content;
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
        /// Validates the content by URI or headers.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="responseHeaders">The response headers.</param>
        /// <remarks>
        /// Each Playlist file MUST be identifiable by either the path component
        /// of its URI or by HTTP Content-Type. In the first case, the path MUST
        /// end with either .m3u8 or .m3u (for compatibility).  In the second,
        /// the HTTP Content-type MUST be "application/vnd.apple.mpegurl" or
        /// "audio/mpegurl" (for compatibility). Client SHOULD refuse to parse
        /// Playlists that are not so identified.
        /// </remarks>
        private static void ValidateContentByUriOrHeaders(Uri uri, NameValueCollection responseHeaders)
        {
            if (uri.IsFile)
            {
                var filename = Path.GetFileName(uri.LocalPath);
                var extension = Path.GetExtension(filename);
                if (ValidFileExtensions.Contains(extension))
                {
                    return;
                }
            }
            else
            {
                var contentType = responseHeaders.Get("Content-Type");
                if (ValidContentTypes.Contains(contentType))
                {
                    return;
                }
            }

            throw new SerializationException("The content cannot be identified as a proper playlist file " 
                                            + " by URI or content type of response headers.");
        }
    }
}
