using System;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Master
{
    /// <summary>
    /// Represents EXT-X-SESSION-DATA tag.
    /// </summary>
    /// <remarks>
    /// The EXT-X-SESSION-DATA tag allows arbitrary session data to be
    /// carried in a Master Playlist.
    ///
    /// Its format is:
    ///
    /// #EXT-X-SESSION-DATA:&lt;attribute list&gt;
    /// </remarks>
    internal class SessionData : BaseTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionData"/> class.
        /// </summary>
        internal SessionData()
        {
            UsingDefaultCtor = true;
        }

        private SessionData(string dataId, string language)
        {
            dataId.RequireNotEmpty("dataId");
            DataId = dataId;
            Language = language;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionData"/> class.
        /// </summary>
        /// <param name="dataId">The data identifier.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="language">The language.</param>
        public SessionData(string dataId, Uri uri, string language)
            : this (dataId, language)
        {
            uri.RequireNotNull("uri");
            Uri = uri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionData"/> class.
        /// </summary>
        /// <param name="dataId">The data identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="language">The language.</param>
        public SessionData(string dataId, string value, string language)
            : this (dataId, language)
        {
            value.RequireNotEmpty("value");
            Value = value;
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-SESSION-DATA"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXSessionData; }
        }

        /// <summary>
        /// The value of DATA-ID is a quoted-string which identifies that data
        /// value. The DATA-ID SHOULD conform to a reverse DNS naming
        /// convention, such as "com.example.movie.title". This attribute is
        /// REQUIRED.
        /// </summary>
        public string DataId { get; private set; }

        /// <summary>
        /// VALUE is a quoted-string. It contains the data identified by DATA-
        /// ID. If the LANGUAGE is specified, VALUE SHOULD contain a human-
        /// readable string written in the specified language.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing a URI. The resource
        /// identified by the URI MUST be formatted as JSON [RFC7159]; otherwise,
        /// clients may fail to interpret the resource.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing a language tag [RFC5646] that
        /// identifies the language of the VALUE. This attribute is OPTIONAL.
        /// </summary>
        public string Language { get; private set; }

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
                ParseDataId(content);
                ParseUri(content);
                ParseValue(content);
                ParseLanguage(content);
                AssertUriOrValueExistsButNotBoth();
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse EXT-X-SESSION-DATA tag.", ex);
            }
        }

        /// <summary>
        /// Serializes the attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void SerializeAttributes(IPlaylistWriter writer)
        {
            AssertUriOrValueExistsButNotBoth();
            var hasPreviousAttributes = false;
            WriteQuotedString(writer, "DATA-ID", DataId, ref hasPreviousAttributes);
            WriteUri(writer, "URI", Uri, ref hasPreviousAttributes);
            WriteQuotedString(writer, "VALUE", Value, ref hasPreviousAttributes);
            WriteQuotedString(writer, "LANGUAGE", Language, ref hasPreviousAttributes);
        }

        private void AssertUriOrValueExistsButNotBoth()
        {
            if (Uri == null && Value == null)
            {
                throw new InvalidOperationException("Either URI or VALUE attribute is required.");
            }
            if (Uri != null && Value != null)
            {
                throw new InvalidOperationException("Either URI or VALUE attribute is required, but not both.");
            }
        }

        private void ParseDataId(string content)
        {
            const string name = "DATA-ID";
            DataId = ValueParser.ParseQuotedString(name, content, true);
        }

        private void ParseUri(string content)
        {
            const string name = "URI";
            Uri = ParseUri(name, content, false);
        }

        private void ParseValue(string content)
        {
            const string name = "VALUE";
            var value = ValueParser.ParseQuotedString(name, content, false);
            if (value != string.Empty)
            {
                Value = value;
            }
        }

        private void ParseLanguage(string content)
        {
            const string name = "LANGUAGE";
            Language = ValueParser.ParseQuotedString(name, content, false);
        }
    }
}
