using System;
using System.IO;
using System.Text;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Tests.Helpers
{
    internal static class TestPlaylistWriterFactory
    {

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <returns></returns>
        private static Encoding GetEncoding()
        {
            return new UTF8Encoding(false);
        }

        /// <summary>
        /// Creates the writer.
        /// </summary>
        /// <returns>
        /// The <see cref="IPlaylistWriter"/> instance.
        /// </returns>
        public static IPlaylistWriter Create()
        {
            var stringBuilder = new StringBuilder();
            var encoding = GetEncoding();
            var memoryStream = new WriteOnlyStream(stringBuilder, encoding);
            var internalWriter = new StreamWriter(memoryStream, encoding);
            return new PlaylistWriter(internalWriter);
        }

        /// <summary>
        /// Creates the writer using the <paramref name="stream"/> as a backing store.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static IPlaylistWriter Create(Stream stream)
        {
            stream.RequireNotNull("stream");
            var internalWriter = new StreamWriter(stream, GetEncoding());
            return new PlaylistWriter(internalWriter);
        }

        /// <summary>
        /// Creates the writer with string builder as a backing storage.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <returns>
        /// The <see cref="IPlaylistWriter"/> instance.
        /// </returns>
        public static IPlaylistWriter CreateWithStringBuilder(out StringBuilder stringBuilder)
        {
            stringBuilder = new StringBuilder();
            var encoding = GetEncoding();
            var memoryStream = new WriteOnlyStream(stringBuilder, encoding);
            var internalWriter = new StreamWriter(memoryStream, encoding);
            return new PlaylistWriter(internalWriter);
        }

        internal sealed class WriteOnlyStream : Stream
        {
            private readonly StringBuilder _stringBuilder;
            private readonly Encoding _encoding;

            /// <summary>
            /// Initializes a new instance of the <see cref="WriteOnlyStream" /> class.
            /// </summary>
            /// <param name="stringBuilder">The string builder.</param>
            /// <param name="encoding">The encoding.</param>
            internal WriteOnlyStream(StringBuilder stringBuilder, Encoding encoding)
            {
                stringBuilder.RequireNotNull("stringBuilder");
                encoding.RequireNotNull("encoding");
                _stringBuilder = stringBuilder;
                _encoding = encoding;
            }

            /// <summary>
            /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
            /// </summary>
            public override void Flush()
            {
            }

            /// <summary>
            /// When overridden in a derived class, sets the position within the current stream.
            /// </summary>
            /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
            /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
            /// <returns>
            /// The new position within the current stream.
            /// </returns>
            /// <exception cref="System.NotSupportedException"></exception>
            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Sets the length of the current stream.
            /// </summary>
            /// <param name="value">The desired length of the current stream in bytes.</param>
            public override void SetLength(long value)
            {
                _stringBuilder.Capacity = (int)value;
            }

            /// <summary>
            /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
            /// </summary>
            /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
            /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
            /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
            /// <returns>
            /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
            /// </returns>
            /// <exception cref="System.NotSupportedException"></exception>
            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Writes a block of bytes to the current stream using data read from a buffer.
            /// </summary>
            /// <param name="buffer">The buffer to write data from.</param>
            /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
            /// <param name="count">The maximum number of bytes to write.</param>
            public override void Write(byte[] buffer, int offset, int count)
            {
                _stringBuilder.Append(_encoding.GetString(buffer, offset, count));
            }

            /// <summary>
            /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
            /// </summary>
            public override bool CanRead
            {
                get { return false; }
            }

            /// <summary>
            /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
            /// </summary>
            public override bool CanSeek
            {
                get { return false; }
            }

            /// <summary>
            /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
            /// </summary>
            public override bool CanWrite
            {
                get { return true; }
            }

            /// <summary>
            /// When overridden in a derived class, gets the length in bytes of the stream.
            /// </summary>
            public override long Length
            {
                get { return _stringBuilder.Length; }
            }

            /// <summary>
            /// When overridden in a derived class, gets or sets the position within the current stream.
            /// </summary>
            /// <exception cref="System.NotSupportedException"></exception>
            public override long Position
            {
                get { return _stringBuilder.Length; } 
                set { throw new NotSupportedException(); }
            }
        }
    }
}
