using System;

namespace SeeSharpHttpLiveStreaming.Utils.Writers
{
    /// <summary>
    /// Represents playlist writer interface.
    /// </summary>
    public interface IPlaylistWriter : IDisposable
    {
        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="line">The line to be write to the writer.</param>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the writer has been disposed of before calling this method.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="line"/> is <b>null</b>.
        /// </exception>
        void WriteLine(string line);
    }
}
