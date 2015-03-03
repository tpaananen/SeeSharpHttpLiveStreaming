using System.Runtime.Serialization;

namespace SeeSharpLiveStreaming
{
    /// <summary>
    /// Classes implementing the interface will be able to deserialize itself from the lines of string.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        void Deserialize(string content, int version);

    }
}
