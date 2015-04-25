using System.Runtime.Serialization;

namespace SeeSharpHttpLiveStreaming.Utils.ValueParsers
{
    internal class QuotedStringParser : ValueParserBase<string>
    {
        /// <summary>
        /// Parses the specified attribute value.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">
        /// If set to <c>true</c> the <paramref name="attribute"/> is 
        /// required to exist in the <paramref name="line"/>.
        /// </param>
        /// <returns>
        /// The parsed value.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        public override string Parse(string attribute, string line, bool requireExists)
        {
            return ParseString(attribute, line, requireExists, true);
        }
    }
}
