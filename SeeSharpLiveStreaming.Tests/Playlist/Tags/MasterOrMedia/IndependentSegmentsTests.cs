using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.MasterOrMedia;
using SeeSharpHttpLiveStreaming.Tests.Helpers;
using SeeSharpHttpLiveStreaming.Utils.Writers;

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

        [Test]
        public void TestIndependentSegmentsIsSerialized()
        {
            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            new IndependentSegments().Serialize(writer);
            Assert.AreEqual(_segments.TagName + Environment.NewLine, sb.ToString());
        }
    }
}
