using System;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.MasterOrMedia;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.MasterOrMedia
{
    [TestFixture]
    public class IndependentSegmentsTests
    {
        private IndependentSegments _segments;

        [SetUp]
        public void SetUp()
        {
            _segments = new IndependentSegments();
            Assert.AreEqual("#EXT-X-INDEPENDENT-SEGMENTS", _segments.TagName);
            Assert.AreEqual(TagType.ExtXIndependentSegments, _segments.TagType);
        }

        [Test]
        public void TestIndependentSegmentsThrowArgumentExceptionIfItHasAttributes()
        {
            Assert.Throws<ArgumentException>(() => _segments.Deserialize("TYPE=asasa", 0));
        }

        [Test]
        public void TestIndependentSegmentsIsCreated([Values("", (string) null)] string value)
        {
            _segments.Deserialize(value, 0);
        }
    }
}
