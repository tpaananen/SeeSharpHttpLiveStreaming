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
        public MediaPlaylist(IReadOnlyCollection<PlaylistLine> playlist)
        {
            Parse(playlist);
        }

        /// <summary>
        /// Deserializes a <see cref="MediaPlaylist"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        private void Parse(IReadOnlyCollection<PlaylistLine> content)
        {
            try
            {
                // Algorithm:
                // 1. Parse tags that are not media segment tags as usual
                // 2. If the tag is a media segment tag:
                // 2.1 Create media segment
                // 2.2 Feed media segment as long as it takes tags
                // 2.3 When media segment rejects to parse tag, add media segment to the list of media segments and create a new media segment and go to 2.1
                // 3. Add media segment to the list of media segments if it was created and has child tags

                foreach (var line in content)
                {
                    if (Tag.IsMasterTag(line.Tag))
                    {
                        throw new SerializationException("The tag " + line.Tag + " is a master playlist tag. Media playlist tag must not contain master playlist tags.");
                    }
                    ProcessSingleLine(line);
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
