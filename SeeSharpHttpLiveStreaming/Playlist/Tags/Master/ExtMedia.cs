﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;
using SeeSharpHttpLiveStreaming.Utils.Writers;
using UriParser = SeeSharpHttpLiveStreaming.Utils.ValueParsers.UriParser;

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
    internal class ExtMedia : MasterBaseTag
    {

        private readonly QuotedStringParser _parser = new QuotedStringParser();
        private readonly EnumeratedStringParser _enumeratedParser = new EnumeratedStringParser();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtMedia"/> class.
        /// </summary>
        internal ExtMedia()
        {
            UsingDefaultCtor = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtMedia"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="language">The language.</param>
        /// <param name="assocLanguage">The assoc language.</param>
        /// <param name="name">The name.</param>
        /// <param name="isDefault">if set to <c>true</c> DEFAULT is set to YES; otherwise, DEFAULT is set to NO.</param>
        /// <param name="autoSelect">if set to <c>true</c> AUTOSELECT is set to YES; otherwise, set to NO.</param>
        /// <param name="forced">if set to <c>true</c> FORCED is set to YES; otherwise, set to NO.</param>
        /// <param name="instreamId">The instream ID.</param>
        /// <param name="characteristics">The characteristics.</param>
        /// <param name="uri">The URI. If type is CLOSED-CAPTIONS, this property is ignored.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when 
        /// <paramref name="type"/> or 
        /// <paramref name="groupId"/> or 
        /// <paramref name="name"/>
        /// is <b>null</b>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="type"/> or 
        /// <paramref name="groupId"/> or 
        /// <paramref name="name"/> is empty.
        /// </exception>
        public ExtMedia(string type, string groupId, string language, string assocLanguage, string name,
                        bool isDefault, bool autoSelect, bool forced, string instreamId,
                        IReadOnlyCollection<string> characteristics, Uri uri = null)
        {
            type.RequireNotEmpty("type");
            groupId.RequireNotEmpty("groupId");
            name.RequireNotEmpty("name");
            if (!MediaTypes.IsValid(type))
            {
                throw new ArgumentException("The media type is invalid.", "type");
            }

            if (type == MediaTypes.ClosedCaptions)
            {
                instreamId.RequireNotEmpty("instreamId");
                try
                {
                    ValidateInstreamId(instreamId, true);
                }
                catch (InvalidOperationException ex)
                {
                    throw new ArgumentException(ex.Message, "instreamId", ex);
                }
                InstreamId = instreamId;
            }
            else
            {
                InstreamId = string.Empty;
                Uri = uri; // can be null
            }

            Type = type;
            GroupId = groupId;
            Language = language;
            AssocLanguage = assocLanguage;
            Name = name;
            Default = isDefault;
            AutoSelect = autoSelect || isDefault;
            Forced = forced;
            Characteristics = characteristics ?? new ReadOnlyCollection<string>(new string[0]);
        }
        
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
        /// Playlist file. This attribute is OPTIONAL; see Section 4.3.4.2.1.
        /// If the TYPE is CLOSED-CAPTIONS, the URI attribute MUST NOT be
        /// present.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// The value is a quoted-string which specifies the group to which the
        /// Rendition belongs. See Section 4.3.4.1.1. This attribute is
        /// REQUIRED.
        /// </summary>
        public string GroupId { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing one of the standard Tags for
        /// Identifying Languages [RFC5646], which identifies the primary
        /// language used in the Rendition. This attribute is OPTIONAL.
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
        /// description SHOULD be in that language. This attribute is REQUIRED.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The value to be parsed is an enumerated-string; valid strings are YES and NO. If
        /// the value is YES, then the client SHOULD play this Rendition of the
        /// content in the absence of information from the user indicating a
        /// different choice. This attribute is OPTIONAL. Its absence indicates
        /// an implicit value of NO.
        /// </summary>
        public bool Default { get; private set; }

        /// <summary>
        /// The value to be parsed is an enumerated-string; valid strings are YES and NO.
        /// This attribute is OPTIONAL. Its absence indicates an implicit value
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
        /// This attribute is OPTIONAL. Its absence indicates an implicit value
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

        /// <summary>
        /// Serializes the attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void SerializeAttributes(IPlaylistWriter writer)
        {
            var hasPreviousAttributes = false;
            WriteEnumeratedString(writer, "TYPE", Type, ref hasPreviousAttributes);
            if (Type != MediaTypes.ClosedCaptions)
            {
                WriteUri(writer, "URI", Uri, ref hasPreviousAttributes);
            }
            else
            {
                WriteQuotedString(writer, "INSTREAM-ID", InstreamId, ref hasPreviousAttributes);
            }
            WriteQuotedString(writer, "GROUP-ID", GroupId, ref hasPreviousAttributes);
            WriteQuotedString(writer, "LANGUAGE", Language, ref hasPreviousAttributes);
            WriteQuotedString(writer, "ASSOC-LANGUAGE", AssocLanguage, ref hasPreviousAttributes);
            WriteQuotedString(writer, "NAME", Name, ref hasPreviousAttributes);
            WriteEnumeratedString(writer, "DEFAULT", YesNo.FromBoolean(Default), ref hasPreviousAttributes);
            WriteForcedAttribute(writer, ref hasPreviousAttributes);
            WriteAutoSelect(writer, ref hasPreviousAttributes);
            WriteCharacteristics(writer, ref hasPreviousAttributes);
        }

        /// <summary>
        /// Adds the tag properties to playlist properties.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        internal override void AddToPlaylist(MasterPlaylist playlist)
        {
        }

        private void WriteCharacteristics(IPlaylistWriter writer, ref bool hasPreviousAttributes)
        {
            var commaSeparatedList = string.Join(",", Characteristics);
            WriteQuotedString(writer, "CHARACTERISTICS", commaSeparatedList, ref hasPreviousAttributes);
        }

        private void WriteForcedAttribute(IPlaylistWriter writer, ref bool hasPreviousAttributes)
        {
            if (Type == MediaTypes.Subtitles)
            {
                WriteEnumeratedString(writer, "FORCED", YesNo.FromBoolean(Forced), ref hasPreviousAttributes);
            }
        }

        private void WriteAutoSelect(IPlaylistWriter writer, ref bool hasPreviousAttributes)
        {
            if (Default)
            {
                WriteEnumeratedString(writer, "AUTOSELECT", YesNo.Yes, ref hasPreviousAttributes);
            }
            else
            {
                WriteEnumeratedString(writer, "AUTOSELECT", YesNo.FromBoolean(AutoSelect), ref hasPreviousAttributes);
            }
        }

        #region Parsing

        private void ParseType(string content)
        {
            const string name = "TYPE";
            Type = new EnumeratedStringParser().Parse(name, content, true);

            if (!MediaTypes.IsValid(Type))
            {
                throw new SerializationException("Failed to parse TYPE attribute.");
            }
        }

        private void ParseUri(string content)
        {
            const string name = "URI";
            bool mustNotExist = Type == MediaTypes.ClosedCaptions;
            var uri = new UriParser(BaseUri).Parse(name, content, false);
            if (mustNotExist)
            {
                if (uri != null)
                {
                    throw new SerializationException("Failed to parse URI attribute, it must not exist when the TYPE is CLOSED-CAPTIONS.");
                }
            }
            Uri = uri;
        }

        private void ParseGroupId(string content)
        {
            const string name = "GROUP-ID";
            GroupId = _parser.Parse(name, content, true);
        }

        private void ParseLanguage(string content)
        {
            const string name = "LANGUAGE";
            Language = _parser.Parse(name, content, false);
        }

        private void ParseAssocLanguage(string content)
        {
            const string name = "ASSOC-LANGUAGE";
            AssocLanguage = _parser.Parse(name, content, false);
        }

        private void ParseName(string content)
        {
            const string name = "NAME";
            Name = _parser.Parse(name, content, true);
        }

        private void ParseDefault(string content)
        {
            const string name = "DEFAULT";
            var defaultValue = _enumeratedParser.Parse(name, content, false);
            if (defaultValue != string.Empty && !YesNo.IsValid(defaultValue))
            {
                throw new SerializationException("Invalid value provided in DEFAULT attribute.");
            }
            Default = YesNo.FromString(defaultValue);
        }

        private void ParseAutoSelect(string content)
        {
            const string name = "AUTOSELECT";
            var value = _enumeratedParser.Parse(name, content, false);
            if (value != string.Empty && !YesNo.IsValid(value))
            {
                throw new SerializationException("Invalid value provided in AUTOSELECT attribute.");
            }
            AutoSelect = YesNo.FromString(value);
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
            var value = _enumeratedParser.Parse(name, content, false);
            if (value != string.Empty && !YesNo.IsValid(value))
            {
                throw new SerializationException("Invalid value provided in FORCED attribute.");
            }
            if (value != string.Empty && Type != MediaTypes.Subtitles)
            {
                throw new SerializationException("FORCED attribute must not exist when TYPE is not SUBTITLES.");
            }
            Forced = YesNo.FromString(value);
        }

        private void ParseInstreamId(string content)
        {
            // see property comments describing the parsing logic
            const string name = "INSTREAM-ID";
            var isRequired = Type == MediaTypes.ClosedCaptions;
            var value = new QuotedStringParser().Parse(name, content, isRequired);
            ValidateInstreamId(value, isRequired);
            InstreamId = value;
        }

        private void ParseCharacteristics(string content)
        {
            const string name = "CHARACTERISTICS";
            var parser = new StringWithSeparatorParser<string>(x => x);
            var values = parser.Parse(name, content, false);
            Characteristics = new ReadOnlyCollection<string>(values);
        }

        #endregion

        private static void ValidateInstreamId(string value, bool isRequired)
        {
            if (value == string.Empty && isRequired)
            {
                throw new InvalidOperationException("Attribute INSTREAM-ID is required if TYPE is CLOSED-CAPTIONS.");
            }
            if (!isRequired)
            {
                if (value != string.Empty)
                {
                    throw new InvalidOperationException("Attribute INSTREAM-ID must not exist if TYPE is not CLOSED-CAPTIONS.");
                }
                return;
            }
            if (value == "CC1" || value == "CC2" || value == "CC3" || value == "CC4")
            {
                return;
            }

            // value must be one of the SERVICEn
            if (!value.StartsWith("SERVICE"))
            {
                throw new InvalidOperationException("Invalid INSTREAM-ID attribute value.");
            }

            var number = value.Substring("SERVICE".Length);
            var temp = int.Parse(number);
            if (temp < 1 || temp > 63)
            {
                throw new InvalidOperationException(
                    "Invalid INSTREAM-ID attribute value. The number of the service value is out of range.");
            }
        }

        /// <summary>
        /// Check whether any of the <paramref name="others"/> has the same set of attributes.
        /// </summary>
        /// <param name="others">The others.</param>
        /// <returns>
        /// <b>True</b> if the same set of attributes are found; otherwise, <b>false</b>.
        /// </returns>
        internal bool EqualityCheck(IEnumerable<ExtMedia> others)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var other in others)
            {
                if (Default == other.Default && 
                    AutoSelect == other.AutoSelect && 
                    Forced == other.Forced && 
                    string.Equals(Language, other.Language) && 
                    string.Equals(AssocLanguage, other.AssocLanguage) && 
                    string.Equals(Name, other.Name) && 
                    string.Equals(InstreamId, other.InstreamId) && 
                    Characteristics.SequenceEqual(other.Characteristics))
                {
                    return true;
                }
                
            }
            return false;
        }
    }
}
