using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Media.MediaSegment
{
    [TestFixture]
    public class MapTests
    {
        private Map _map;

        [SetUp]
        public void SetUp()
        {
            _map = new Map();
            Assert.AreEqual("#EXT-X-MAP", _map.TagName);
            Assert.AreEqual(TagType.ExtXMap, _map.TagType);
        }

        [Test]
        public void TestMapThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _map.Deserialize(null, 0));
        }

        [Test]
        public void TestMapThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _map.Deserialize(string.Empty, 0));
        }

        [Test]
        public void TestMapIsParsed()
        {
            _map.Deserialize(GetLine(), 5);

            Assert.AreEqual("https://example.com/", _map.Uri.AbsoluteUri);
            Assert.AreEqual(new ByteRange(1231, 3232), _map.ByteRange);
        }

        [Test]
        public void TestIncompatibleVersionIsDetected()
        {
            Assert.Throws<SerializationException>(() => _map.Deserialize(GetLine(), 4));
        }

        private static string GetLine()
        {
            string value = "URI=\"https://example.com\"";
            value += "BYTERANGE=\"1231@3232\"";

            return value;
        }
    }
}
