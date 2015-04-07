using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Utils;
using Version = SeeSharpHttpLiveStreaming.Playlist.Tags.BasicTags.Version;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents a base class for playlists.
    /// </summary>
    internal abstract class PlaylistBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistBase"/> class.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        protected PlaylistBase(Uri baseUri)
        {
            baseUri.RequireNotNull("baseUri");
            BaseUri = baseUri;
            Version = Playlist.Tags.BasicTags.Version.InitialVersionNumber;
        }

        /// <summary>
        /// Gets the base URI.
        /// </summary>
        protected Uri BaseUri { get; private set; }

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
        /// Gets the compatibility level version number.
        /// </summary>
        /// <remarks>
        /// Defaults to one (1) if that tag is not included when deserializing from server.
        /// Server can set the the version accoring to supported level (TBD).
        /// </remarks>
        public int Version { get; private set; }

        /// <summary>
        /// Processes the playlist line.
        /// </summary>
        /// <param name="line">The playlist line.</param>
        /// <returns>
        /// The tag created from the line.
        /// </returns>
        /// <remarks>
        /// If the tag is not a <see cref="Version"/> tag, it is added to the list 
        /// of tags.
        /// </remarks>
        protected virtual BaseTag ProcessSingleLine(PlaylistLine line)
        {
            var tag = TagFactory.Create(line, BaseUri, Version);
            if (tag.TagType != TagType.ExtXVersion)
            {
                _tags.Add(tag);
            }
            else
            {
                var versionTag = (Version) tag;
                Version = versionTag.VersionNumber;
            }
            return tag;
        }
    }
}
