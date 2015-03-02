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
            var valueParser = new ValueParser();
            var value = valueParser.ParseQuotedString("ATTRIBUTE", quotedString, false);
            Assert.AreEqual("VALUE", value);
        }

        [Test]
        public void TestParseQuotedStringReturnsEmptyString()
        {
            const string quotedString = "ATTRIBUTET=\"VALUE\"";
            var valueParser = new ValueParser();
            var value = valueParser.ParseQuotedString("ATTRIBUTE", quotedString, false);
            Assert.AreEqual(string.Empty, value);
        }

        [Test]
        public void TestParseQuotedStringReturnsEmptyStringWhenNoEndQuote()
        {
            const string quotedString = "ATTRIBUTET=\"VALUE";
            var valueParser = new ValueParser();
            var value = valueParser.ParseQuotedString("ATTRIBUTE", quotedString, false);
            Assert.AreEqual(string.Empty, value);
        }

        [Test]
        public void TestParseQuotedStringThrowsExceptionIfAttributeNotExistsAndIsRequired()
        {
            const string quotedString = "ATTRIBUTET=\"VALUE\"";
            var valueParser = new ValueParser();
            Assert.Throws<SerializationException>(() => valueParser.ParseQuotedString("ATTRIBUTE", quotedString, true));
        }

    }
}
