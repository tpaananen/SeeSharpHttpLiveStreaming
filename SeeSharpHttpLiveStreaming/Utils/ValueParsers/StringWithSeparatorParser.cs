using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SeeSharpHttpLiveStreaming.Utils.ValueParsers
{
    internal class StringWithSeparatorParser<T> : ValueParserBase<IList<T>>
    {

        private readonly Func<string, T> _parser;
        private readonly char _separator;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringWithSeparatorParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="separator">The separator.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="parser"/> is <b>null</b>.
        /// </exception>
        internal StringWithSeparatorParser(Func<string, T> parser, char separator = ',')
        {
            parser.RequireNotNull("parser");
            _parser = parser;
            _separator = separator;
        } 

        /// <summary>
        /// Parses the specified attribute value.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">If set to <c>true</c> the <paramref name="attribute" /> is
        /// required to exist in the <paramref name="line" />.</param>
        /// <returns>
        /// The parsed value in a list or an empty list.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        public override IList<T> Parse(string attribute, string line, bool requireExists)
        {
            string substring = ParseString(attribute, line, requireExists, true);
            if (substring == string.Empty)
            {
                return new List<T>();
            }
            var strings = substring.Split(_separator);
            return strings.Select(_parser).ToList();
        }
    }
}
