using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using SeeSharpLiveStreaming.Playlist.Tags.Master;

namespace SeeSharpLiveStreaming.Playlist.Tags
{
    public class MediaPlaylist : PlaylistBase
    {

        /// <summary>
        /// Gets the media types.
        /// </summary>
        public IReadOnlyCollection<ExtMedia> MediaTypes { get; private set; }

        /// <summary>
        /// Deserializes a <see cref="MediaPlaylist"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        public override void Deserialize(string content)
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
