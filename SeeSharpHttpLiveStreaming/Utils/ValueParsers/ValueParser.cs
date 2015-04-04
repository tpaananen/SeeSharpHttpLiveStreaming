using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Utils.ValueParsers
{
    /// <summary>
    /// Static value parser class that can parse attribute values from the playlist files.
    /// See https://tools.ietf.org/html/draft-pantos-http-live-streaming-14#section-4.2 for 
    /// details.
    /// </summary>
    internal static class ValueParser
    {
        private const NumberStyles DecimalParsingNumberStyles = NumberStyles.Float | 
                                                                NumberStyles.Number | 
                                                                NumberStyles.Integer;

        private const int SizeOfByte = 8; // bits
        private const string HexPrefixIdentifier = "0x"; // case ignored

        /// <summary>
        /// Gets the start position where the parsing starts.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">If set to <c>true</c> the attribute is required to exist in the <paramref name="line"/>.</param>
        /// <returns>
        /// Start position of the attribute or -1 if attribute is not found and is not required.
        /// </returns>
        // ReSharper disable once UnusedParameter.Local
        private static int StartPosition(string attribute, string line, bool requireExists)
        {
            var position = line.IndexOf(attribute + "=", StringComparison.Ordinal);
            if (requireExists && position < 0)
            {
                throw new SerializationException("Could not locate the attribute " + attribute + " from line '" + line + "'.");
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
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        /// <returns></returns>
        public static string ParseEnumeratedString(string attribute, string line, bool requireExists)
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
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">If set to <c>true</c> the <paramref name="attribute" /> is
        /// required to exist in the <paramref name="line" />.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="parser">The parser of a single value in the list.</param>
        /// <returns>
        /// The parsed value in a list or an empty list.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="parser"/> is <b>null</b>.
        /// </exception>
        public static IList<T> ParseSeparatedQuotedString<T>(string attribute, string line, 
                                                             bool requireExists,
                                                             Func<string, T> parser,
                                                             char separator = ',')
        {
            parser.RequireNotNull("parser");
            var substring = ParseQuotedString(attribute, line, requireExists);
            if (substring == string.Empty)
            {
                return new List<T>();
            }
            var strings = substring.Split(separator);
            return strings.Select(parser).ToList();
        }

        /// <summary>
        /// Parses the specified attribute value.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">If set to <c>true</c> the <paramref name="attribute" /> is
        /// required to exist in the <paramref name="line" />.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>
        /// The parsed value in a list or an empty list.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        public static IList<string> ParseSeparatedQuotedString(string attribute, string line, 
                                                               bool requireExists, char separator = ',')
        {
            var substring = ParseQuotedString(attribute, line, requireExists);
            if (substring == string.Empty)
            {
                return new List<string>();
            }
            var strings = substring.Split(separator);
            return strings.ToList();
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
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        /// <exception cref="FormatException">
        /// Thrown when parsing of the value fails.
        /// </exception>
        public static long ParseInt(string attribute, string line, bool requireExists)
        {
            var stringValue = ParseEnumeratedString(attribute, line, requireExists);
            if (stringValue != string.Empty)
            {
                return long.Parse(stringValue, CultureInfo.InvariantCulture);
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
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        /// <exception cref="FormatException">
        /// Thrown when parsing of the value fails.
        /// </exception>
        public static decimal ParseDecimal(string attribute, string line, bool requireExists)
        {
            string value = ParseEnumeratedString(attribute, line, requireExists);

            return value != string.Empty 
                ? decimal.Parse(value, DecimalParsingNumberStyles, CultureInfo.InvariantCulture) 
                : 0;
        }

        /// <summary>
        /// Parses the specified attribute hexadecimal value. If the parsed value is not long enough, 
        /// length specified by the <paramref name="bits"/>, zeroes ('0') are padded to the beginning
        /// of the value. The hex identifier prefix (0x) is removed.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">If set to <c>true</c> the <paramref name="attribute" /> is
        /// required to exist in the <paramref name="line" />.</param>
        /// <param name="bits">Number of bits. It is assumed that each 8 bits forms a byte.</param>
        /// <returns>
        /// The parsed value or default value or an empty string if the <paramref name="attribute"/>
        /// does not exist in the <paramref name="line"/>
        /// and the <paramref name="requireExists"/> is <b>false</b>.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the <paramref name="bits"/> is less than 8.
        /// </exception>
        public static string ParseHexadecimal(string attribute, string line, bool requireExists, int bits)
        {
            string value = ParseEnumeratedString(attribute, line, requireExists);

            if (value == string.Empty)
            {
                return string.Empty;
            }
            
            value = CreateHexValue(value, bits);
            return value;
        }

        /// <summary>
        /// Creates the hexadecimal value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bits">The bits.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Thrown when the <paramref name="value"/> is longer than value represented by <paramref name="bits"/>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the <paramref name="bits"/> is less than 8.
        /// </exception>
        /// <returns></returns>
        internal static string CreateHexValue(string value, int bits)
        {
            value.RequireNotNull("value");
            if (bits < SizeOfByte)
            {
                throw new ArgumentOutOfRangeException("bits", bits, "The bits parameter cannot be less than " + SizeOfByte + ".");
            }

            if (!value.StartsWith(HexPrefixIdentifier, StringComparison.OrdinalIgnoreCase))
            {
                throw new SerializationException("The hex value does not have prefix of " + HexPrefixIdentifier + ", case ignored.");
            }

            var prefix = value.Substring(0, HexPrefixIdentifier.Length);
            value = value.Substring(HexPrefixIdentifier.Length);
            int sizeInBytes = bits / SizeOfByte;
            if (value.Length > sizeInBytes)
            {
                throw new SerializationException("The value " + value + " to be parsed is longer than the number of bits " +
                                                 bits + ".");
            }

            value = prefix + value.PadLeft(sizeInBytes, '0');
            return value;
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
        /// The parsed value as a <see cref="Resolution"/> instance or <see cref="Resolution.Default" /> in case of missing attribute.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        /// <exception cref="FormatException">
        /// Thrown when parsing fails.
        /// </exception>
        public static Resolution ParseResolution(string attribute, string line, bool requireExists)
        {
            string value = ParseEnumeratedString(attribute, line, requireExists);
            var xPosition = value.IndexOf(Resolution.SeparatorChar, StringComparison.Ordinal);
            if (xPosition < 0)
            {
                if (requireExists)
                {
                    throw new SerializationException("Failed to parse resolution value from " + value);
                }
                return Resolution.Default;
            }
            try
            {
                var x = int.Parse(value.Substring(0, xPosition), NumberStyles.Integer, CultureInfo.InvariantCulture);
                var y = int.Parse(value.Substring(xPosition + 1), NumberStyles.Integer, CultureInfo.InvariantCulture);
                return new Resolution(x, y);
            }
            catch (Exception ex)
            {
                throw new FormatException("Failed to parse resolution value from " + value, ex);
            }
        }

        /// <summary>
        /// Parses the specified integer value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <exception cref="FormatException">
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
                throw new FormatException("Failed to parse attribute value " + line);
            }
            return value;
        }

        /// <summary>
        /// Parses the specified decimal value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <exception cref="FormatException">
        /// Thrown when the line cannot be parsed as a decimal.
        /// </exception>
        /// <returns>
        /// <see cref="Decimal"/> parsed from the <paramref name="line"/>.
        /// </returns>
        public static decimal ParseDecimal(string line)
        {
            decimal value;
            if (!decimal.TryParse(line, DecimalParsingNumberStyles, CultureInfo.InvariantCulture, out value))
            {
                throw new FormatException("Failed to parse attribute value " + line);
            }
            return value;
        }
    }
}
