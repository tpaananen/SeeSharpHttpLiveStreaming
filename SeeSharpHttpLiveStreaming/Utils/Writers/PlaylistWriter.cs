﻿using System;
using System.Diagnostics;
using System.IO;

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
        /// Initializes a new instance of the <see cref="PlaylistWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal PlaylistWriter(TextWriter writer)
        {
            writer.RequireNotNull("writer");
            TextWriter = writer;
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="line">The line to be write to the writer.</param>
        public void WriteLine(string line)
        {
            line.RequireNotNull("line");
            ThrowIfDisposedOf();
            TextWriter.WriteLine(line);
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
        internal virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                TextWriter.Dispose();
            }
            _disposed = true;
        }
    }
}