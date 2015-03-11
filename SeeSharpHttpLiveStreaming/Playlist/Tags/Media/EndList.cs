using System;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media
{
    /// <summary>
    /// Represents the EXT-X-ENDLIST tag.
    /// </summary>
    /// <remarks>
    /// The EXT-X-ENDLIST tag indicates that no more Media Segments will be
    /// added to the Media Playlist file.  It MAY occur anywhere in the Media
    /// Playlist file.  Its format is:
    ///
    /// #EXT-X-ENDLIST
    /// </remarks>
    public class EndList : BaseTag
    {
        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-ENDLIST"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXEndList; }
        }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public override void Deserialize(string content, int version)
        {
            if (!string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("The EXT-X-ENDLIST does not have attributes but attributes were provided.");
            }
        }
    }
}
