using System;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media
{
    /// <summary>
    /// Represents the EXT-X-PLAYLIST-TYPE tag.
    /// </summary>
    /// <remarks>
    /// The EXT-X-PLAYLIST-TYPE tag provides mutability information about the
    /// Media Playlist file. It applies to the entire Media Playlist file.
    /// It is OPTIONAL. Its format is:
    ///
    /// #EXT-X-PLAYLIST-TYPE:&lt;EVENT|VOD&gt;
    /// 
    /// If the EXT-X-PLAYLIST-TYPE value is EVENT, Media Segments can only be
    /// added to the end of the Media Playlist.  If the EXT-X-PLAYLIST-TYPE
    /// value is VOD, the Media Playlist cannot change.
    /// </remarks>
    public class PlaylistType : BaseTag
    {
        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-PLAYLIST-TYPE"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXPlaylistType; }
        }

        /// <summary>
        /// Gets the type of the playlist.
        /// </summary>
        public string PlaylistTypeValue { get; private set; }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public override void Deserialize(string content, int version)
        {
            content.RequireNotEmpty("content");
            try
            {
                if (!Playlist.PlaylistType.IsValid(content))
                {
                    throw new FormatException("Invalid value " + content + " for EXT-X-PLAYLIST-TYPE tag.");
                }
                PlaylistTypeValue = content;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse EXT-X-PLAYLIST-TYPE tag.", ex);
            }
        }
    }
}
