using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Master
{
    /// <summary>
    /// Represents rendition group where each EXT-X-MEDIA tags have the same GROUP-ID attribute value..
    /// </summary>
    /// <remarks>
    /// A set of EXT-X-MEDIA tags with the same GROUP-ID value and the same
    /// TYPE value forms a group of Renditions.Each member of the group
    /// MUST be an alternative rendition of the same content; otherwise
    /// playback errors can occur.
    /// All EXT-X-MEDIA tags in a Playlist MUST meet the following
    /// constraints:
    /// o All EXT-X-MEDIA tags in the same group MUST have different NAME
    ///   attributes.
    ///
    /// o  A group MUST NOT have more than one member with a DEFAULT
    ///    attribute of YES.
    ///
    /// o All members of a group whose AUTOSELECT attribute has a value of
    ///   YES MUST have LANGUAGE[RFC5646] attributes with unique values.
    ///
    /// TODO: validate
    /// A Playlist MAY contain multiple groups of the same TYPE in order to
    /// provide multiple encodings of that media type. If it does so, each
    /// group of the same TYPE MUST have the same set of members, and each
    /// corresponding member MUST have identical attributes with the
    /// exception of the URI attribute.
    ///
    /// Each member in a group of Renditions MAY have a different sample
    /// format.  However, any EXT-X-STREAM-INF (Section 4.3.4.2) tag or EXT-
    /// X-I-FRAME-STREAM-INF(Section 4.3.4.3) tag which references that
    /// group MUST have a CODECS attribute that lists every sample format
    /// present in any Rendition in the group, or client playback failures
    /// can occur.
    /// </remarks>
    public sealed class RenditionGroup
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RenditionGroup" /> class.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="tags">The tags.</param>
        /// <exception cref="SerializationException">Thrown if the EXT-X-MEDIA tags in a Playlist do not meet the specified requirements.
        /// See remarks of the <see cref="RenditionGroup" />.</exception>
        public RenditionGroup(string groupId, string type, IReadOnlyCollection<BaseTag> tags)
        {
            groupId.RequireNotEmpty("groupId");
            type.RequireNotEmpty("type");
            tags.RequireNotEmpty("tags");
            

            GroupId = groupId;
            Type = type;
            ExtMedias = GetMediaTags(groupId, tags);

            ValidateMediaTags(ExtMedias);
        }

        private static IReadOnlyCollection<ExtMedia> GetMediaTags(string groupId, IEnumerable<BaseTag> tags)
        {
            var list = new List<ExtMedia>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var tag in tags)
            {
                var media = tag as ExtMedia;
                if (media == null)
                {
                    continue;
                }

                if (media.GroupId != groupId)
                {
                    continue;
                }
                list.Add(media);
            }
            return list.AsReadOnly();
        }

        /// <summary>
        /// Gets the group identifier.
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        public string GroupId { get; private set; }

        /// <summary>
        /// Gets the type of the group.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Gets the EXT-X-MEDIA tags that belong to the group.
        /// </summary>
        public IReadOnlyCollection<ExtMedia> ExtMedias { get; private set; }

        /// <summary>
        /// Validates the media tags.
        /// </summary>
        /// <param name="extMedias">The ext medias.</param>
        /// <remarks>
        /// All EXT-X-MEDIA tags in a Playlist MUST meet the following constraints:
        /// 
        /// o All EXT-X-MEDIA tags in the same group MUST have different NAME
        ///   attributes.
        ///
        /// o A group MUST NOT have more than one member with a DEFAULT
        ///   attribute of YES.
        ///
        /// o All members of a group whose AUTOSELECT attribute has a value of
        ///   YES MUST have LANGUAGE [RFC5646] attributes with unique values.
        ///
        /// </remarks>
        private static void ValidateMediaTags(IReadOnlyCollection<ExtMedia> extMedias)
        {
            extMedias.RequireNotEmpty("extMedias");

            // Check that names are unique
            if (extMedias.Select(x => x.Name).Distinct().Count() != extMedias.Count)
            {
                throw new SerializationException("All EXT-X-MEDIA tags in the same rendition group MUST have different NAME attributes.");
            }

            // Only one default should exist
            if (extMedias.Count(x => x.Default) > 1)
            {
                throw new SerializationException("A rendition group MUST NOT have more than one member with a DEFAULT attribute of YES.");
            }

            // some perf hit but the list is typically very short
            if (extMedias.Where(x => x.AutoSelect).Select(x => x.Language).Count() !=
                extMedias.Where(x => x.AutoSelect).Select(x => x.Language).Distinct().Count())
            { 
                throw new SerializationException("All members of a rendition group whose AUTOSELECT attribute has a value of YES MUST have LANGUAGE [RFC5646] attributes with unique values.");
            }
        }

    }
}
