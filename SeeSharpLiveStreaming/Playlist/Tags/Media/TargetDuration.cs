﻿using System;
using System.Runtime.Serialization;
using SeeSharpLiveStreaming.Utils;
using SeeSharpLiveStreaming.Utils.ValueParsers;

namespace SeeSharpLiveStreaming.Playlist.Tags.Media
{
    /// <summary>
    /// Represents EXT-X-TARGETDURATION tag. 
    /// </summary>
    /// <remarks>
    /// The EXT-X-TARGETDURATION tag specifies the maximum Media Segment
    /// duration.The EXTINF duration of each Media Segment in the Playlist
    /// file, when rounded to the nearest integer, MUST be less than or equal
    /// to the target duration; longer segments can trigger playback stalls
    /// or other errors.It applies to the entire Playlist file.Its format
    /// is:
    /// #EXT-X-TARGETDURATION:&lt;s&gt;
    /// where s is a decimal-integer indicating the target duration in
    /// seconds.The EXT-X-TARGETDURATION tag is REQUIRED.
    /// </remarks>
    public class TargetDuration : BaseTag
    {
        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXTargetDuration; }
        }


        /// <summary>
        /// Gets the duration.
        /// </summary>
        public decimal Duration { get; private set; }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public override void Deserialize(string content, int version)
        {
            content.RequireNotNull("content");
            try
            {
                Duration = ValueParser.ParseDecimal(content);
                if (Duration == 0)
                {
                    throw new SerializationException("The EXT-X-TARGETDURATION is required.");
                }
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse EXT-X-TARGETDURATION tag.", ex);
            }
        }
    }
}
