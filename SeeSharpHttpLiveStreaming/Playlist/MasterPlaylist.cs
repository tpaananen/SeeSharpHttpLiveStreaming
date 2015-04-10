using System;
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
        private readonly List<VariantStream> _variantStreams = new List<VariantStream>(); 

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
        public IReadOnlyCollection<VariantStream> VariantStreams
        {
            get
            {
                return new ReadOnlyCollection<VariantStream>(_variantStreams);
            }
        }

        private void Parse(IReadOnlyCollection<PlaylistLine> content)
        {
            content.RequireNotEmpty("content");
            var tags = ReadTags(content);
            var streamInfs = CreateVariantStreams(tags);
            var renditionGroups = CreateRenditionGroups(tags);
            LinkVariantStreamAndRenditionGroups(streamInfs, renditionGroups);
        }

        private void LinkVariantStreamAndRenditionGroups(IEnumerable<StreamInf> streamInfs, IReadOnlyCollection<RenditionGroup> renditionGroups)
        {
            foreach (var streamInf in streamInfs)
            {
                var video = renditionGroups.FirstOrDefault(x => x.GroupId == streamInf.Video && x.Type == MediaTypes.Video);
                var audio = renditionGroups.FirstOrDefault(x => x.GroupId == streamInf.Audio && x.Type == MediaTypes.Audio);
                var subtitles = renditionGroups.FirstOrDefault(x => x.GroupId == streamInf.Subtitles && x.Type == MediaTypes.Subtitles);
                var closedCaptions = renditionGroups.FirstOrDefault(x => x.GroupId == streamInf.ClosedCaptions && x.Type == MediaTypes.ClosedCaptions);
                _variantStreams.Add(new VariantStream(streamInf, video, audio, subtitles, closedCaptions));
            }
        }

        private static IEnumerable<StreamInf> CreateVariantStreams(IEnumerable<BaseTag> tags)
        {
            return tags.OfType<StreamInf>().ToList();
        }

        private static IReadOnlyCollection<RenditionGroup> CreateRenditionGroups(IReadOnlyCollection<BaseTag> tags)
        {
            var groupIds = tags.OfType<ExtMedia>().Select(x => new { x.GroupId, x.Type }).Distinct();
            var renditionGroups = groupIds.Select(groupDetail => new RenditionGroup(groupDetail.GroupId, groupDetail.Type, tags)).ToList();
            ValidateGroups(renditionGroups);
            return renditionGroups;
        }

        private static void ValidateGroups(IEnumerable<RenditionGroup> renditionGroups)
        {
            // Each member of the media group must be replicated 
            // in each media group for that media type.
            var grouping = renditionGroups.GroupBy(d => d.Type);
            foreach (var group in grouping)
            {
                var list = group.ToList();
                if (list.Count > 1)
                {
                    ValidateGroup(list);
                }
            }
        }

        /// <summary>
        /// Validates the media groups having the same type attribute.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <exception cref="SerializationException">
        /// Thrown when no matching EXT-X-MEDIA tag from the list of renditions.
        /// </exception>
        private static void ValidateGroup(ICollection<RenditionGroup> group)
        {
            foreach (var media in group.SelectMany(x => x.ExtMedias))
            {
                // check all other groups than the one the media exists and find equal attributes from each other group
                if (!group.Where(d => !d.ExtMedias.Contains(media)).All(d => media.EqualityCheck(d.ExtMedias)))
                {
                    throw new SerializationException("Could not find matching EXT-X-MEDIA tag from other media groups with TYPE " + media.Type + ", GROUP-ID " + media.GroupId + ", NAME " + media.Name);
                }
            }
        }

        /// <summary>
        /// Reads the tags.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        /// <exception cref="SerializationException">
        /// The tag is not a master playlist tag.
        /// </exception>
        private List<BaseTag> ReadTags(IEnumerable<PlaylistLine> content)
        {
            var collection = new List<BaseTag>();
            foreach (var line in content)
            {
                if (!Tag.IsMasterTag(line.Tag) && !Tag.IsBasicTag(line.Tag))
                {
                    throw new SerializationException("The tag " + line.Tag + " is not a master playlist tag. Master playlist must not contain other than master or basic tags.");
                }
                collection.Add(ProcessSingleLine(line));
            }
            return collection;
        }
    }
}
