using System;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    [TestFixture]
    public class PlaylistFactoryTests : PlaylistReadingTestBase
    {
        private static readonly Uri _uri = new Uri("http://localhost/hello/world/playlist.m3u8");

        [Test]
        public void TestPlaylistFactoryThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => PlaylistFactory.Create(null, _uri));
            Assert.Throws<ArgumentNullException>(() => PlaylistFactory.Create(Tag.StartLine, null));
        }

        [Test]
        public void TestPlaylistFactoryThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PlaylistFactory.Create(string.Empty, _uri));
        }

        [Test]
        public void TestPlaylistFactoryReadsPlaylist()
        {
            var content = CreateValidMasterPlaylist(Environment.NewLine);
            var playlist = PlaylistFactory.Create(content, _uri);
            Assert.IsNotNull(playlist);
        }
    }
}
