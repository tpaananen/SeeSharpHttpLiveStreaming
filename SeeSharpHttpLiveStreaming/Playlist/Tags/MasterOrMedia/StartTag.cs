﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.MasterOrMedia
{
    /// <summary>
    /// The EXT-X-START tag indicates a preferred point at which to start
    /// playing a Playlist.  By default, clients SHOULD start playback at
    /// this point when beginning a playback session.  This tag is OPTIONAL.
    /// 
    /// Its format is:
    /// 
    /// #EXT-X-START:&lt;attribute list&gt;
    /// </summary>
    public class StartTag : BaseTag
    {
        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-START"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXStart; }
        }

        /// <summary>
        /// The value of TIME-OFFSET is a signed-decimal-floating-point number of
        /// seconds.  A positive number indicates a time offset from the
        /// beginning of the Playlist.  A negative number indicates a negative
        /// time offset from the end of the last Media Segment in the Playlist.
        /// This attribute is REQUIRED.
        ///
        /// The absolute value of TIME-OFFSET SHOULD NOT be larger than the
        /// Playlist duration.  If the absolute value of TIME-OFFSET exceeds the
        /// duration of the Playlist, it indicates either the end of the Playlist
        /// (if positive) or the beginning of the Playlist (if negative).
        ///
        /// If the Playlist does not contain the EXT-X-ENDLIST tag, the TIME-
        /// OFFSET SHOULD NOT be within three target durations of the end of the
        /// Playlist file.
        /// </summary>
        public decimal TimeOffset { get; private set; }

        /// <summary>
        /// The value is an enumerated-string; valid strings are YES and NO.  If
        /// the value is YES, clients SHOULD start playback at the Media Segment
        /// containing the TIME-OFFSET, but SHOULD NOT render media samples in
        /// that segment whose presentation times are prior to the TIME-OFFSET.
        /// If the value is NO, clients SHOULD attempt to render every media
        /// sample in that segment.  This attribute is OPTIONAL. If it is
        /// missing, its value should be treated as NO.
        /// </summary>
        public bool Precise { get; private set; }

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
                ParseTimeOffset(content);
                ParsePrecise(content);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse " + TagName + " tag.", ex);
            }
        }

        private void ParseTimeOffset(string content)
        {
            const string name = "TIME-OFFSET";
            TimeOffset = ValueParser.ParseDecimal(name, content, true);
        }

        private void ParsePrecise(string content)
        {
            const string name = "PRECISE";
            var value = ValueParser.ParseEnumeratedString(name, content, false);
            if (value != string.Empty && !YesNo.IsValid(value))
            {
                throw new SerializationException("Invalid value " + value + " for PRECISE attribute.");
            }
            Precise = value == YesNo.Yes;
        }
    }
}
