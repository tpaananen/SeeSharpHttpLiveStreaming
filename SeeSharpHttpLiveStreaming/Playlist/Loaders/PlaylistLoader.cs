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
        private static readonly string[] ContentTypeSplitter = {";"};

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
            if (ValidateByFileName(uri))
            {
                return;
            }
            ValidateByContentType(responseHeaders);
        }

        private static void ValidateByContentType(NameValueCollection responseHeaders)
        {
            string contentType = responseHeaders.Get("Content-Type");
            if (string.IsNullOrEmpty(contentType))
            {
                throw new SerializationException("Failed to get content type for validation.");
            }
            // Get the first instance of splitted string which should be the actual content type
            // There can be charset included in the content type
            contentType = contentType.Split(ContentTypeSplitter, StringSplitOptions.RemoveEmptyEntries)[0];
            if (!ValidContentTypes.Contains(contentType))
            {
                throw new SerializationException("The content cannot be identified as a proper playlist file. "
                                                 + "Content type received " + contentType);
            }
        }

        private static bool ValidateByFileName(Uri uri)
        {
            string filename = Path.GetFileName(WebUtility.UrlDecode(uri.ToString()));
            string extension = Path.GetExtension(filename);
            return ValidFileExtensions.Contains(extension);
        }
    }
}
