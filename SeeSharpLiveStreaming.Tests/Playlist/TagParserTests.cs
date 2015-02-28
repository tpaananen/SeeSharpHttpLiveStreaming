using System.IO;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpLiveStreaming.Utils;

namespace SeeSharpLiveStreaming.Tests.Playlist
{
    [TestFixture]
    public class TagParserTests
    {
        [Datapoints]
        private static string[] _lineFeeds = {"\r\n", "\n"};

        private static string GetPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000 http://example.com/low.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000 http://example.com/mid.m3u8" + lineFeed + lineFeed + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000 http://example.com/hi.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\" http://example.com/audio-only.m3u8" + lineFeed;
        }

        [Theory]
        public void TestReadFirstLine(string lineFeed)
        {
            var playlist = GetPlaylist(lineFeed);
            using (var reader = new StringReader(playlist))
            {
                TagParser.ReadFirstLine(reader);
            }
        }

        [Test]
        public void TestReadFirstLineThrowsException()
        {
            const string playlist = "ahskjshdjsgfhjgsjhgsgjshgdshjd\njshdjshdjshdjshdjshdjshdjsd";
            using (var reader = new StringReader(playlist))
            {
                Assert.Throws<SerializationException>(() => TagParser.ReadFirstLine(reader));
            }
        }

        [Theory]
        public void TestReadWhileNonEmptyLineReadsTheSecondLine(string lineFeed)
        {
            var playlist = GetPlaylist(lineFeed);
            var secondLine = TagParser.ReadWhileNonEmptyLine(playlist, 1);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000 http://example.com/low.m3u8", secondLine);
        }

        [Theory]
        public void TestReadWhileNonEmptyLineReadsTheLastLine(string lineFeed)
        {
            var playlist = GetPlaylist(lineFeed);
            var secondLine = TagParser.ReadWhileNonEmptyLine(playlist, 4);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\" http://example.com/audio-only.m3u8", secondLine);
        }

        [Test]
        public void TestParseTagReturnsEmptyString()
        {
            var tag = TagParser.ParseTag(null);
            Assert.AreEqual(string.Empty, tag);
            tag = TagParser.ParseTag(string.Empty);
            Assert.AreEqual(string.Empty, tag);
        }

        [Test]
        public void TestParseTagReturnEmptyStringIfNoTagEndMarkerIsFound()
        {
            var tag = TagParser.ParseTag("#EXT-X-STREAM-INF;BANDWIT");
            Assert.AreEqual(string.Empty, tag);
        }

        [Theory]
        public void TestParseTagReturnsEmptyStringIfUnknownTagIsFound(string lineFeed)
        {
            var line = "#EXT-X-STREAM-INT:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000 http://example.com/low.m3u8" + lineFeed;
            var tag = TagParser.ParseTag(line);
            Assert.AreEqual(string.Empty, tag);
        }

        [Theory]
        public void TestParseTagReturnsTagCorrectly(string lineFeed)
        {
            var line = "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000 http://example.com/low.m3u8" + lineFeed;
            var tag = TagParser.ParseTag(line);
            Assert.AreEqual("#EXT-X-STREAM-INF", tag);
        }
    }
}
