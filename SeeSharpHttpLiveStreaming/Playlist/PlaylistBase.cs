using System;
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
            Version = Tags.BasicTags.Version.InitialVersionNumber;
        }

        /// <summary>
        /// Gets the base URI.
        /// </summary>
        protected Uri BaseUri { get; private set; }

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
        protected virtual BaseTag ProcessSingleLine(PlaylistLine line)
        {
            var tag = TagFactory.Create(line, BaseUri, Version);
            if (tag.TagType == TagType.ExtXVersion)
            {
                var versionTag = (Version) tag;
                Version = versionTag.VersionNumber;
            }
            return tag;
        }
    }
}
