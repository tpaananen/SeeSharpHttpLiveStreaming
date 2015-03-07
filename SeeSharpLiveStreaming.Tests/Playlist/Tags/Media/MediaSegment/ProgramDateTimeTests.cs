using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Media.MediaSegment
{
    [TestFixture]
    public class ProgramDateTimeTests
    {

        private ProgramDateTime _dateTime;

        [SetUp]
        public void SetUp()
        {
            _dateTime = new ProgramDateTime();
            Assert.AreEqual("#EXT-X-PROGRAM-DATE-TIME", _dateTime.TagName);
            Assert.AreEqual(TagType.ExtXProgramDateTime, _dateTime.TagType);
        }

        [Test]
        public void TestProgramDateTimeThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _dateTime.Deserialize(null, 0));
        }

        [Test]
        public void TestProgramDateTimeThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _dateTime.Deserialize(string.Empty, 0));
        }

        [Test]
        public void TestProgramDateTimeParsesDateTimeWithUtcOffset()
        {
            _dateTime.Deserialize("2010-02-19T14:54:23.031+08:00", 0);
            Assert.AreEqual(new DateTimeOffset(2010, 02, 19, 14, 54, 23, 031, TimeSpan.FromHours(8)).UtcTicks, 
                            _dateTime.DateTime.UtcTicks);
        }

        [Test]
        public void TestProgramDateTimeParsesDateTimeWithUtcIndicator()
        {
            _dateTime.Deserialize("2010-02-19T14:54:23.031Z", 0);
            Assert.AreEqual(new DateTimeOffset(2010, 02, 19, 14, 54, 23, 031, TimeSpan.FromHours(0)).UtcTicks, 
                            _dateTime.DateTime.UtcTicks);
        }

        [Test]
        public void TestProgramDateTimeParsingFails()
        {
            var exception = Assert.Throws<SerializationException>(() => _dateTime.Deserialize("2010-02-29T14:54:23.031Z", 0));
            Assert.AreEqual(typeof(FormatException), exception.InnerException.GetType());
        }
    }
}
