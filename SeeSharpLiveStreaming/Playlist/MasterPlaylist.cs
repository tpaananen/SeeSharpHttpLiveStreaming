using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SeeSharpLiveStreaming.Playlist.Tags;

namespace SeeSharpLiveStreaming.Playlist
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
                foreach (var line in content)
                {
                    if (Tag.IsMediaPlaylistTag(line.Tag) || Tag.IsMediaSegmentTag(line.Tag))
                    {
                        throw new SerializationException("The tag " + line.Tag + " is not a master playlist tag. Master playlist tag must not contain other than master playlist tags or basic tags.");
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
                throw new SerializationException(string.Format("Failed to deserialize {0} class.", typeof(MasterPlaylist).Name), ex);
            }
        }
    }
}
