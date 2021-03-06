﻿using System;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media
{
    /// <summary>
    /// Represents the EXT-X-ENDLIST tag.
    /// </summary>
    /// <remarks>
    /// The EXT-X-ENDLIST tag indicates that no more Media Segments will be
    /// added to the Media Playlist file. It MAY occur anywhere in the Media
    /// Playlist file.  Its format is:
    ///
    /// #EXT-X-ENDLIST
    /// </remarks>
    internal class EndList : MediaBaseTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndList"/> class.
        /// </summary>
        // ReSharper disable once EmptyConstructor
        internal EndList()
        {
        }

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
        /// Gets a value indicating whether this tag has attributes.
        /// </summary>
        public override bool HasAttributes
        {
            get { return false; }
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

        /// <summary>
        /// Serializes the attributes. This tag has no attributes so it does nothing.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void SerializeAttributes(IPlaylistWriter writer)
        {
        }

        /// <summary>
        /// Adds the tag properties to the playlist.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        internal override void AddToPlaylist(MediaPlaylist playlist)
        {
            playlist.IsFinal = true;
        }
    }
}
