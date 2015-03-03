using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace SeeSharpLiveStreaming.Utils.ValueParsers
{
    /// <summary>
    /// Static value parser class that can parse attribute values from the playlist files.
    /// </summary>
    internal static class ValueParser
    {
        private const NumberStyles DecimalParsingNumberStyles = NumberStyles.Float | 
                                                                NumberStyles.Number | 
                                                                NumberStyles.Integer;

        /// <summary>
        /// Gets the start position where the parsing starts.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">If set to <c>true</c> the attribute is required to exist in the <paramref name="line"/>.</param>
        /// <returns></returns>
        private static int StartPosition(string attribute, string line, bool requireExists)
        {
            var position = line.IndexOf(attribute + "=", StringComparison.Ordinal);
            if (requireExists && position < 0)
            {
                throw new SerializationException("Could not locate the attribute " + attribute);
            }
            if (position < 0)
            {
                return -1;
            }

            return position + attribute.Length + 1; // 1 == "=" mark
        }

        /// <summary>
        /// Gets the unquoted attribute string value to convert to some target type.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">
        /// If set to <c>true</c> the <paramref name="attribute"/> 
        /// is required to exist in the <paramref name="line"/>.
        /// </param>
        /// <returns></returns>
        private static string GetUnquotedStringToConvert(string attribute, string line, bool requireExists)
        {
            var position = StartPosition(attribute, line, requireExists);
            if (position < 0)
            {
                return string.Empty;
            }

            var substring = line.Substring(position);
            var endPosition = substring.IndexOf(",", StringComparison.Ordinal);
            if (endPosition < 0) // this is the last, there is no comma at the end of the line
            {
                return substring;
            }

            substring = substring.Substring(0, endPosition);
            return substring;
        }

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
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
        public static string ParseQuotedString(string attribute, string line, bool requireExists)
        {
            var position = StartPosition(attribute, line, requireExists);
            if (position < 0)
            {
                return string.Empty;
            }

            var substring = line.Substring(position + 1); // + 1 is quatation mark
            var endPosition = substring.IndexOf("\"", StringComparison.Ordinal);
            if (endPosition < 0)
            {
                return string.Empty;
            }

            substring = substring.Substring(0, endPosition);
            return substring;
        }

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
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
        public static IList<string> ParseCommaSeparatedQuotedString(string attribute, string line, bool requireExists)
        {
            var substring = ParseQuotedString(attribute, line, requireExists);
            return substring == string.Empty ? new List<string>() : substring.Split(',').ToList();
        }

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
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
        public static int ParseInt(string attribute, string line, bool requireExists)
        {
            var stringValue = GetUnquotedStringToConvert(attribute, line, requireExists);
            if (stringValue != string.Empty)
            {
                return int.Parse(stringValue, CultureInfo.InvariantCulture);
            }
            return 0;
        }

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
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
        public static decimal ParseDecimal(string attribute, string line, bool requireExists)
        {
            string value = GetUnquotedStringToConvert(attribute, line, requireExists);

            return value != string.Empty 
                ? decimal.Parse(value, DecimalParsingNumberStyles, CultureInfo.InvariantCulture) 
                : 0;
        }

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
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
        public static double ParseDouble(string attribute, string line, bool requireExists)
        {
            string value = GetUnquotedStringToConvert(attribute, line, requireExists);

            return value != string.Empty
                ? double.Parse(value, DecimalParsingNumberStyles, CultureInfo.InvariantCulture)
                : 0;
        }

        /// <summary>
        /// Parses the specified attribute value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <exception cref="SerializationException">
        /// Thrown when the line cannot be parsed as an integer.
        /// </exception>
        /// <returns>
        /// Integer parsed from the <paramref name="line"/>.
        /// </returns>
        public static int ParseInt(string line)
        {
            int value;
            if (!int.TryParse(line, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                throw new SerializationException("Failed to parse attribute value.");
            }
            return value;
        }

        /// <summary>
        /// Parses the specified attribute value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <exception cref="SerializationException">
        /// Thrown when the line cannot be parsed as an integer.
        /// </exception>
        /// <returns>
        /// <see cref="Decimal"/> parsed from the <paramref name="line"/>.
        /// </returns>
        public static decimal ParseDecimal(string line)
        {
            decimal value;
            if (!decimal.TryParse(line, DecimalParsingNumberStyles, CultureInfo.InvariantCulture, out value))
            {
                throw new SerializationException("Failed to parse attribute value.");
            }
            return value;
        }

        /// <summary>
        /// Parses the specified attribute value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <exception cref="SerializationException">
        /// Thrown when the line cannot be parsed as an integer.
        /// </exception>
        /// <returns>
        /// <see cref="Double"/> parsed from the <paramref name="line"/>.
        /// </returns>
        public static double ParseDouble(string line)
        {
            double value;
            if (!double.TryParse(line, DecimalParsingNumberStyles, CultureInfo.InvariantCulture, out value))
            {
                throw new SerializationException("Failed to parse attribute value.");
            }
            return value;
        }
    }
}
