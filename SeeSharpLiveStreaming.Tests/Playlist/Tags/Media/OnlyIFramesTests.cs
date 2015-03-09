using System;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Media
{
    [TestFixture]
    public class OnlyIFramesTests
    {
        private OnlyIFrames _frames;

        [SetUp]
        public void SetUp()
        {
            _frames = new OnlyIFrames();
            
        }

        [Test]
        public void TestOnlyIFramesTagIsCreated([Values((string)null, "")] string value)
        {
            _frames.Deserialize(value, 4);
            Assert.AreEqual("#EXT-X-I-FRAMES-ONLY", _frames.TagName);
            Assert.AreEqual(TagType.ExtXIFramesOnly, _frames.TagType);
        }

        [Test]
        public void TestOnlyIFramesFailsToDeserializeIfAttributesProvided()
        {
            Assert.Throws<ArgumentException>(() => _frames.Deserialize("2323", 4));
        }

        [Test]
        public void TestOnlyIFramesFailsToParseIfRequiredVersionIsBelowFour()
        {
            Assert.Throws<IncompatibleVersionException>(() => _frames.Deserialize("", 3));
        }
    }
}
