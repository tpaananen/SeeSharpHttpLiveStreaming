using System;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media
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
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-TARGETDURATION"; }
        }

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
        public long Duration { get; private set; }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>..
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public override void Deserialize(string content, int version)
        {
            content.RequireNotEmpty("content");
            try
            {
                Duration = ValueParser.ParseInt(content);
                if (Duration == 0)
                {
                    throw new SerializationException("Non zero value is required.");
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse EXT-X-TARGETDURATION tag.", ex);
            }
        }
    }
}
