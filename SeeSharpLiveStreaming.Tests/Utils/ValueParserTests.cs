using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;
using UriParser = SeeSharpHttpLiveStreaming.Utils.ValueParsers.UriParser;

namespace SeeSharpHttpLiveStreaming.Tests.Utils
{
    [TestFixture]
    public class ValueParserTests
    {
        [Test]
        public void TestParseQuotedString()
        {
            const string quotedString = "ATTRIBUTE=\"VALUE\"";
            var value = new QuotedStringParser().Parse("ATTRIBUTE", quotedString, false);
            Assert.AreEqual("VALUE", value);
        }

        [Test]
        public void TestParseQuotedStringReturnsEmptyString()
        {
            const string quotedString = "ATTRIBUTET=\"VALUE\"";
            var value = new QuotedStringParser().Parse("ATTRIBUTE", quotedString, false);
            Assert.AreEqual(string.Empty, value);
        }

        [Test]
        public void TestParseQuotedStringReturnsEmptyStringWhenNoEndQuote()
        {
            const string quotedString = "ATTRIBUTE=\"VALUE";
            var value = new QuotedStringParser().Parse("ATTRIBUTE", quotedString, false);
            Assert.AreEqual(string.Empty, value);
        }

        [Test]
        public void TestParseQuotedStringThrowsExceptionIfAttributeNotExistAndIsRequired()
        {
            const string quotedString = "ATTRIBUTET=\"VALUE\"";
            Assert.Throws<SerializationException>(() => new QuotedStringParser().Parse("ATTRIBUTE", quotedString, true));
        }

        [Test]
        public void TestParseSeparatedQuotedString()
        {
            const string quotedString = "ATTRIBUTE=\"VALUE,VALUE2\"";
            IList<string> actual = new StringWithSeparatorParser<string>(x => x).Parse("ATTRIBUTE", quotedString, false);
            CollectionAssert.AreEqual(new List<string> { "VALUE", "VALUE2"}, actual);
        }

        [Test]
        public void TestParseSeparatedQuotedStringReturnsEmptyListIfValueNotFound()
        {
            const string quotedString = "ATTRIBUTET=\"VALUE,VALUE2\"";
            IList<string> actual = new StringWithSeparatorParser<string>(x => x).Parse("ATTRIBUTE", quotedString, false);
            CollectionAssert.AreEqual(new List<string>(), actual);
        }

        [Test]
        public void TestParseSeparatedQuotedStringWithCustomSeparatorAndParser()
        {
            const string quotedString = "ATTRIBUTE=\"1/2/3/1002\"";
            IList<int> actual = new StringWithSeparatorParser<int>(int.Parse, '/').Parse("ATTRIBUTE", quotedString, false);
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 1002}, actual);
        }

        [Test]
        public void TestParseSeparatedQuotedStringReturnsEmptyList()
        {
            const string quotedString = "ATTRIBUTE=12121212";
            IList<int> actual = new StringWithSeparatorParser<int>(int.Parse).Parse("ATTRIBUTE", quotedString, false);
            CollectionAssert.AreEqual(new List<int>(), actual);
        }

        [Test]
        public void TestParseInt()
        {
            const int expected = 103443;
            var actual = new IntegerParser().Parse("ATTRIBUTE", "ATTRIBUTE=103443,SECOND=2121", false);
            Assert.AreEqual(expected, actual);

            actual = new IntegerParser().Parse("ATTRIBUTE", "FIRST=12121,ATTRIBUTE=103443", false);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestParseDecimal()
        {
            const decimal expected = 103443.21m;
            var actual = new DecimalParser().Parse("ATTRIBUTE", "ATTRIBUTE=103443.21,SECOND=2121", false);
            Assert.AreEqual(expected, actual);

            actual = new DecimalParser().Parse("ATTRIBUTE", "FIRST=12121,ATTRIBUTE=103443.21", false);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestParseDecimalWhenDoesNotExist()
        {
            var actual = new DecimalParser().Parse("ATTRIBUTET", "ATTRIBUTE=103443.21,SECOND=2121", false);
            Assert.AreEqual(0m, actual);
        }

        [Test]
        public void TestParseHexadecimal([Values("0x", "0X")] string prefix)
        {
            var actual = new HexParser(128).Parse("ATTRIBUTE", "ATTRIBUTE="+ prefix + "1212,SECOND=2121", false);
            Assert.AreEqual(HexParser.HexPrefixIdentifier + "1212".PadLeft(32, '0'), actual);

            actual = new HexParser(128).Parse("ATTRIBUTE", "FIRST=12121,ATTRIBUTE=" + prefix + "".PadLeft(32, 'F'), false);
            Assert.AreEqual(prefix + "".PadLeft(32, 'F'), actual);

            actual = new HexParser(128).Parse("ATTRIBUTET", "ATTRIBUTE=" + prefix + "0", false);
            Assert.AreEqual("", actual);

            actual = new HexParser(128).Parse("ATTRIBUTE", "ATTRIBUTE=" + prefix + "0", false);
            Assert.AreEqual(HexParser.HexPrefixIdentifier + "0".PadLeft(32, '0'), actual);
        }

        [Test]
        public void TestParseHexValueThrowsIfPrefixIsMissing()
        {
            Assert.Throws<SerializationException>(() => new HexParser(128).Parse("ATTRIBUTE", "ATTRIBUTE=FFFFFFFFFFFFFFFF", false));
        }

        [Test]
        public void TestParseHexadecimalFailsIfProvidedInputIsLongerThanGivenBits()
        {
            Assert.Throws<SerializationException>(() => new HexParser(128).Parse("ATTRIBUTE", "ATTRIBUTE=0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", false));
        }

        [Test]
        public void TestParseHexadecimalThrowsIfNumberOfBitsIsLessThanSizeOfByte()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new HexParser(HexParser.SizeOfChar - 1).Parse("ATTRIBUTE", "ATTRIBUTE=0x0", true));
        }

        [Test]
        public void TestParseResolution()
        {
            var expected = new Resolution(1920, 1080);
            var actual = new ResolutionParser().Parse("ATTRIBUTE", "ATTRIBUTE=1920x1080,SECOND=2121", false);
            Assert.AreEqual(expected, actual);

            actual = new ResolutionParser().Parse("ATTRIBUTE", "FIRST=12121,ATTRIBUTE=1920x1080", false);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestParseResolutionWithRequiredAndNotExist()
        {
            Assert.Throws<SerializationException>(() => new ResolutionParser().Parse("ATTRIBUTE", "ATTRIBUTET=1920x1080,SECOND=2121", true));
            Assert.Throws<SerializationException>(() => new ResolutionParser().Parse("ATTRIBUTE", "ATTRIBUTE=1920-1080,SECOND=2121", true));
        }

        [Test]
        public void TestParseResolutionThrowsFormatExceptionIfInputIsInvalid()
        {
            Assert.Throws<FormatException>(() => new ResolutionParser().Parse("ATTRIBUTE", "ATTRIBUTE=abcxdef,SECOND=2121", true));
        }

        [Test]
        public void TestCreateHexValueThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => HexParser.CreateHexValue(null, 128));
        }

        [Test]
        public void TestAbsoluteUriIsParsed()
        {
            const string uriString = "http://example.com/";
            var parser = new UriParser(new Uri(uriString));
            var uri = parser.Parse("URI", "URI=\"" + uriString + "\"", false);
            Assert.AreEqual(new Uri(uriString), uri);
        }

        [Test]
        public void TestRelativeUriIsParsed()
        {
            const string uriString = "http://example.com/";
            var parser = new UriParser(new Uri(uriString));
            var uri = parser.Parse("URI", "URI=\"file.m3u\"", false);
            Assert.AreEqual(new Uri(new Uri(uriString), "file.m3u"), uri);
        }

        [Test]
        public void TestUriParserThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new UriParser(null));
        }

        [Test]
        public void TestStringWithSeparatorParserThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StringWithSeparatorParser<string>(null));
        }
    }
}
