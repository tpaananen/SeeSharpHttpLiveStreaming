using System.IO;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpLiveStreaming.Playlist.Tags;

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

        [Theory]
        public void TestTagParserCreatesPlaylistCollection(string lineFeed)
        {
            var playlist = GetPlaylist(lineFeed);
            var list = TagParser.ReadLines(playlist);

            Assert.IsNotNull(list);
            Assert.AreEqual(4, list.Count);
            
            Assert.AreEqual("#EXT-X-STREAM-INF", list[0].Tag);
            Assert.AreEqual("#EXT-X-STREAM-INF", list[1].Tag);
            Assert.AreEqual("#EXT-X-STREAM-INF", list[2].Tag);
            Assert.AreEqual("#EXT-X-STREAM-INF", list[3].Tag);

            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000 http://example.com/low.m3u8", list[0].Line);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000 http://example.com/mid.m3u8", list[1].Line);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000 http://example.com/hi.m3u8", list[2].Line);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\" http://example.com/audio-only.m3u8", list[3].Line);

        }
    }
}
