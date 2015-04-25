using System.Runtime.Serialization;

namespace SeeSharpHttpLiveStreaming.Utils.ValueParsers
{
    internal class EnumeratedStringParser : ValueParserBase<string>
    {
        /// <summary>
        /// Gets the unquoted attribute string value to convert to some target type.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">
        /// If set to <c>true</c> the <paramref name="attribute"/> 
        /// is required to exist in the <paramref name="line"/>.
        /// </param>
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        /// <returns>
        /// Parsed string or <see cref="string.Empty"/>.
        /// </returns>
        public override string Parse(string attribute, string line, bool requireExists)
        {
            return ParseString(attribute, line, requireExists, false);
        }
    }
}
