using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Master
{
    /// <summary>
    /// The EXT-X-MEDIA tag is used to relate Media Playlists that contain
    /// alternative Renditions (Section 4.3.4.2.1) of the same content. For
    /// example, three EXT-X-MEDIA tags can be used to identify audio-only
    /// Media Playlists that contain English, French and Spanish Renditions
    /// of the same presentation. Or two EXT-X-MEDIA tags can be used to
    /// identify video-only Media Playlists that show two different camera
    /// angles.
    /// Its format is:
    /// #EXT-X-MEDIA:&lt;attribute-list&gt;
    /// </summary>
    public class ExtMedia : BaseTag
    {
        
        /// <summary>
        /// The value is an enumerated-string; valid strings are AUDIO, VIDEO,
        /// SUBTITLES and CLOSED-CAPTIONS.If the value is AUDIO, the Playlist
        /// described by the tag MUST contain audio media.If the value is
        /// VIDEO, the Playlist MUST contain video media. If the value is
        /// SUBTITLES, the Playlist MUST contain subtitle media. If the value is
        /// CLOSED-CAPTIONS, the Media Segments for the video Renditions can
        /// include closed captions. Specifying a Playlist that does not contain
        /// the appropriate media type can lead to client playback errors. This
        /// attribute is REQUIRED.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing a URI that identifies the
        /// Playlist file.This attribute is OPTIONAL; see Section 4.3.4.2.1.
        /// If the TYPE is CLOSED-CAPTIONS, the URI attribute MUST NOT be
        /// present.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// The value is a quoted-string which specifies the group to which the
        /// Rendition belongs.See Section 4.3.4.1.1. This attribute is
        /// REQUIRED.
        /// </summary>
        public string GroupId { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing one of the standard Tags for
        /// Identifying Languages [RFC5646], which identifies the primary
        /// language used in the Rendition.This attribute is OPTIONAL.
        /// </summary>
        public string Language { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing a language tag [RFC5646] that
        /// identifies a language that is associated with the Rendition.An
        /// associated language is often used in a different role than the
        /// language specified by the LANGUAGE attribute (e.g.written vs.
        /// spoken, or as a fallback dialect). This attribute is OPTIONAL.
        /// </summary>
        public string AssocLanguage { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing a human-readable description
        /// of the Rendition.If the LANGUAGE attribute is present then this
        /// description SHOULD be in that language.This attribute is REQUIRED.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The value to be parsed is an enumerated-string; valid strings are YES and NO. If
        /// the value is YES, then the client SHOULD play this Rendition of the
        /// content in the absence of information from the user indicating a
        /// different choice.This attribute is OPTIONAL. Its absence indicates
        /// an implicit value of NO.
        /// </summary>
        public bool Default { get; private set; }

        /// <summary>
        /// The value to be parsed is an enumerated-string; valid strings are YES and NO.
        /// This attribute is OPTIONAL.Its absence indicates an implicit value
        /// of NO. If the value is YES, then the client MAY choose to play this
        /// Rendition in the absence of explicit user preference because it
        /// matches the current playback environment, such as chosen system
        /// language.
        /// If the AUTOSELECT attribute is present, its value MUST be YES if the
        /// value of the DEFAULT attribute is YES.
        /// </summary>
        public bool AutoSelect { get; private set; }

        /// <summary>
        /// The value is an enumerated-string; valid strings are YES and NO.
        /// This attribute is OPTIONAL.Its absence indicates an implicit value
        /// of NO. The FORCED attribute MUST NOT be present unless the TYPE is
        /// SUBTITLES.
        /// A value of YES indicates that the Rendition contains content which is
        /// considered essential to play. When selecting a FORCED Rendition, a
        /// client SHOULD choose the one that best matches the current playback
        /// environment (e.g.language).
        /// A value of NO indicates that the Rendition contains content which is
        /// intended to be played in response to explicit user request.
        /// </summary>
        public bool Forced { get; private set; }

        /// <summary>
        /// The value is a quoted-string that specifies a Rendition within the
        /// segments in the Media Playlist. This attribute is REQUIRED if the
        /// TYPE attribute is CLOSED-CAPTIONS, in which case it MUST have one of
        /// the values: "CC1", "CC2", "CC3", "CC4", or "SERVICEn" where n MUST be
        /// an integer between 1 and 63 (e.g."SERVICE3" or "SERVICE42").
        /// The values "CC1", "CC2", "CC3", and "CC4" identify a Line 21 Data
        /// Services channel[CEA608]. The "SERVICE" values identify a Digital
        /// Television Closed Captioning[CEA708] service block number.
        /// For all other TYPE values, the INSTREAM-ID MUST NOT be specified.
        /// </summary>
        public string InstreamId { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing one or more Uniform Type
        /// Identifiers [UTI] separated by comma(,) characters. This attribute
        /// is OPTIONAL. Each UTI indicates an individual characteristic of the
        /// Rendition.
        /// A SUBTITLES Rendition MAY include the following characteristics:
        /// "public.accessibility.transcribes-spoken-dialog";
        /// "public.accessibility.describes-music-and-sound"; 
        /// "public.easy-to-read" (which indicates that the subtitles have been edited for ease
        /// of reading).
        /// An AUDIO Rendition MAY include the following characteristics:
        /// "public.accessibility.describes-video".
        /// The CHARACTERISTICS attribute MAY include private UTIs.
        /// </summary>
        public IReadOnlyCollection<string> Characteristics { get; private set; }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-MEDIA"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXMedia; }
        }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>..
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="content"/> is <b>null</b>.</exception>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        public override void Deserialize(string content, int version)
        {
            content.RequireNotEmpty("content");
            try
            {
                ParseType(content);
                ParseUri(content);
                ParseGroupId(content);
                ParseLanguage(content);
                ParseAssocLanguage(content);
                ParseName(content);
                ParseDefault(content);
                ParseAutoSelect(content);
                ParseForced(content);
                ParseInstreamId(content);
                ParseCharacteristics(content);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to deserialize EXT-X-MEDIA tag.", ex);
            }
        }

        private void ParseType(string content)
        {
            const string name = "TYPE";
            Type = ValueParser.ParseEnumeratedString(name, content, true);

            if (!MediaTypes.IsValid(Type))
            {
                throw new SerializationException("Failed to parse TYPE attribute.");
            }
        }

        private void ParseUri(string content)
        {
            const string name = "URI";
            bool mustNotExists = Type == MediaTypes.ClosedCaptions;
            var uriString = ValueParser.ParseQuotedString(name, content, false);
            if (mustNotExists)
            {
                if (uriString != string.Empty)
                {
                    throw new SerializationException("Failed to parse URI attribute, it must not exists when the TYPE is CLOSED-CAPTIONS.");
                }
            }

            if (uriString != string.Empty)
            {
                Uri = new Uri(uriString);
            }
        }

        private void ParseGroupId(string content)
        {
            const string name = "GROUP-ID";
            GroupId = ValueParser.ParseQuotedString(name, content, true);
        }

        private void ParseLanguage(string content)
        {
            const string name = "LANGUAGE";
            Language = ValueParser.ParseQuotedString(name, content, false);
        }

        private void ParseAssocLanguage(string content)
        {
            const string name = "ASSOC-LANGUAGE";
            AssocLanguage = ValueParser.ParseQuotedString(name, content, false);
        }

        private void ParseName(string content)
        {
            const string name = "NAME";
            Name = ValueParser.ParseQuotedString(name, content, true);
        }

        private void ParseDefault(string content)
        {
            const string name = "DEFAULT";
            var defaultValue = ValueParser.ParseEnumeratedString(name, content, false);
            if (defaultValue != string.Empty && !YesNo.IsValid(defaultValue))
            {
                throw new SerializationException("Invalid value provided in DEFAULT attribute.");
            }
            Default = defaultValue == YesNo.Yes;
        }

        private void ParseAutoSelect(string content)
        {
            const string name = "AUTOSELECT";
            var value = ValueParser.ParseEnumeratedString(name, content, false);
            if (value != string.Empty && !YesNo.IsValid(value))
            {
                throw new SerializationException("Invalid value provided in AUTOSELECT attribute.");
            }
            AutoSelect = value == YesNo.Yes;
            //if (value != string.Empty && Default && !AutoSelect)
            if (value != string.Empty)
            {
                if (Default)
                {
                    if (!AutoSelect)
                    {
                        throw new SerializationException("The AUTOSELECT must be YES when provided and DEFAULT is YES.");
                    }
                }
            }
        }

        private void ParseForced(string content)
        {
            const string name = "FORCED";
            var value = ValueParser.ParseEnumeratedString(name, content, false);
            if (value != string.Empty && !YesNo.IsValid(value))
            {
                throw new SerializationException("Invalid value provided in FORCED attribute.");
            }
            if (value != string.Empty && Type != MediaTypes.Subtitles)
            {
                throw new SerializationException("FORCED attribute must not exist when TYPE is not SUBTITLES.");
            }
            Forced = value == YesNo.Yes;
        }

        private void ParseInstreamId(string content)
        {
            // see property comments describing the parsing logic
            const string name = "INSTREAM-ID";
            var isRequired = Type == MediaTypes.ClosedCaptions;
            var value = ValueParser.ParseQuotedString(name, content, isRequired);

            if (value == string.Empty && isRequired)
            {
                throw new SerializationException("Attribute INSTREAM-ID is required if TYPE is CLOSED-CAPTIONS.");
            }
            if (!isRequired)
            {
                if (value != string.Empty)
                {
                    throw new SerializationException("Attribute INSTREAM-ID must not exist if TYPE is not CLOSED-CAPTIONS.");
                }
                return;
            }

            if (value != "CC1" && value != "CC2" && value != "CC3" && value != "CC4")
            {
                // value must be one of the SERVICEn
                if (!value.StartsWith("SERVICE"))
                {
                    throw new SerializationException("Invalid INSTREAM-ID attribute value.");
                }

                var number = value.Substring("SERVICE".Length);
                var temp = ValueParser.ParseInt(number);
                if (temp < 1 || temp > 63)
                {
                    throw new SerializationException("Invalid INSTREAM-ID attribute value. The number of the service value is out of range.");
                }
            }
            InstreamId = value;
        }

        private void ParseCharacteristics(string content)
        {
            const string name = "CHARACTERISTICS";
            Characteristics = new ReadOnlyCollection<string>(ValueParser.ParseSeparatedQuotedString(name, content, false));
        }

    }
}
