using System;
using System.Globalization;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Utils.ValueParsers
{
    internal class ResolutionParser : ValueParserBase<Resolution>
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
        /// The parsed value as a <see cref="Resolution"/> instance or <see cref="Resolution.Default" /> in case of missing attribute.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        /// <exception cref="FormatException">
        /// Thrown when parsing fails.
        /// </exception>
        public override Resolution Parse(string attribute, string line, bool requireExists)
        {
            string value = ParseString(attribute, line, requireExists, false);
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

    }
}
