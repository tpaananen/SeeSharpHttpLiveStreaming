using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace SeeSharpLiveStreaming.Utils.ValueParsers
{
    /// <summary>
    /// Base class for value parsers.
    /// </summary>
    internal class ValueParser : IValueParser
    {

        /// <summary>
        /// Gets the start position where the parsing starts.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">If set to <c>true</c> the attribute is required to exist in the <paramref name="line"/>.</param>
        /// <returns></returns>
        protected int StartPosition(string attribute, string line, bool requireExists)
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

        protected string GetUnquotedStringToConvert(string line)
        {
            return null;
        }

        /// <summary>
        /// Parses the specified attribute value.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">
        /// If set to <c>true</c> the <paramref name="attribute"/> is 
        /// required exist in the <paramref name="line"/>.
        /// </param>
        /// <returns>
        /// The parsed value.
        /// </returns>
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
        public string ParseQuotedString(string attribute, string line, bool requireExists)
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
        /// required exist in the <paramref name="line"/>.
        /// </param>
        /// <returns>
        /// The parsed value.
        /// </returns>
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
        public int ParseInt(string attribute, string line, bool requireExists)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses the specified attribute value.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">
        /// If set to <c>true</c> the <paramref name="attribute"/> is 
        /// required exist in the <paramref name="line"/>.
        /// </param>
        /// <returns>
        /// The parsed value.
        /// </returns>
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
        public decimal ParseDecimal(string attribute, string line, bool requireExists)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses the specified attribute value.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">
        /// If set to <c>true</c> the <paramref name="attribute"/> is 
        /// required exist in the <paramref name="line"/>.
        /// </param>
        /// <returns>
        /// The parsed value.
        /// </returns>
        /// <exception cref="SerializationException">Thrown when parsing of the value fails.</exception>
        public double ParseDouble(string attribute, string line, bool requireExists)
        {
            throw new NotImplementedException();
        }
    }
}
