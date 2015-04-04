using System;
using System.Collections.Generic;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    [TestFixture]
    public class PlaylistFactoryTests : PlaylistReadingTestBase
    {

        [Test]
        public void TestPlaylistFactoryThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => PlaylistFactory.Create(null));
        }

        [Test]
        public void TestPlaylistFactoryThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PlaylistFactory.Create(string.Empty));
        }

        [Test]
        public void TestPlaylistFactoryReadsPlaylist()
        {
            var content = CreateValidMasterPlaylist(Environment.NewLine);
            var playlist = PlaylistFactory.Create(content);
            Assert.IsNotNull(playlist);
        }
    }
}
