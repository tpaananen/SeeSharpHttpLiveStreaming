using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SeeSharpLiveStreaming.Utils;

namespace SeeSharpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Represents a master playlist.
    /// </summary>
    public class MasterPlaylist : PlaylistBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPlaylist"/> class.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        public MasterPlaylist(IList<PlaylistLine> playlist)
            : base(playlist)
        {
        }

        /// <summary>
        /// Deserializes a <see cref="MasterPlaylist"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        public override void Deserialize(string content)
        {
            content.RequireNotNull("content");
            throw new NotImplementedException();
        }
    }
}
