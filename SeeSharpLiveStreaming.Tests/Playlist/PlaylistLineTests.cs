using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SeeSharpLiveStreaming.Playlist.Tags;

namespace SeeSharpLiveStreaming.Tests.Playlist
{
    [TestFixture]
    public class PlaylistLineTests
    {
        [Test]
        public void TestPlaylistLineGetParameters()
        {
            var playlistLine = new PlaylistLine("#EXT-X-STREAM-INF", "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000 http://example.com/low.m3u8");
            var parameters = playlistLine.GetParameters();
            Assert.AreEqual("BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000 http://example.com/low.m3u8", parameters);
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
