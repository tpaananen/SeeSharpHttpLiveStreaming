using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace SeeSharpHttpLiveStreaming.Utils.ValueParsers
{
    internal class IntegerParser : ValueParserBase<long>
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
        /// The parsed value or default value of zero.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        /// <exception cref="FormatException">
        /// Thrown when parsing of the value fails.
        /// </exception>
        public override long Parse(string attribute, string line, bool requireExists)
        {
            var stringValue = ParseString(attribute, line, requireExists, false);
            if (stringValue != string.Empty)
            {
                return long.Parse(stringValue, CultureInfo.InvariantCulture);
            }
            return 0;
        }
    }
}
