using System;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents HTTP Live streaming playlist wrapper.
    /// </summary>
    public class HlsPlaylist : IHlsPlaylist
    {

        /// <summary>
        /// Gets the playlist.
        /// </summary>
        public PlaylistBase Playlist { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this playlist is a master playlist.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the playlist has not been initialized.
        /// </exception>
        public bool IsMaster
        {
            get
            {
                if (Playlist == null)
                {
                    throw new InvalidOperationException("The playlist has not been initialized.");
                }
                return Playlist is MasterPlaylist;
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the playlist has not been initialized.
        /// </exception>
        public int Version 
        {
            get
            {
                if (Playlist == null)
                {
                    throw new InvalidOperationException("The playlist has not been initialized.");
                }
                return Playlist.Version;
            }
        }

        /// <summary>
        /// Parses an object from the string content.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">
        /// Thrown when parsing of the <paramref name="content"/> fails.
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
        public void Parse(string content)
        {
            content.RequireNotEmpty("content");

            if (Playlist != null)
            {
                throw new InvalidOperationException("The playlist is already deserialized.");
            }
            Playlist = PlaylistFactory.Create(content);
        }
    }
}
