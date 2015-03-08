using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;

namespace SeeSharpHttpLiveStreaming.Tests.Utils
{
    [TestFixture]
    public class ValueParserTests
    {
        [Test]
        public void TestParseQuotedString()
        {
            const string quotedString = "ATTRIBUTE=\"VALUE\"";
            var value = ValueParser.ParseQuotedString("ATTRIBUTE", quotedString, false);
            Assert.AreEqual("VALUE", value);
        }

        [Test]
        public void TestParseQuotedStringReturnsEmptyString()
        {
            const string quotedString = "ATTRIBUTET=\"VALUE\"";
            var value = ValueParser.ParseQuotedString("ATTRIBUTE", quotedString, false);
            Assert.AreEqual(string.Empty, value);
        }

        [Test]
        public void TestParseQuotedStringReturnsEmptyStringWhenNoEndQuote()
        {
            const string quotedString = "ATTRIBUTE=\"VALUE";
            var value = ValueParser.ParseQuotedString("ATTRIBUTE", quotedString, false);
            Assert.AreEqual(string.Empty, value);
        }

        [Test]
        public void TestParseQuotedStringThrowsExceptionIfAttributeNotExistsAndIsRequired()
        {
            const string quotedString = "ATTRIBUTET=\"VALUE\"";
            Assert.Throws<SerializationException>(() => ValueParser.ParseQuotedString("ATTRIBUTE", quotedString, true));
        }

        [Test]
        public void TestParseSeparatedQuotedString()
        {
            const string quotedString = "ATTRIBUTE=\"VALUE,VALUE2\"";
            var actual = ValueParser.ParseSeparatedQuotedString("ATTRIBUTE", quotedString, false);
            CollectionAssert.AreEqual(new List<string> { "VALUE", "VALUE2"}, actual);
        }

        [Test]
        public void TestParseSeparatedQuotedStringReturnsEmptyListIfValueNotFound()
        {
            const string quotedString = "ATTRIBUTET=\"VALUE,VALUE2\"";
            var actual = ValueParser.ParseSeparatedQuotedString("ATTRIBUTE", quotedString, false);
            CollectionAssert.AreEqual(new List<string>(), actual);
        }

        [Test]
        public void TestParseSeparatedQuotedStringWithCustomSeparatorAndParser()
        {
            const string quotedString = "ATTRIBUTE=\"1/2/3/1002\"";
            var actual = ValueParser.ParseSeparatedQuotedString("ATTRIBUTE", quotedString, false, int.Parse, '/');
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 1002}, actual);
        }

        [Test]
        public void TestParseSeparatedQuotedStringReturnsEmptyList()
        {
            const string quotedString = "ATTRIBUTE=12121212";
            var actual = ValueParser.ParseSeparatedQuotedString("ATTRIBUTE", quotedString, false, int.Parse);
            CollectionAssert.AreEqual(new List<int>(), actual);
        }

        [Test]
        public void TestParseInt()
        {
            const int expected = 103443;
            var actual = ValueParser.ParseInt("ATTRIBUTE", "ATTRIBUTE=103443,SECOND=2121", false);
            Assert.AreEqual(expected, actual);

            actual = ValueParser.ParseInt("ATTRIBUTE", "FIRST=12121,ATTRIBUTE=103443", false);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestParseDecimal()
        {
            const decimal expected = 103443.21m;
            var actual = ValueParser.ParseDecimal("ATTRIBUTE", "ATTRIBUTE=103443.21,SECOND=2121", false);
            Assert.AreEqual(expected, actual);

            actual = ValueParser.ParseDecimal("ATTRIBUTE", "FIRST=12121,ATTRIBUTE=103443.21", false);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestParseDecimalWhenDoesNotExist()
        {
            var actual = ValueParser.ParseDecimal("ATTRIBUTET", "ATTRIBUTE=103443.21,SECOND=2121", false);
            Assert.AreEqual(0m, actual);
        }

        [Test]
        public void TestParseHexadecimal()
        {
            var actual = ValueParser.ParseHexadecimal("ATTRIBUTE", "ATTRIBUTE=0x1212,SECOND=2121", false, 128);
            Assert.AreEqual("1212".PadLeft(16, '0'), actual);

            actual = ValueParser.ParseHexadecimal("ATTRIBUTE", "FIRST=12121,ATTRIBUTE=0x1212", false, 128);
            Assert.AreEqual("1212".PadLeft(16, '0'), actual);

            actual = ValueParser.ParseHexadecimal("ATTRIBUTET", "ATTRIBUTE=0x0", false, 128);
            Assert.AreEqual("", actual);

            actual = ValueParser.ParseHexadecimal("ATTRIBUTE", "ATTRIBUTE=0x0", false, 128);
            Assert.AreEqual("0".PadLeft(16, '0'), actual);

            actual = ValueParser.ParseHexadecimal("ATTRIBUTE", "ATTRIBUTE=FFFFFFFFFFFFFFFF", false, 128);
            Assert.AreEqual("FFFFFFFFFFFFFFFF", actual);
        }

        [Test]
        public void TestParseHexadecimalFailsIfProvidedInputIsLongerThanGivenBits()
        {
            Assert.Throws<SerializationException>(() => ValueParser.ParseHexadecimal("ATTRIBUTE", "ATTRIBUTE=FFFFFFFFFFFFFFFFF", false, 128));
        }

        [Test]
        public void TestParseHexadecimalThrowsIfNumberOfBitsIsLessThanSizeOfByte()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ValueParser.ParseHexadecimal("ATTRIBUTE", "ATTRIBUTE=0x0", true, sizeof (byte) - 1));
        }

        [Test]
        public void TestParseResolution()
        {
            var expected = new Resolution(1920, 1080);
            var actual = ValueParser.ParseResolution("ATTRIBUTE", "ATTRIBUTE=1920x1080,SECOND=2121", false);
            Assert.AreEqual(expected, actual);

            actual = ValueParser.ParseResolution("ATTRIBUTE", "FIRST=12121,ATTRIBUTE=1920x1080", false);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestParseResolutionWithRequiredAndNotExists()
        {
            Assert.Throws<SerializationException>(() => ValueParser.ParseResolution("ATTRIBUTE", "ATTRIBUTET=1920x1080,SECOND=2121", true));
            Assert.Throws<SerializationException>(() => ValueParser.ParseResolution("ATTRIBUTE", "ATTRIBUTE=1920-1080,SECOND=2121", true));
        }

        [Test]
        public void TestParseResolutionThrowsFormatExceptionIfInputIsInvalid()
        {
            Assert.Throws<FormatException>(() => ValueParser.ParseResolution("ATTRIBUTE", "ATTRIBUTE=abcxdef,SECOND=2121", true));
        }

        [Test]
        public void TestParseIntValue()
        {
            const int expected = 103443;
            var actual = ValueParser.ParseInt("103443");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestParseDecimalValue()
        {
            const decimal expected = 103443.21m;
            var actual = ValueParser.ParseDecimal("103443.21");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestParseDecimalValueFails()
        {
            Assert.Throws<FormatException>(() => ValueParser.ParseDecimal("123x1080"));
        }
    }
}
