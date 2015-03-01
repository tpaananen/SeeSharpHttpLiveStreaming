using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SeeSharpLiveStreaming.Utils;

namespace SeeSharpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Represents a master playlist.
    /// </summary>
    public sealed class MasterPlaylist : PlaylistBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPlaylist"/> class.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        public MasterPlaylist(IList<PlaylistLine> playlist)
        {
            Parse(playlist);
        }

        /// <summary>
        /// Deserializes a <see cref="MasterPlaylist"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        protected override void Parse(IList<PlaylistLine> content)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("Failed to deserialize {0} class.", typeof(MediaPlaylist).Name), ex);
            }
        }
    }
}
