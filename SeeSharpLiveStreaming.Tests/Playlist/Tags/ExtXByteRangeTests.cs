using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpLiveStreaming.Tests.Playlist.Tags
{
    [TestFixture]
    public class ExtXByteRangeTests
    {
        private ExtXByteRange _byteRange;

        [SetUp]
        public void SetUp()
        {
            _byteRange = new ExtXByteRange();
            Assert.AreEqual("#EXT-X-BYTERANGE", _byteRange.TagName);
            Assert.AreEqual(TagType.ExtXByteRange, _byteRange.TagType);
        }

        [Test]
        public void TestParseExtXByteRangeWithStartIndex()
        {
            const string validValue = "11211@1212";
            _byteRange.Deserialize(validValue, 7);
            Assert.AreEqual(11211, _byteRange.Length);
            Assert.AreEqual(1212, _byteRange.StartIndex);
        }

        [Test]
        public void TestParseExtXByteRangeWithoutStartIndex()
        {
            const string validValue = "11211";
            _byteRange.Deserialize(validValue, 7);
            Assert.AreEqual(11211, _byteRange.Length);
            Assert.AreEqual(0, _byteRange.StartIndex);
        }

        [Test]
        public void TestParseExtXByteRangeRefusesToParseIfVersionIsBelowFour()
        {
            const string validValue = "11211";
            Assert.Throws<IncompatibleVersionException>(() => _byteRange.Deserialize(validValue, 3));
        }

        [Test]
        public void TestParseExtXByteRangeFailsWithInvalidInput()
        {
            const string invalidValue = "11211@1212@121";
            Assert.Throws<SerializationException>(() => _byteRange.Deserialize(invalidValue, 7));
        }

        [Test]
        public void TestParseExtXByteRangeFailsWithEmptyInput()
        {
            Assert.Throws<ArgumentException>(() => _byteRange.Deserialize(string.Empty, 7));
        }

        [Test]
        public void TestParseExtXByteRangeFailsWithNullInput()
        {
            Assert.Throws<ArgumentNullException>(() => _byteRange.Deserialize(null, 7));
        }
    }
}
