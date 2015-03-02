using System;
using System.Runtime.Serialization;
using SeeSharpLiveStreaming.Utils;

namespace SeeSharpLiveStreaming.Playlist.Tags.Master
{
    /// <summary>
    /// The EXT-X-STREAM-INF tag specifies a Variant Stream, which is a set
    /// of Renditions which can be combined to play the presentation.The
    /// attributes of the tag provide information about the Variant Stream.
    /// The EXT-X-STREAM-INF tag identifies the next URI line in the Playlist
    /// as a Rendition of the Variant Stream.
    /// Its format is:
    /// #EXT-X-STREAM-INF:<attribute-list>
    /// </summary>
    public class StreamInf : BaseTag
    {

        /// <summary>
        /// The value is a decimal-integer of bits per second. It represents the
        /// peak segment bit rate of the Variant Stream.
        /// If all the Media Segments in a Variant Stream have already been
        /// created, the BANDWIDTH value MUST be the largest sum of peak segment
        /// bit rates that is produced by any playable combination of Renditions.
        /// (For a Variant Stream with a single Media Playlist, this is just the
        /// peak segment bit rate of that Media Playlist.) An inaccurate value
        /// can cause playback stalls or prevent clients from playing the
        /// variant.
        /// If the Master Playlist is to be made available before all Media
        /// Segments in the presentation have been encoded, the BANDWIDTH value
        /// SHOULD be the BANDWIDTH value of a representative period of similar
        /// content, encoded using the same settings.
        /// Every EXT-X-STREAM-INF tag MUST include the BANDWIDTH attribute.
        /// </summary>
        public decimal Bandwidth { get; private set; }

        /// <summary>
        /// The value is a decimal-integer of bits per second. It represents the
        /// average segment bit rate of the Variant Stream.
        /// If all the Media Segments in a Variant Stream have already been
        /// created, the AVERAGE-BANDWIDTH value MUST be the largest sum of
        /// average segment bit rates that is produced by any playable
        /// combination of Renditions. (For a Variant Stream with a single Media
        /// Playlist, this is just the average segment bit rate of that Media
        /// Playlist.) An inaccurate value can cause playback stalls or prevent
        /// clients from playing the variant.
        /// If the Master Playlist is to be made available before all Media
        /// Segments in the presentation have been encoded, the AVERAGE-BANDWIDTH
        /// value SHOULD be the AVERAGE-BANDWIDTH value of a representative
        /// period of similar content, encoded using the same settings.
        /// The AVERAGE-BANDWIDTH attribute is OPTIONAL.
        /// </summary>
        public decimal AverageBandwith { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing a comma-separated list of
        /// formats, where each format specifies a media sample type that is
        /// present in one or more Renditions specified by the Variant Stream.
        /// Valid format identifiers are those in the ISO Base Media File Format
        /// Name Space defined by The ’Codecs’ and ’Profiles’ Parameters for
        /// "Bucket" Media Types [RFC6381].
        /// Every EXT-X-STREAM-INF tag SHOULD include a CODECS attribute.
        /// </summary>
        public string Codecs { get; private set; }

        /// <summary>
        /// The value is a decimal-resolution describing the optimal pixel
        /// resolution at which to display all the video in the Variant Stream.
        /// The RESOLUTION attribute is OPTIONAL but is recommended if the
        /// Variant Stream includes video.
        /// </summary>
        public decimal Resolution { get; private set; }

        /// <summary>
        /// The value is a quoted-string. It MUST match the value of the GROUP-
        /// ID attribute of an EXT-X-MEDIA tag elsewhere in the Master Playlist
        /// whose TYPE attribute is AUDIO. It indicates the set of audio
        /// Renditions that MAY be used when playing the presentation. See
        /// Section 4.3.4.2.1. of the specification.
        /// The AUDIO attribute is OPTIONAL.
        /// </summary>
        public string Audio { get; private set; }

        /// <summary>
        /// The value is a quoted-string. It MUST match the value of the GROUP-
        /// ID attribute of an EXT-X-MEDIA tag elsewhere in the Master Playlist
        /// whose TYPE attribute is VIDEO. It indicates the set of video
        /// Renditions that MAY be used when playing the presentation.See
        /// Section 4.3.4.2.1. of the specification.
        /// The VIDEO attribute is OPTIONAL.
        /// </summary>
        public string Video { get; private set; }

        /// <summary>
        /// The value is a quoted-string. It MUST match the value of the GROUP-
        /// ID attribute of an EXT-X-MEDIA tag elsewhere in the Master Playlist
        /// whose TYPE attribute is SUBTITLES. It indicates the set of subtitle
        /// Renditions that MAY be used when playing the presentation.See
        /// Section 4.3.4.2.1.
        /// The SUBTITLES attribute is OPTIONAL.
        /// </summary>
        public string Subtitles { get; private set; }

        /// <summary>
        /// The value can be either a quoted-string or an enumerated-string with
        /// the value NONE.If the value is a quoted-string, it MUST match the
        /// value of the GROUP-ID attribute of an EXT-X-MEDIA tag elsewhere in
        /// the Playlist whose TYPE attribute is CLOSED-CAPTIONS, and indicates
        /// the set of closed-caption Renditions that MAY be used when playlist
        /// the presentation. See Section 4.3.4.2.1.
        /// If the value is the enumerated-string value NONE, all EXT-X-STREAM-
        /// INF tags MUST have this attribute with a value of NONE, indicating
        /// that there are no closed captions in any Variant Stream in the Master
        /// Playlist. Having closed captions in one Variant Stream but not
        /// another can trigger playback inconsistencies.
        /// The CLOSED-CAPTIONS attribute is OPTIONAL.
        /// </summary>
        public string ClosedCaptions { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has closed captions.
        /// </summary>
        public bool HasClosedCaptions => ClosedCaptions == null || 
                                         ClosedCaptions.Equals("NONE", StringComparison.Ordinal);

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType => TagType.ExtXStreamInf;

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        public override void Deserialize(string content, int version)
        {
            try
            {
                content.RequireNotNull("content");
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to deserialize EXT-X-STREAM-INF tag.", ex);
            }
        }
    }
}
