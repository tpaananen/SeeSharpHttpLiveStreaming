using System;
using SeeSharpLiveStreaming.Utils;

namespace SeeSharpLiveStreaming.Playlist.Tags.Master
{
    public class StreamInf : ISerializable
    {

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">
        /// Thrown when the serialization fails.
        /// </exception>
        public void Deserialize(string content)
        {
            content.RequireNotNull("content");
            throw new NotImplementedException();
        }
    }
}
