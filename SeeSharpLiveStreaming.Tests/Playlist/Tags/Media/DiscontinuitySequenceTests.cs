using System;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media;
using SeeSharpHttpLiveStreaming.Tests.Helpers;

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
        public void TestDeserializationFails()
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

        [Test]
        public void TestCtorThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new DiscontinuitySequence(-1));
        }

        [Test]
        public void TestDiscontinuitySequenceSerializes()
        {
            var discSeq = new DiscontinuitySequence(121);
            StringBuilder sb;
            discSeq.Serialize(TestPlaylistWriterFactory.CreateWithStringBuilder(out sb));
            var line = new PlaylistLine(discSeq.TagName, sb.ToString());
            _discSeq.Deserialize(line.GetParameters(), 0);

            Assert.AreEqual(discSeq.Number, _discSeq.Number);
        }
    }
}
