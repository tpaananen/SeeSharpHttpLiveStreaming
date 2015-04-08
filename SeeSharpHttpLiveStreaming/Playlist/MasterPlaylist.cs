﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Master;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents a master playlist.
    /// </summary>
    internal sealed class MasterPlaylist : PlaylistBase
    {
        /// <summary>
        /// Specifies the alternative media selection groups.
        /// </summary>
        private readonly List<RenditionGroup> _renditionGroups = new List<RenditionGroup>();

        /// <summary>
        /// Specifies the default variant streams available.
        /// </summary>
        private readonly List<StreamInf> _variantStreams = new List<StreamInf>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPlaylist" /> class.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <param name="baseUri">The base URI.</param>
        public MasterPlaylist(IReadOnlyCollection<PlaylistLine> playlist, Uri baseUri)
            : base(baseUri)
        {
            Parse(playlist);
        }

        /// <summary>
        /// Gets the variant streams.
        /// </summary>
        public IReadOnlyCollection<StreamInf> VariantStreams
        {
            get
            {
                return new ReadOnlyCollection<StreamInf>(_variantStreams);
            }
        }

        /// <summary>
        /// Gets the rendition groups.
        /// </summary>
        public IReadOnlyCollection<RenditionGroup> RenditionGroups
        {
            get
            {
                return new ReadOnlyCollection<RenditionGroup>(_renditionGroups);
            }
        }

        /// <summary>
        /// Deserializes a <see cref="MasterPlaylist"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        private void Parse(IReadOnlyCollection<PlaylistLine> content)
        {
            content.RequireNotEmpty("content");
            ReadTags(content);
            CreateVariantStreams();
            CreateRenditionGroups();
        }

        private void CreateVariantStreams()
        {
            _variantStreams.AddRange(_tags.OfType<StreamInf>());
        }

        private void CreateRenditionGroups()
        {
            var groupIds = _tags.OfType<ExtMedia>().Select(x => new { x.GroupId, x.Type }).Distinct();
            foreach (var groupDetail in groupIds)
            {
                var renditionGroup = new RenditionGroup(groupDetail.GroupId, groupDetail.Type, _tags);
                _renditionGroups.Add(renditionGroup);
            }

            ValidateGroups();
        }

        private void ValidateGroups()
        {
            // Each member of the media group must be replicated 
            // in each media group for that media type.

            // Validate that each group of ExtMedia tags have  attributes 
            // in each group having the same TYPE (except URI and GROUP-ID)
            var grouping = _renditionGroups.GroupBy(d => d.Type);
            foreach (var group in grouping)
            {
                if (group.Count() > 1)
                {
                    ValidateGroup(group);
                }
            }
        }

        /// <summary>
        /// Validates the groups having the same type attribute.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <exception cref="SerializationException">
        /// Thrown when no matching EXT-X-MEDIA tag from the list of renditions.
        /// </exception>
        private static void ValidateGroup(IEnumerable<RenditionGroup> @group)
        {
            // go throw medias, there must be equally many (-1 for self) medias with the same attributes as there is groups
            foreach (var media in @group.SelectMany(x => x.ExtMedias))
            {
                if (!media.EqualityCheck(@group.SelectMany(x => x.ExtMedias).Where(d => d != media)))
                {
                    throw new SerializationException("Could not find matching EXT-X-MEDIA tag from the list of renditions.");
                }
            }
        }
        // throw new SerializationException("Could not find matching EXT-X-MEDIA tag from the list of renditions.");


        private void ReadTags(IEnumerable<PlaylistLine> content)
        {
            foreach (var line in content)
            {
                if (!Tag.IsMasterTag(line.Tag) && !Tag.IsBasicTag(line.Tag))
                {
                    throw new SerializationException("The tag " + line.Tag + " is not a master playlist tag. Master playlist tag must not contain other than master playlist tags or basic tags.");
                }
                ProcessSingleLine(line);
            }
        }
    }
}
