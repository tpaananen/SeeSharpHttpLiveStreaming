using System;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.BasicTags;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags
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

        [Test]
        public void TestTagFactoryThrowsIfTagNameDoesNotExist()
        {
            Assert.Throws<NotSupportedException>(() => TagFactory.Create("INVALID-TAG"));
        }

        [Test]
        public void TestTagFactoryThrowsIfInvalidTagNameProvided()
        {
            Assert.Throws<InvalidOperationException>(() => TagFactory.ValidateAndAddTag("#INVALID-TAG", typeof(BaseTag)));
        }

        [Test]
        public void TestTagFactoryThrowsIfTheSameTagNameExistsTwice()
        {
            Assert.Throws<InvalidOperationException>(() => TagFactory.ValidateAndAddTag("#EXT-X-VERSION", typeof(ExtXVersion)));
        }
    }
}
