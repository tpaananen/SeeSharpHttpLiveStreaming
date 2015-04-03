using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Loaders;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Loaders
{
    [TestFixture]
    public class PlaylistLoaderFactoryTests
    {
        [Test]
        public void TestPlaylistLoaderFactoryCreatesANewInstance()
        {
            var factory = new PlaylistLoaderFactory();
            var instance = factory.Create();
            Assert.IsNotNull(instance);
        }
    }
}
