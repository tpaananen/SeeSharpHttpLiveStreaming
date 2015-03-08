using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
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
        public bool IsMaster
        {
            get { return Playlist is MasterPlaylist; }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public int Version 
        {
            get
            {
                if (Playlist == null)
                {
                    throw new InvalidOperationException("The playlist has not been parsed.");
                }
                return Playlist.Version;
            }
        }

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
        public void Parse(string content)
        {
            content.RequireNotEmpty("content");

            if (Playlist != null)
            {
                throw new InvalidOperationException("The playlist is already deserialized.");
            }
            
            try
            {
                IReadOnlyCollection<PlaylistLine> playlist = TagParser.ReadLines(content);
                Playlist = PlaylistBase.Create(playlist);
            }
            catch (Exception ex)
            {
                if (ex is SerializationException)
                {
                    throw;
                }
                throw new SerializationException("Failed to parse playlist file.", ex);
            }
        }
    }
}
