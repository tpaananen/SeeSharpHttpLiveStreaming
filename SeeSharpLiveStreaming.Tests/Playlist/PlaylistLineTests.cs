using System;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    [TestFixture]
    public class PlaylistLineTests
    {
        [Test]
        public void TestPlaylistLineGetParameters()
        {
            var playlistLine = new PlaylistLine("#EXT-X-STREAM-INF", "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000", " http://example.com/low.m3u8");
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

    }
}
