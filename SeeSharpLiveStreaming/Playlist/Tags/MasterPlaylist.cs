using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SeeSharpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Represents a master playlist.
    /// </summary>
    public class MasterPlaylist : PlaylistBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPlaylist" /> class.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        public MasterPlaylist(string playlist)
            : base(playlist)
        {
        }

        /// <summary>
        /// Deserializes a <see cref="MasterPlaylist"/>.
        /// </summary>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        public override void Deserialize()
        {
            
        }
    }
}
