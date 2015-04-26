using System;
using System.Runtime.Serialization;

namespace SeeSharpHttpLiveStreaming.Utils.ValueParsers
{
    internal class HexParser : ValueParserBase<string>
    {

        internal const int SizeOfChar = 4; // hex 2 chars per byte
        internal const string HexPrefixIdentifier = "0x"; // case ignored

        private readonly int _bits;

        /// <summary>
        /// Initializes a new instance of the <see cref="HexParser"/> class.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the bits argument provided in constructor is less than <see cref="SizeOfChar"/>.
        /// </exception>
        internal HexParser(int bits)
        {
            if (bits < SizeOfChar)
            {
                throw new ArgumentOutOfRangeException("bits", bits, "The bits parameter cannot be less than " + SizeOfChar + ".");
            }
            _bits = bits;
        }

        /// <summary>
        /// Parses the specified attribute hexadecimal value. If the parsed value is not long enough, 
        /// length specified by the bits (in constructor), zeroes ('0') are padded to the beginning
        /// of the value. The hex identifier prefix (0x) is removed.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="line">The line.</param>
        /// <param name="requireExists">If set to <c>true</c> the <paramref name="attribute" /> is
        /// required to exist in the <paramref name="line" />.</param>
        /// <returns>
        /// The parsed value or default value or an empty string if the <paramref name="attribute"/>
        /// does not exist in the <paramref name="line"/>
        /// and the <paramref name="requireExists"/> is <b>false</b>.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Thrown when <paramref name="requireExists"/> is <b>true</b> and attribute is not found.
        /// </exception>
        public override string Parse(string attribute, string line, bool requireExists)
        {
            string value = ParseString(attribute, line, requireExists, false);

            if (value == string.Empty)
            {
                return string.Empty;
            }
            
            value = CreateHexValue(value, _bits);
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
        /// Thrown when the <paramref name="value"/> is greater than value represented by <paramref name="bits"/>
        /// </exception>
        /// <returns>
        /// The hex value
        /// </returns>
        internal static string CreateHexValue(string value, int bits)
        {
            value.RequireNotNull("value");

            if (!value.StartsWith(HexPrefixIdentifier, StringComparison.OrdinalIgnoreCase))
            {
                throw new SerializationException("The hex value does not have prefix of " + HexPrefixIdentifier + ", case ignored.");
            }

            int length = bits / SizeOfChar;
            if (value.Length - HexPrefixIdentifier.Length != length)
            {
                if (value.Length - HexPrefixIdentifier.Length > length)
                {
                    throw new SerializationException("The value " + value + 
                        " to be parsed is greater than the number of bits " +
                        bits + ".");
                }

                value = value.Substring(HexPrefixIdentifier.Length);
                value = HexPrefixIdentifier + value.PadLeft(length, '0');
            }
            return value;
        }
    }
}
