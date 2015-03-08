﻿using System;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment
{
    /// <summary>
    /// Represents the EXT-X-MAP tag.
    /// </summary>
    /// <remarks>
    /// The EXT-X-MAP tag specifies how to obtain the Media Initialization
    /// Section (Section 3) required to parse the applicable Media Segments.
    /// It applies to every Media Segment that appears after it in the
    /// Playlist until the next EXT-X-MAP tag or until the end of the
    /// playlist.
    /// 
    /// Its format is:
    /// #EXT-X-MAP:&lt;attribute-list&gt;
    /// 
    /// An EXT-X-MAP tag SHOULD be supplied for Media Segments in Playlists
    /// with the EXT-X-I-FRAMES-ONLY tag when the first Media Segment (i.e.,
    /// I-frame) in the Playlist (or the first segment following an EXT-
    /// X-DISCONTINUITY tag) does not immediately follow the Media
    /// Initialization Section at the beginning of its resource.
    ///
    /// Use of the EXT-X-MAP tag in a Media Playlist that does not contain
    /// the EXT-X-I-FRAMES-ONLY tag REQUIRES a compatibility version number
    /// of 6 or greater; otherwise, a compatibility version number of 5 or
    /// greater is REQUIRED.
    /// 
    /// </remarks>
    public class Map : BaseTag
    {
        /// <summary>
        /// Gets the name of the tag, for example EXT-X-MEDIA.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-MAP"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXMap; }
        }

        /// <summary>
        /// The value is a quoted-string containing a URI that identifies a
        /// resource that contains the Media Initialization Section. This
        /// attribute is REQUIRED.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// The value is a quoted-string specifying a byte range into the
        /// resource identified by the URI attribute. This range SHOULD contain
        /// only the Media Initialization Section. The format of the byte range
        /// is described in Section 4.3.2.2. This attribute is OPTIONAL; if it
        /// is not present, the byte range is the entire resource indicated by
        /// the URI.
        /// </summary>
        public ByteRange ByteRange { get; private set; }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public override void Deserialize(string content, int version)
        {
            content.RequireNotEmpty("content");
            try
            {
                const int requiredVersion = 5; // in some case 6
                if (version < requiredVersion)
                {
                    throw new IncompatibleVersionException(this, version, requiredVersion);
                }

                ParseUri(content);
                ParseByteRange(content, version);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse EXT-X-MAP tag.", ex);
            }
        }

        private void ParseUri(string content)
        {
            const string name = "URI";
            var value = ValueParser.ParseQuotedString(name, content, true);
            Uri = new Uri(value);
        }

        private void ParseByteRange(string content, int version)
        {
            const string name = "BYTERANGE";
            var value = ValueParser.ParseQuotedString(name, content, false);
            ByteRange = new ByteRange();
            if (value != string.Empty)
            {
                ByteRange.Deserialize(value, version);
            }
        }
    }
}