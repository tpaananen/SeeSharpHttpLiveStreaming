using System;
using System.Runtime.Serialization;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents interface for HTTP Live Streaming playlist wrappers.
    /// </summary>
    internal interface IHlsPlaylist
    {

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the playlist has not been initialized.
        /// </exception>
        int Version { get; }

        /// <summary>
        /// Gets the playlist.
        /// </summary>
        PlaylistBase Playlist { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Playlist"/> is a master playlist.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the playlist has not been initialized.
        /// </exception>
        bool IsMaster { get; }

        /// <summary>
        /// Parses an object from the string content.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">
        /// Thrown when parsing fails.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the playlist has already been deserialized.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="content"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="content"/> is empty string.
        /// </exception>
        void Parse(string content);
    }
}