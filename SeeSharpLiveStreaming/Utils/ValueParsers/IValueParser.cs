using System.Runtime.Serialization;

namespace SeeSharpLiveStreaming.Utils.ValueParsers
{
    /// <summary>
    /// Represents value parser interface.
    /// </summary>
    internal interface IValueParser
    {
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
        string ParseQuotedString(string attribute, string line, bool requireExists);

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
        int ParseInt(string attribute, string line, bool requireExists);

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
        decimal ParseDecimal(string attribute, string line, bool requireExists);

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
        double ParseDouble(string attribute, string line, bool requireExists);
    }
}
