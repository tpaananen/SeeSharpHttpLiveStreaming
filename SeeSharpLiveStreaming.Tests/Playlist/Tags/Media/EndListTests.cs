using System;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Media
{
    [TestFixture]
    public class EndListTests
    {

        private EndList _endList;

        [SetUp]
        public void SetUp()
        {
            _endList = new EndList();
        }

        [Test]
        public void TestEndListIsCreated()
        {
            Assert.AreEqual("#EXT-X-ENDLIST", _endList.TagName);
            Assert.AreEqual(TagType.ExtXEndList, _endList.TagType);
        }

        [Test]
        public void TestEndListCanBeDeserializedWithNullContent()
        {
            _endList.Deserialize(null, 0);
        }

        [Test]
        public void TestEndListCanBeDeserializedWithEmptyContent()
        {
            _endList.Deserialize(string.Empty, 0);
        }

        [Test]
        public void TestEndListThrowsArgumentExceptionIfAttributesAreProvided()
        {
            Assert.Throws<ArgumentException>(() => _endList.Deserialize("1212", 0));
        }
    }
}
