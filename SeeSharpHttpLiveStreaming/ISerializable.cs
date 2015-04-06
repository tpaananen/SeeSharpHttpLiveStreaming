using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming
{
    /// <summary>
    /// Classes implementing this interface will be able to deserialize themselves from the line(s) of strings and 
    /// also they will be able to serialize themselves into line(s) of strings.
    /// </summary>
    internal interface ISerializable
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
