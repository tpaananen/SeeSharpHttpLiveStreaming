using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Media
{
    [TestFixture]
    public class DiscontinuitySequenceTests
    {
        private DiscontinuitySequence _discSeq;

        [SetUp]
        public void SetUp()
        {
            _discSeq = new DiscontinuitySequence();
            Assert.AreEqual("#EXT-X-DISCONTINUITY-SEQUENCE", _discSeq.TagName);
            Assert.AreEqual(TagType.ExtXDiscontinuitySequence, _discSeq.TagType);
        }

        [Test]
        public void TestParsingThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _discSeq.Deserialize(null, 0));
        }

        [Test]
        public void TestParsingThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _discSeq.Deserialize(string.Empty, 0));
        }

        [Test]
        public void TestDeserializationFailes()
        {
            var exception = Assert.Throws<SerializationException>(() => _discSeq.Deserialize("sdsdsd", 0));
            Assert.That(exception.InnerException is FormatException);
        }

        [Test]
        public void TestDeserializationSucceeds()
        {
            _discSeq.Deserialize("4245", 0);
            Assert.AreEqual(4245, _discSeq.Number);
        }
    }
}
