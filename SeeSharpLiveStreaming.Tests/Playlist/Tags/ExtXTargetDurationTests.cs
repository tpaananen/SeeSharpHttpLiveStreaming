using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media;

namespace SeeSharpLiveStreaming.Tests.Playlist.Tags
{
    [TestFixture]
    public class ExtXTargetDurationTests
    {
        private TargetDuration _targetDuration;

        [SetUp]
        public void SetUp()
        {
            _targetDuration = new TargetDuration();
            Assert.AreEqual("#EXT-X-TARGETDURATION", _targetDuration.TagName);
            Assert.AreEqual(TagType.ExtXTargetDuration, _targetDuration.TagType);
        }

        [Test]
        public void TestTargetDurationParsed()
        {
            const string validValue = "1234";
            _targetDuration.Deserialize(validValue, 0);
            Assert.AreEqual(1234, _targetDuration.Duration);
        }

        [Test]
        public void TestTargetDurationParsingFailsWhenInvalidInputValue([Values("0", "sds")] string value)
        {
            Assert.Throws<SerializationException>(() => _targetDuration.Deserialize(value, 0));
        }

        [Test]
        public void TestTargetDurationThrowsForNullInput()
        {
            Assert.Throws<ArgumentNullException>(() => _targetDuration.Deserialize(null, 0));
        }

        [Test]
        public void TestTargetDurationThrowsForEmptyInput()
        {
            Assert.Throws<ArgumentException>(() => _targetDuration.Deserialize(string.Empty, 0));
        }

    }
}
