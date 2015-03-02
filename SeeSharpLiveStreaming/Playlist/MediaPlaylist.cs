using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SeeSharpLiveStreaming.Playlist.Tags;

namespace SeeSharpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents the media playlist.
    /// </summary>
    public sealed class MediaPlaylist : PlaylistBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlaylist"/> class.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        public MediaPlaylist(IList<PlaylistLine> playlist)
        {
            Parse(playlist);
        }

        /// <summary>
        /// Deserializes a <see cref="MediaPlaylist"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        protected override void Parse(IList<PlaylistLine> content)
        {
            try
            {
                foreach (var line in content)
                {
                    if (Tag.IsMasterTag(line.Tag))
                    {
                        throw new SerializationException("The tag " + line.Tag + " is a master playlist tag. Media playlist tag must not contain master playlist tags.");
                    }
                    CreateLine(line);
                }
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
