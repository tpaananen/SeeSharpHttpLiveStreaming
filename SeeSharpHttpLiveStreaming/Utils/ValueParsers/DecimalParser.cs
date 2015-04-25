using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace SeeSharpHttpLiveStreaming.Utils.ValueParsers
{
    internal class DecimalParser : ValueParserBase<decimal>
    {
        private const NumberStyles DecimalParsingNumberStyles = NumberStyles.Float | 
                                                                NumberStyles.Number | 
                                                                NumberStyles.Integer;

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
        public override decimal Parse(string attribute, string line, bool requireExists)
        {
            string value = ParseString(attribute, line, requireExists, false);

            return value != string.Empty 
                ? decimal.Parse(value, DecimalParsingNumberStyles, CultureInfo.InvariantCulture) 
                : 0;
        }

    }
}
