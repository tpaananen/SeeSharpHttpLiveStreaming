using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags
{
    [TestFixture]
    public class TestExtXDiscontinuity
    {
        private Discontinuity _discontinuity;

        [SetUp]
        public void SetUp()
        {
            _discontinuity = new Discontinuity();
            Assert.AreEqual("#EXT-X-DISCONTINUITY", _discontinuity.TagName);
            Assert.AreEqual(TagType.ExtXDiscontinuity, _discontinuity.TagType);
        }

        [Test]
        public void TestExtXDiscontinuityIsCreated()
        {
            Assert.IsNotNull(_discontinuity);
        }

        [Test]
        public void TestExtXDiscontinuityDeserializeDoesNotThrowArgumeNullException()
        {
            _discontinuity.Deserialize(null, 0);
        }

        [Test]
        public void TestExtXDiscontinuityDeserializeDoesNotThrowArgumeException()
        {
            _discontinuity.Deserialize(string.Empty, 0);
        }

        [Test]
        public void TestExtXDiscontinuityThrowsSerializationExceptionIfAttributesProvided()
        {
            Assert.Throws<SerializationException>(() => _discontinuity.Deserialize("asasas", 0));
        }

    }

    
}
