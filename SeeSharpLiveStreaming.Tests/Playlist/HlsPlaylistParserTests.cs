using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SeeSharpLiveStreaming.Playlist;
using SeeSharpLiveStreaming.Playlist.Tags;

namespace SeeSharpLiveStreaming.Tests.Playlist
{
    /// <summary>
    /// Playlist examples: https://tools.ietf.org/html/draft-pantos-http-live-streaming-14#section-8
    /// </summary>
    [TestFixture]
    public class HlsPlaylistParserTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        private static string CreateValidMediaPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-TARGETDURATION:10" + lineFeed +
                "" + lineFeed +
                "#EXTINF:9.009," +
                "http://media.example.com/first.ts" + lineFeed +
                "#EXTINF:9.009," +
                "http://media.example.com/second.ts" + lineFeed +
                "#EXTINF:3.003," +
                "http://media.example.com/third.ts" + lineFeed;
        }

        private static string CreateValidMasterPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000 http://example.com/low.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000 http://example.com/mid.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000 http://example.com/hi.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\" http://example.com/audio-only.m3u8" + lineFeed;
        }

        [Datapoints]
        public static string[] NewLines = { "\r\n", "\n" };

        [Theory]
        public void TestParserCreatesMasterPlaylist(string newLine)
        {
            var playlist = CreateValidMasterPlaylist(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist);

            Assert.IsNotNull(playlistObject);
            Assert.That(playlistObject is HlsPlaylist);
            Assert.IsNotNull(playlistObject.Playlist);
            Assert.That(playlistObject.Playlist is MasterPlaylist);
        }

        [Theory]
        public void TestParserCreatesMediaPlaylist(string newLine)
        {
            var playlist = CreateValidMediaPlaylist(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist);

            Assert.IsNotNull(playlistObject);
            Assert.That(playlistObject is HlsPlaylist);
            Assert.IsNotNull(playlistObject.Playlist);
            Assert.That(playlistObject.Playlist is MediaPlaylist);
        }
    }
}
