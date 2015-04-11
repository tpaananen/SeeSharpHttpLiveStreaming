using System;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    [TestFixture]
    public class PlaylistLineTests
    {
        private static readonly Uri Uri = new Uri("http://example.com/low.m3u8");

        [Test]
        public void TestPlaylistLineGetParameters()
        {
            var playlistLine = new PlaylistLine("#EXT-X-STREAM-INF", "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000", Uri);
            var parameters = playlistLine.GetParameters();
            Assert.AreEqual("BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000", parameters);
            Assert.AreEqual(new Uri("http://example.com/low.m3u8"), playlistLine.Uri);
        }

        [Test]
        public void TestPlaylistLineGetParametersReturnsEmptyStringIfThereIsNoParameters()
        {
            var playlistLine = new PlaylistLine(Tag.StartLine, Tag.StartLine);
            var parameters = playlistLine.GetParameters();
            Assert.AreEqual(string.Empty, parameters);
        }

        [Test]
        public void TestPlaylistLineCreationFailsIfTheLineDoesNotStartWithTheTag()
        {
            Assert.Throws<ArgumentException>(() => new PlaylistLine("#EXT-X-INF", "onlyvalue"));
        }

        [Test]
        public void TestPlaylistLineCreationFailsIfUriIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PlaylistLine("#EXT-X-INF", "#EXT-X-INF:value", null));
        }

        [Test]
        public void TestPlaylistLinesAreEqualNoUri()
        {
            var first = new PlaylistLine("1234", "1234");
            var second = new PlaylistLine("1234", "1234");
            Assert.AreEqual(first, second);
            Assert.That(first == second);
        }

        [Test]
        public void TestPlaylistLinesAreEqualWithUri()
        {
            var first = new PlaylistLine("1234", "1234", Uri);
            var second = new PlaylistLine("1234", "1234", Uri);
            Assert.AreEqual(first, second);
            Assert.That(first == second);
            Assert.That(first.Equals((object)second));
        }

        [Test]
        public void TestPlaylistLinesAreNotEqualNoUri()
        {
            var first = new PlaylistLine("1234", "123433");
            var second = new PlaylistLine("1234", "12343");
            Assert.That(first != second);
            Assert.That(!first.Equals((object)second));
        }

        [Test]
        public void TestPlaylistLinesAreNotEqualWithUri()
        {
            var first = new PlaylistLine("1234", "1234", Uri);
            var second = new PlaylistLine("1234", "1234", new Uri("http://localhost/"));
            Assert.That(first != second);
        }

        [Test]
        public void TestPlaylistLineDoesNotEqualToNull()
        {
            var first = new PlaylistLine("1234", "1234", Uri);
            Assert.IsFalse(first.Equals(null));
            Assert.IsFalse(first.Equals(new object()));
        }

        [Test]
        public void TestPlaylistLineRemovesLineEnding([Values("\r\n", "\n")] string lineEnding)
        {
            var playlistLine = new PlaylistLine("#EXT-X-STREAM-INF", "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000" + lineEnding, Uri);
            Assert.IsFalse(playlistLine.Line.EndsWith(lineEnding));
        }
    }
}
