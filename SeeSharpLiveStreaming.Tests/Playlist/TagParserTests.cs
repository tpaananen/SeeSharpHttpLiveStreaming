using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    [TestFixture]
    public class TagParserTests
    {
        [Datapoints]
        // ReSharper disable once UnusedMember.Local
        private static string[] _lineFeeds = {"\r\n", "\n"};

        private static string GetPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000" + lineFeed + "http://example.com/low.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000" + lineFeed + lineFeed + "http://example.com/mid.m3u8" + lineFeed + lineFeed + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000" + lineFeed + "http://example.com/hi.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\"" + lineFeed + "http://example.com/audio-only.m3u8" + lineFeed;
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
            var line = "#EXT-X-STREAM-INT:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000" + lineFeed + "http://example.com/low.m3u8" + lineFeed;
            var tag = TagParser.ParseTag(line);
            Assert.AreEqual(string.Empty, tag);
        }

        [Theory]
        public void TestParseTagReturnsTagCorrectly(string lineFeed)
        {
            var line = "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000" + lineFeed + "http://example.com/low.m3u8" + lineFeed;
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
            
            Assert.AreEqual("#EXT-X-STREAM-INF", list.ElementAt(0).Tag);
            Assert.AreEqual("#EXT-X-STREAM-INF", list.ElementAt(1).Tag);
            Assert.AreEqual("#EXT-X-STREAM-INF", list.ElementAt(2).Tag);
            Assert.AreEqual("#EXT-X-STREAM-INF", list.ElementAt(3).Tag);

            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000", list.ElementAt(0).Line);
            Assert.AreEqual(new Uri("http://example.com/low.m3u8"), list.ElementAt(0).Uri);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000", list.ElementAt(1).Line);
            Assert.AreEqual(new Uri("http://example.com/mid.m3u8"), list.ElementAt(1).Uri);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000", list.ElementAt(2).Line);
            Assert.AreEqual(new Uri("http://example.com/hi.m3u8"), list.ElementAt(2).Uri);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\"", list.ElementAt(3).Line);
            Assert.AreEqual(new Uri("http://example.com/audio-only.m3u8"), list.ElementAt(3).Uri);
        }
    }
}
