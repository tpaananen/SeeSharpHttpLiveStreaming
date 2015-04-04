using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SeeSharpHttpLiveStreaming.Utils.Writers
{
    /// <summary>
    /// Represents playlist writer.
    /// </summary>
    internal class PlaylistWriter : IPlaylistWriter
    {

        internal readonly TextWriter TextWriter;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistWriter" /> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="stream"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the <paramref name="bufferSize"/> is out of range.
        /// </exception>
        /// <remarks>
        /// The stream is owned by the writer from now own. When the writer instance is 
        /// being disposed of, the stream also is closed.
        /// </remarks>
        internal PlaylistWriter(Stream stream, int bufferSize = ushort.MaxValue)
        {
            stream.RequireNotNull("stream");
            var encoding = new UTF8Encoding(false);
            var writer = new StreamWriter(stream, encoding, bufferSize, false);
            TextWriter = writer;
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value to be written to the current line.</param>
        public void Write(string value)
        {
            value.RequireNotNull("line");
            ThrowIfDisposedOf();
            TextWriter.Write(value);
        }

        /// <summary>
        /// Writes the line end.
        /// </summary>
        public void WriteLineEnd()
        {
            ThrowIfDisposedOf();
            TextWriter.WriteLine();
            TextWriter.Flush();
        }

        [DebuggerStepThrough]
        private void ThrowIfDisposedOf()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("The playlist writer has been disposed of.");
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
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> 
        /// to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                TextWriter.Flush();
                TextWriter.Dispose();
            }
            _disposed = true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> 
        /// to release only unmanaged resources.
        /// </param>
        internal void Close(bool disposing)
        {
            Dispose(disposing);
        }
    }
}
