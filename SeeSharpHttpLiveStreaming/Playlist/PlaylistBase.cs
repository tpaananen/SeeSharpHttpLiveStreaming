﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.BasicTags;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist
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
        internal static PlaylistBase Create(IReadOnlyCollection<PlaylistLine> playlist)
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
            return playlist.FirstOrDefault(x => !Tag.IsBasicTag(x.Tag)).Tag ?? string.Empty;
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
        private static PlaylistBase CreatePlaylistByTag(string tag, IReadOnlyCollection<PlaylistLine> playlist)
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
        /// Processes the playlist line.
        /// </summary>
        /// <param name="line">The line.</param>
        protected void ProcessSingleLine(PlaylistLine line)
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