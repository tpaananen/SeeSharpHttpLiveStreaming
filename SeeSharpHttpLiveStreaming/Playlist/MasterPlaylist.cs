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
        private readonly List<VariantStream> _variantStreams = new List<VariantStream>();
        private readonly List<IntraFrameStreamInf> _intraFrameLists = new List<IntraFrameStreamInf>(); 

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

        /// <summary>
        /// Gets the pointers to intra frame playlists.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public IReadOnlyCollection<IntraFrameStreamInf> IntraFrames
        {
            get
            {
                return new ReadOnlyCollection<IntraFrameStreamInf>(_intraFrameLists);
            }
        }

        private void Parse(IReadOnlyCollection<PlaylistLine> content)
        {
            content.RequireNotEmpty("content");
            var tags = ReadTags(content);
            var renditionGroups = CreateRenditionGroups(tags);
            LinkVariantStreamAndRenditionGroups(tags.OfType<StreamInf>(), renditionGroups);
        }

        private void LinkVariantStreamAndRenditionGroups(IEnumerable<StreamInf> streamInfs, IReadOnlyCollection<RenditionGroup> renditionGroups)
        {
            bool? closedCaptionsStatus = null;
            foreach (var streamInf in streamInfs)
            {
                var hasClosedCaptions = ValidateClosedCaptions(streamInf, ref closedCaptionsStatus);
                var video = streamInf.Video != string.Empty ? GetRenditionGroup(renditionGroups, streamInf, x => x.Video, MediaTypes.Video) : null;
                var audio = streamInf.Audio != string.Empty ? GetRenditionGroup(renditionGroups, streamInf, x => x.Audio, MediaTypes.Audio) : null;
                var subtitles = streamInf.Subtitles != string.Empty ? GetRenditionGroup(renditionGroups, streamInf, x => x.Subtitles, MediaTypes.Subtitles) : null;
                var closedCaptions = hasClosedCaptions ? GetRenditionGroup(renditionGroups, streamInf, x => x.ClosedCaptions, MediaTypes.ClosedCaptions) : null;
                _variantStreams.Add(new VariantStream(streamInf, video, audio, subtitles, closedCaptions));
            }
        }

        private static RenditionGroup GetRenditionGroup(IEnumerable<RenditionGroup> renditionGroups, StreamInf streamInf, Func<StreamInf, string> selector, string mediaType)
        {
            var groupId = selector(streamInf);
            var group = renditionGroups.FirstOrDefault(x => x.GroupId == groupId && x.Type == mediaType);
            if (group == null)
            {
                throw new SerializationException(string.Format("Could not find matching alternative media group from the playlist for TYPE {0} and GROUP-ID {1}.", mediaType, groupId));
            }
            return group;
        }

        private static bool ValidateClosedCaptions(StreamInf streamInf, ref bool? closedCaptionsStatus)
        {
            bool currentClosedCaptions = streamInf.ClosedCaptions != StreamInf.ClosedCaptionsNone;
            if (closedCaptionsStatus == null)
            {
                closedCaptionsStatus = currentClosedCaptions;
                return currentClosedCaptions;
            }

            if (currentClosedCaptions != closedCaptionsStatus)
            {
                throw new SerializationException(
                    "Each EXT-X-STREAM-INF must have CLOSED-CAPTIONS as NONE if one has value NONE.");
            }
            return currentClosedCaptions;
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
                ValidateGroup(group);
            }
        }

        /// <summary>
        /// Validates the media groups having the same type attribute.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <exception cref="SerializationException">
        /// Thrown when no matching EXT-X-MEDIA tag from the list of renditions.
        /// </exception>
        private static void ValidateGroup(IEnumerable<RenditionGroup> group)
        {
            // ReSharper disable PossibleMultipleEnumeration
            var media = group.SelectMany(x => x.ExtMedias)
                             .FirstOrDefault(m => !group.Where(d => !d.ExtMedias.Contains(m))
                                                        .All(d => m.EqualityCheck(d.ExtMedias)));
            // ReSharper restore PossibleMultipleEnumeration
            if (media != null)
            {
                throw new SerializationException(
                    "Could not find matching EXT-X-MEDIA tag from other media groups with TYPE " + media.Type +
                    ", GROUP-ID " + media.GroupId + ", NAME " + media.Name);
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
                if (!Tag.IsMasterTag(line.Tag) && !Tag.IsBasicTag(line.Tag) && !Tag.IsMasterOrMediaTag(line.Tag))
                {
                    throw new SerializationException(
                        "The tag " + line.Tag + " is not a master playlist tag. " + 
                        "Master playlist must not contain other than master, master or media, or basic tags.");
                }
                collection.Add(ProcessSingleLine(line));
            }
            return collection;
        }

        /// <summary>
        /// Processes the playlist line.
        /// </summary>
        /// <param name="line">The playlist line.</param>
        protected override BaseTag ProcessSingleLine(PlaylistLine line)
        {
            var tag = base.ProcessSingleLine(line);
            var masterTag = tag as MasterBaseTag;
            if (masterTag != null)
            {
                masterTag.AddToPlaylist(this);
            }

            return tag;
        }

        /// <summary>
        /// Adds the frame to the I-FRAME list.
        /// </summary>
        /// <param name="frame">The frame.</param>
        internal void AddIFrame(IntraFrameStreamInf frame)
        {
            _intraFrameLists.Add(frame);
        }
    }
}
