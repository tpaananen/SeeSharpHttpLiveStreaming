using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SeeSharpHttpLiveStreaming.Tests.Helpers
{
    internal class TestWebServer : IDisposable
    {

        private readonly HttpListener _httpListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestWebServer"/> class.
        /// </summary>
        internal TestWebServer()
        {
            _httpListener = new HttpListener();
        }

        /// <summary>
        /// Listens the specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="playlistContent">Content of the playlist.</param>
        public void Listen(ushort port, string contentType, string playlistContent)
        {
            _httpListener.Prefixes.Clear();
            _httpListener.Prefixes.Add(string.Format("{0}://{1}:{2}/", Uri.UriSchemeHttp, "localhost", port));
            _httpListener.Start();
            #pragma warning disable 4014
            BeginGetContext(contentType, playlistContent);
            #pragma warning restore 4014
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private async Task BeginGetContext(string contentType, string playlistContent)
        {
            try
            {
                if (!_httpListener.IsListening)
                {
                    return;
                }

                var context = await _httpListener.GetContextAsync().ConfigureAwait(false);

                #pragma warning disable 4014
                BeginGetContext(contentType, playlistContent);
                #pragma warning restore 4014

                var response = context.Response;
                response.ContentType = contentType;
                response.ContentEncoding = new UTF8Encoding(false);
                var bytes = response.ContentEncoding.GetBytes(playlistContent);
                await response.OutputStream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                response.Close();
            }
            catch (ObjectDisposedException) {}
            catch (HttpListenerException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    _httpListener.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
