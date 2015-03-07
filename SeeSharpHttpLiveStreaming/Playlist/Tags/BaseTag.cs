using System;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Represents the base tag for each tag described in HTTP Live Streaming specification.
    /// </summary>
    public abstract class BaseTag : ISerializable
    {
        /// <summary>
        /// Gets the name of the tag, for example EXT-X-MEDIA.
        /// </summary>
        public abstract string TagName { get; }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public abstract TagType TagType { get; }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public abstract void Deserialize(string content, int version);

        /// <summary>
        /// Creates the specified content.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">The start tag cannot be created.</exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">Thrown if parsing of the content fails.</exception>
        internal static BaseTag Create(PlaylistLine line, int version)
        {
            if (line.Tag == Tag.StartLine)
            {
                throw new InvalidOperationException("The start tag cannot be created.");
            }

            var tagObject = TagFactory.Create(line.Tag);
            tagObject.Deserialize(line.GetParameters(), version);
            return tagObject;
        }
    }
}
