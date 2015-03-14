using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming
{
    /// <summary>
    /// Classes implementing the interface will be able to deserialize itself from the lines of string.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="SerializationException">Thrown when the deserialization fails.</exception>
        void Deserialize(string content, int version);

        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        void Serialize(IPlaylistWriter writer);
    }
}
