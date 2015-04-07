using System;
using System.Collections.Generic;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using Version = SeeSharpHttpLiveStreaming.Playlist.Tags.BasicTags.Version;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags
{
    [TestFixture]
    public class TagFactoryTests
    {

        [Test]
        public void TestBaseTagCreateThrowsIfStartTagIsTriedToCreate()
        {
            var line = new PlaylistLine(Tag.StartLine, Tag.StartLine);
            Assert.Throws<InvalidOperationException>(() => TagFactory.Create(line, 0));
        }

        [Test]
        public void TestTagFactoryCreatesTag()
        {
            var tag = TagFactory.Create("#EXT-X-VERSION");
            Assert.AreEqual(typeof(Version), tag.GetType());
        }

        [Test]
        public void TestTagFactoryCreatesTagFromLine()
        {
            var line = new PlaylistLine("#EXT-X-VERSION", "#EXT-X-VERSION:3");
            var tag = TagFactory.Create(line, Version.InitialVersionNumber);
            Assert.AreEqual(typeof(Version), tag.GetType());
            Assert.AreEqual(3, ((Version)tag).VersionNumber);
            Assert.IsTrue(tag.UsingDefaultCtor);
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
            var creator = TagFactory.CreateConstructor(typeof (Version));
            Assert.Throws<InvalidOperationException>(() => TagFactory.ValidateAndAddTag("#INVALID-TAG", creator, new Dictionary<string, Func<BaseTag>>()));
        }

        [Test]
        public void TestTagFactoryThrowsIfTheSameTagNameExistsTwice()
        {
            var creator = TagFactory.CreateConstructor(typeof (Version));
            var container = new Dictionary<string, Func<BaseTag>>();
            TagFactory.ValidateAndAddTag("#EXT-X-VERSION", creator, container);
            Assert.Throws<InvalidOperationException>(() => TagFactory.ValidateAndAddTag("#EXT-X-VERSION", creator, container));
        }
    }
}
