using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using SeeSharpLiveStreaming.Playlist.Tags;
using SeeSharpLiveStreaming.Playlist.Tags.BasicTags;
using SeeSharpLiveStreaming.Utils;

namespace SeeSharpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents a base class for playlists.
    /// </summary>
    public abstract class PlaylistBase
    {

        /// <summary>
        /// The list of tags.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected readonly List<BaseTag> _tags = new List<BaseTag>();

        /// <summary>
        /// Gets the tags.
        /// </summary>
        public IReadOnlyCollection<BaseTag> Tags
        {
            get { return new ReadOnlyCollection<BaseTag>(_tags); }
        }

        /// <summary>
        /// Gets the compatibility level version number tag.
        /// </summary>
        protected int Version { get; private set; }

        /// <summary>
        /// Creates a specific playlist depending on content of the <paramref name="playlist" />.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <returns>
        /// The <see cref="PlaylistBase" /> instance.
        /// </returns>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        internal static PlaylistBase Create(IList<PlaylistLine> playlist)
        {
            playlist.RequireNotEmpty("playlist");

            var firstTag = GetFirstNonCommonTag(playlist);
            return CreatePlaylistByTag(firstTag, playlist);
        }

        /// <summary>
        /// Gets the first non common tag.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <returns></returns>
        private static string GetFirstNonCommonTag(IEnumerable<PlaylistLine> playlist)
        {
            return playlist.FirstOrDefault(x => Tag.IsMasterTag(x.Tag) || 
                                                Tag.IsMediaPlaylistTag(x.Tag)).Tag ?? string.Empty;
        }

        /// <summary>
        /// Creates the playlist by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="playlist">The playlist.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the second tag is invalid. The playlist cannot be parsed.
        /// </exception>
        private static PlaylistBase CreatePlaylistByTag(string tag, IList<PlaylistLine> playlist)
        {
            if (Tag.IsMasterTag(tag))
            {
                return new MasterPlaylist(playlist);
            }
            if (Tag.IsMediaPlaylistTag(tag))
            {
                return new MediaPlaylist(playlist);
            }

            throw new ArgumentException("Invalid second tag. Cannot create a playlist instance.");
        }

        /// <summary>
        /// When overridden in a derived class deserializes an instance of <see cref="PlaylistBase"/>.
        /// </summary>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        protected abstract void Parse(IList<PlaylistLine> playlist);

        /// <summary>
        /// Processes the playlist line.
        /// </summary>
        /// <param name="line">The line.</param>
        protected void CreateLine(PlaylistLine line)
        {
            var tag = BaseTag.Create(line, Version);
            if (tag.TagType != TagType.ExtXVersion)
            {
                _tags.Add(tag);
            }
            else
            {
                var versionTag = (ExtXVersion) tag;
                Version = versionTag.Version;
            }
        }
    }
}
