using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Contains static set of valid tags in the playlist file.
    /// </summary>
    /// <remarks>
    /// Specification: https://tools.ietf.org/html/draft-pantos-http-live-streaming-14
    /// </remarks>
    internal static class Tag
    {

        /// <summary>
        /// Specifies the tag end marker.
        /// </summary>
        internal const string TagEndMarker = ":";

        /// <summary>
        /// Each playlist should start with this line.
        /// </summary>
        internal const string StartLine = "#EXTM3U";

        /// <summary>
        /// The basic tags
        /// </summary>
        /// <remarks>
        /// 4.3.1. Basic Tags > https://tools.ietf.org/html/draft-pantos-http-live-streaming-14#section-4.3.1
        /// </remarks>
        internal static readonly IReadOnlyCollection<string> BasicTags = new []
            {
                StartLine,
                "#EXT-X-VERSION"
            };

        /// <summary>
        /// The media segment tags
        /// </summary>
        /// <remarks>
        /// See: 4.3.2. Media Segment Tags > https://tools.ietf.org/html/draft-pantos-http-live-streaming-14#section-4.3.2
        /// </remarks>
        internal static readonly IReadOnlyCollection<string> MediaSegmentTags = 
            new[]
            {
                "#EXTINF",
                "#EXT-X-BYTERANGE",
                "#EXT-X-DISCONTINUITY",
                "#EXT-X-KEY",
                "#EXT-X-MAP",
                "#EXT-X-PROGRAM-DATE-TIME"
            };

        /// <summary>
        /// The media playlist tags
        /// </summary>
        /// <remarks>
        /// 4.3.3 Media Playlist Tags > https://tools.ietf.org/html/draft-pantos-http-live-streaming-14#section-4.3.3
        /// </remarks>
        internal static readonly IReadOnlyCollection<string> MediaPlaylistTags = new ReadOnlyCollection<string>(
            new[]
            {
                "#EXT-X-TARGETDURATION",
                "#EXT-X-MEDIA-SEQUENCE",
                "#EXT-X-DISCONTINUITY-SEQUENCE",
                "#EXT-X-ENDLIST",
                "#EXT-X-PLAYLIST-TYPE",
                "#EXT-X-I-FRAMES-ONLY"
            });

        /// <summary>
        /// The master playlist tags
        /// </summary>
        /// <remarks>
        /// 4.3.4. Master Playlist Tags > https://tools.ietf.org/html/draft-pantos-http-live-streaming-14#section-4.3.4
        /// </remarks>
        internal static readonly IReadOnlyCollection<string> MasterPlaylistTags = new[]
            {
                "#EXT-X-MEDIA",
                "#EXT-X-STREAM-INF",
                "#EXT-X-I-FRAME-STREAM-INF",
                "#EXT-X-SESSION-DATA"
            };

        /// <summary>
        /// The master or media playlist tags
        /// </summary>
        /// <remarks>
        /// 4.3.5. Media or Master Playlist Tags > https://tools.ietf.org/html/draft-pantos-http-live-streaming-14#section-4.3.5
        /// </remarks>
        internal static readonly IReadOnlyCollection<string> MasterOrMediaPlaylistTags = new[]
            {
                "#EXT-X-INDEPENDENT-SEGMENTS",
                "#EXT-X-START"
            };

        /// <summary>
        /// The following tags have no attributes specified and therefore should not have the end marker prefixed.
        /// </summary>
        internal static readonly IReadOnlyCollection<string> HasNoAttributes = new[]
            {
                "#EXT-X-DISCONTINUITY",
                "#EXT-X-I-FRAMES-ONLY",
                "#EXT-X-ENDLIST",
                "#EXT-X-INDEPENDENT-SEGMENTS"
            };

        /// <summary>
        /// Determines whether the tag is a basic tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// <b>True</b> if the <paramref name="tag"/> is a valid basic tag; otserwise, <b>false</b>.
        /// </returns>
        internal static bool IsBasicTag(string tag)
        {
            return BasicTags.Contains(tag);
        }

        /// <summary>
        /// Determines whether the tag is a media playlist tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// <b>True</b> if the <paramref name="tag"/> is a valid media playlist tag; otserwise, <b>false</b>.
        /// </returns>
        internal static bool IsMediaPlaylistTag(string tag)
        {
            bool isValid = MediaPlaylistTags.Contains(tag);
            if (!isValid)
            {
                isValid = MasterOrMediaPlaylistTags.Contains(tag);
            }
            return isValid;
        }

        /// <summary>
        /// Determines whether the tag is a media segment playlist tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// <b>True</b> if the <paramref name="tag"/> is a valid media playlist tag; otserwise, <b>false</b>.
        /// </returns>
        internal static bool IsMediaSegmentTag(string tag)
        {
            return MediaSegmentTags.Contains(tag);
        }

        /// <summary>
        /// Determines whether the tag is a master playlist tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// <b>True</b> if the <paramref name="tag"/> is a valid master playlist tag; otserwise, <b>false</b>.
        /// </returns>
        internal static bool IsMasterTag(string tag)
        {
            bool isValid = MasterPlaylistTags.Contains(tag);
            if (!isValid)
            {
                isValid = MasterOrMediaPlaylistTags.Contains(tag);
            }
            return isValid;
        }

        /// <summary>
        /// Determines whether the specified tag is a valid tag.
        /// </summary>
        /// <param name="tag">The tag to be validated.</param>
        /// <returns></returns>
        public static bool IsValid(string tag)
        {
            return IsMasterTag(tag) ||
                   IsMediaPlaylistTag(tag) ||
                   IsBasicTag(tag) ||
                   IsMediaSegmentTag(tag);
        }

        /// <summary>
        /// Determines whether the tag is followed by URI.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// True if the tag is followed by the URI; otherwise, false.
        /// </returns>
        public static bool IsFollowedByUri(string tag)
        {
            return tag == "#EXT-X-STREAM-INF" || 
                   IsMediaSegmentTag(tag);
        }

        /// <summary>
        /// Determines whether the tag has attributes.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static bool HasAttributes(string tag)
        {
            return !HasNoAttributes.Contains(tag);
        }
    }
}
