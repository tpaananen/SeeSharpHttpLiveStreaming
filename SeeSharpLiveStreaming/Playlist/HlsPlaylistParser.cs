using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeSharpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents a HTTP Live streaming playlist parser.
    /// </summary>
    public static class HlsPlaylistParser
    {
        /// <summary>
        /// Parses the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static IHlsPlaylist Parse(string content)
        {
            var playlist = new HlsPlaylist();
            playlist.Deserialize(content);
            return playlist;
        }
    }
}
