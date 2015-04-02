using System;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;
using SeeSharpHttpLiveStreaming.Tests.Helpers;

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
        public void TestMapIsParsedWithoutByteRange()
        {
            const string value = "URI=\"https://example.com\"";
            _map.Deserialize(value, 5);
            Assert.AreEqual(new ByteRange(), _map.ByteRange);
        }

        [Test]
        public void TestIncompatibleVersionIsDetected()
        {
            Assert.Throws<SerializationException>(() => _map.Deserialize(GetLine(), 4));
        }

        [Test]
        public void TestMapCtorThrowsArgumentNullException()
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            Assert.Throws<ArgumentNullException>(() => new Map(null, 0, 0));
        }

        [Test]
        public void TestSerializeSerializes([Values(0L, 121L)] long length)
        {
            var uri = new Uri("http://example.com/");
            var map = new Map(uri, length, 1024);
            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            map.Serialize(writer);

            var line = new PlaylistLine(map.TagName, sb.ToString());
            _map.Deserialize(line.GetParameters(), 5);

            Assert.AreEqual(map.Uri, _map.Uri);
            Assert.AreEqual(map.ByteRange, _map.ByteRange);
        }

        private static string GetLine()
        {
            string value = "URI=\"https://example.com\"";
            value += "BYTERANGE=\"1231@3232\"";

            return value;
        }
    }
}
