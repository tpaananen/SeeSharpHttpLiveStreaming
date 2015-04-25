using System;
using System.Net;

namespace SeeSharpHttpLiveStreaming.Utils.ValueParsers
{
    internal class UriParser : ValueParserBase<Uri>
    {
        private readonly Uri _baseUri;

        internal UriParser(Uri baseUri)
        {
            baseUri.RequireNotNull("baseUri");
            _baseUri = baseUri;
        }

        /// <summary>
        /// Parses the specified attribute value from the string provided.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="fromString">The string to be parsed from.</param>
        /// <param name="requireExists">If set to <c>true</c> the <paramref name="attribute" /> is
        /// required to exist in the <paramref name="fromString" />.</param>
        /// <returns>
        /// The parsed value or <b>null</b>.
        /// </returns>
        public override Uri Parse(string attribute, string fromString, bool requireExists)
        {
            var value = ParseString(attribute, fromString, requireExists, true);
            if (value == string.Empty)
            {
                return null;
            }
            var uri = WebUtility.UrlDecode(value);
            return UriUtils.CreateUri(uri, _baseUri);
        }
    }
}
