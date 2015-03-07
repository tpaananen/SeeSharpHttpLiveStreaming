using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
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

        /// <summary>
        /// Gets the start position where the parsing starts.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">If set to <c>true</c> the attribute is required to exist in the <paramref name="line"/>.</param>
        /// <returns></returns>
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
        /// <exception cref="InvalidOperationException">Parser type mismatch. Expected typeparameter to be typeof string but was  + typeof(T) + .</exception>
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
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
            return strings.Length == 0 ? new List<T>() : strings.Select(parser).ToList();
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
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
        public static IList<string> ParseSeparatedQuotedString(string attribute, string line, 
                                                                  bool requireExists, char separator = ',')
        {
            var substring = ParseQuotedString(attribute, line, requireExists);
            if (substring == string.Empty)
            {
                return new List<string>();
            }
            var strings = substring.Split(separator);
            if (strings.Length == 0)
            {
                return new List<string>();
            }
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
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
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
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
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
        /// Thrown when parsing of the value fails.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the <paramref name="bits"/> is less than 8.
        /// </exception>
        public static string ParseHexadecimal(string attribute, string line, bool requireExists, int bits)
        {
            const int sizeOfByte = 8; // bits
            if (bits < sizeOfByte)
            {
                throw new ArgumentOutOfRangeException("bits", bits, "The bits parameter cannot be less than " + sizeOfByte + ".");
            }
            string value = ParseEnumeratedString(attribute, line, requireExists);
            if (value.StartsWith("0x"))
            {
                value = value.Substring(2);
            }

            int sizeInBytes = bits / sizeOfByte;
            if (value.Length > sizeInBytes)
            {
                throw new SerializationException("The value to be parsed is longer " + value.Length + " than the number of bits " + bits + ".");
            }

            value = value.PadLeft(sizeInBytes, '0');
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
        /// The parsed value (format ZZxYY or default value of <see cref="string.Empty" />.
        /// </returns>
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
        public static Resolution ParseResolution(string attribute, string line, bool requireExists)
        {
            string value = ParseEnumeratedString(attribute, line, requireExists);
            var xPosition = value.IndexOf("x", StringComparison.Ordinal);
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
                throw new FormatException("Failed to parse resolution value from " + line, ex);
            }
        }

        /// <summary>
        /// Parses the specified attribute value.
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
                throw new FormatException("Failed to parse attribute value " + line);
            }
            return value;
        }
    }
}
