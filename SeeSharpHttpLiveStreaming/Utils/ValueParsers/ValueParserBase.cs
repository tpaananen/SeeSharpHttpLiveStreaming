using System;
using System.Runtime.Serialization;

namespace SeeSharpHttpLiveStreaming.Utils.ValueParsers
{
    /// <summary>
    /// Base class for value parsers that can parse attribute values from the playlist files.
    /// See https://tools.ietf.org/html/draft-pantos-http-live-streaming-14#section-4.2 for 
    /// details.
    /// </summary>
    internal abstract class ValueParserBase<T>
    {

        protected const char QuatationMark = '\"';

        /// <summary>
        /// Parses the specified attribute value from the string provided.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="fromString">The string to be parsed from.</param>
        /// <param name="requireExists">
        /// If set to <c>true</c> the <paramref name="attribute"/> is 
        /// required to exist in the <paramref name="fromString"/>.
        /// </param>
        /// <returns>
        /// The parsed value.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        public abstract T Parse(string attribute, string fromString, bool requireExists);

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
        protected static int StartPosition(string attribute, string line, bool requireExists)
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
        /// Parses the specified attribute value as either quoted or enumerated string.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">If set to <c>true</c> the <paramref name="attribute" /> is
        /// required to exist in the <paramref name="line" />.</param>
        /// <param name="quotedString">if set to <c>true</c> the attribute value is expected to be a quoted string.</param>
        /// <returns>
        /// The parsed value.
        /// </returns>
        /// <exception cref="SerializationException">Thrown when <paramref name="requireExists" /> is <b>true</b> and attribute is not found.</exception>
        protected static string ParseString(string attribute, string line, bool requireExists, bool quotedString)
        {
            var position = StartPosition(attribute, line, requireExists);
            if (position < 0)
            {
                return string.Empty;
            }

            int endPosition;
            if (quotedString && line[position] == QuatationMark)
            {
                endPosition = line.IndexOf("\"", position + 1, StringComparison.Ordinal);
                if (endPosition < 0)
                {
                    return string.Empty;
                }
                ++position;
            }
            else if (!quotedString)
            {
                endPosition = line.IndexOf(",", position, StringComparison.Ordinal);
            }
            else
            {
                return string.Empty;
            }

            if (position == endPosition)
            {
                return string.Empty;
            }

            var substring  = endPosition < position 
                ? line.Substring(position) 
                : line.Substring(position, endPosition - position);
            return substring;
        }
    }
}
