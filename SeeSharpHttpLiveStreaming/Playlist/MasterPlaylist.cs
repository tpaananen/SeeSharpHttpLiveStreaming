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
    public sealed class MasterPlaylist : PlaylistBase
    {
        private readonly List<RenditionGroup> _renditionGroups = new List<RenditionGroup>(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPlaylist"/> class.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        public MasterPlaylist(IReadOnlyCollection<PlaylistLine> playlist)
        {
            Parse(playlist);
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
            try
            {
                ReadTags(content);
                CreateRenditionGroups();
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("Failed to deserialize {0} class.", typeof(MasterPlaylist).Name), ex);
            }
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

        private static void ValidateGroups()
        {
        }

        private void ReadTags(IEnumerable<PlaylistLine> content)
        {
            foreach (var line in content)
            {
                if (Tag.IsMediaPlaylistTag(line.Tag) || Tag.IsMediaSegmentTag(line.Tag))
                {
                    throw new SerializationException("The tag " + line.Tag + " is not a master playlist tag. Master playlist tag must not contain other than master playlist tags or basic tags.");
                }
                ProcessSingleLine(line);
            }
        }
    }
}
