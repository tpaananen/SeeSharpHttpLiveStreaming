using System;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags
{
    [TestFixture]
    public class BaseTagTests
    {

        [Test]
        public void TestBaseTagCreateThrowsIfStartTagIsTriedToCreate()
        {
            var line = new PlaylistLine(Tag.StartLine, Tag.StartLine);
            Assert.Throws<InvalidOperationException>(() => BaseTag.Create(line, 0));
        }

    }
}
