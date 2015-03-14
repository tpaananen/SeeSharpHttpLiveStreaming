using System;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;

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
    public class SessionData : BaseTag
    {
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
        /// VALUE is a quoted-string.  It contains the data identified by DATA-
        /// ID.  If the LANGUAGE is specified, VALUE SHOULD contain a human-
        /// readable string written in the specified language.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing a URI.  The resource
        /// identified by the URI MUST be formatted as JSON [RFC7159]; otherwise,
        /// clients may fail to interpret the resource.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing a language tag [RFC5646] that
        /// identifies the language of the VALUE.  This attribute is OPTIONAL.
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
            var value = ValueParser.ParseQuotedString(name, content, false); // MUST BE IF VALUE DOES NOT EXIST
            if (value != string.Empty)
            {
                Uri = new Uri(value);
            }
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
