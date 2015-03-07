using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Media
{
    [TestFixture]
    public class MediaSequenceTests
    {

        private MediaSequence _mediaSequence;

        [SetUp]
        public void SetUp()
        {
            _mediaSequence = new MediaSequence();
            Assert.AreEqual("#EXT-X-MEDIA-SEQUENCE", _mediaSequence.TagName);
            Assert.AreEqual(TagType.ExtXMediaSequence, _mediaSequence.TagType);
        }

        [Test]
        public void TestMediaSequenceThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _mediaSequence.Deserialize(null, 0));
        }

        [Test]
        public void TestMediaSequenceThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _mediaSequence.Deserialize(string.Empty, 0));
        }

        [Test]
        public void TestMediaSequenceThrowsIfParsingFails()
        {
            var exception = Assert.Throws<SerializationException>(() => _mediaSequence.Deserialize("121212dsd", 0));
            Assert.AreEqual(typeof(FormatException), exception.InnerException.GetType());
        }

        [Test]
        public void TestMediaSequenceIsParsed()
        {
            _mediaSequence.Deserialize("54321", 0);
            Assert.AreEqual(54321, _mediaSequence.Number);
        }


    }
}
