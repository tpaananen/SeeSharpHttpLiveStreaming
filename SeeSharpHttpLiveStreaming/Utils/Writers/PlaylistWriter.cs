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

        private static readonly UTF8Encoding ReferenceEncoding = new UTF8Encoding(false);

        internal readonly TextWriter TextWriter;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistWriter" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when encoding of the <paramref name="writer"/> is not <see cref="UTF8Encoding"/> or 
        /// when the encoding of the <paramref name="writer"/> uses BOM.
        /// </exception>
        /// <remarks>
        /// We could also use <see cref="ASCIIEncoding"/> as specified but we refuse to do so.
        /// </remarks>
        internal PlaylistWriter(TextWriter writer)
        {
            writer.RequireNotNull("writer");
            if (!Equals(writer.Encoding, ReferenceEncoding))
            {
                throw new ArgumentException("The internal writer must use UTF-8 encoding without BOM.");
            }
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
