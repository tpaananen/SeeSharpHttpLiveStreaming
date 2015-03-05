using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.BasicTags;

namespace SeeSharpLiveStreaming.Tests.Playlist.Tags
{
    [TestFixture]
    public class TagFactoryTests
    {

        [Test]
        public void TestTagFactoryCreatesTag()
        {
            var tag = TagFactory.Create("#EXT-X-VERSION");
            Assert.AreEqual(typeof(ExtXVersion), tag.GetType());
        }

        [Test]
        public void TestTagFactoryReturnsNewInstanceEachCall()
        {
            var tag = TagFactory.Create("#EXT-X-VERSION");
            var tag2 = TagFactory.Create("#EXT-X-VERSION");
            Assert.AreNotEqual(tag, tag2);
        }

    }
}
