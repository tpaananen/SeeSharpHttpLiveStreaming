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
        public void TestTargetDurationParsingFailsWhenInvalidInputValue([Values("-1", "0", "sds")] string value)
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

        [Test]
        public void TestTargetDurationCtorThrowsArgumentException([Values(-1L, 0L)] long value)
        {
            Assert.Throws<ArgumentException>(() => new TargetDuration(value));
        }

        [Test]
        public void TestTargetDurationSerializes()
        {
            var dur = new TargetDuration(121L);
            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            dur.Serialize(writer);
            var line = new PlaylistLine(dur.TagName, sb.ToString());
            _targetDuration.Deserialize(line.GetParameters(), 0);
            Assert.AreEqual(dur.Duration, _targetDuration.Duration);
        }

    }
}
