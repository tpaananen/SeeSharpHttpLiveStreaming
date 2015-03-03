using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpLiveStreaming.Utils.ValueParsers;

namespace SeeSharpLiveStreaming.Tests.Utils
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
            const string quotedString = "ATTRIBUTET=\"VALUE";
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
        public void TestParseCommaSeparatedQuotedString()
        {
            const string quotedString = "ATTRIBUTE=\"VALUE,VALUE2\"";
            var actual = ValueParser.ParseCommaSeparatedQuotedString("ATTRIBUTE", quotedString, false);
            CollectionAssert.AreEqual(new List<string> { "VALUE", "VALUE2"}, actual);
        }

        [Test]
        public void TestParseCommaSeparatedQuotedStringReturnsEmptyListIfValueNotFound()
        {
            const string quotedString = "ATTRIBUTET=\"VALUE,VALUE2\"";
            var actual = ValueParser.ParseCommaSeparatedQuotedString("ATTRIBUTE", quotedString, false);
            CollectionAssert.AreEqual(new List<string>(), actual);
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
        public void TestParseDouble()
        {
            const double expected = 103443.21;
            var actual = ValueParser.ParseDouble("ATTRIBUTE", "ATTRIBUTE=103443.21,SECOND=2121", false);
            Assert.AreEqual(expected, actual);

            actual = ValueParser.ParseDouble("ATTRIBUTE", "FIRST=12121,ATTRIBUTE=103443.21", false);
            Assert.AreEqual(expected, actual);
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
        public void TestParseDoubleValue()
        {
            const double expected = 103443.21;
            var actual = ValueParser.ParseDouble("103443.21");
            Assert.AreEqual(expected, actual);
        }
    }
}
