using System;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;
using SeeSharpHttpLiveStreaming.Tests.Helpers;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Media.MediaSegment
{
    [TestFixture]
    public class ExtInfTests
    {
        private ExtInf _extInf;

        [SetUp]
        public void SetUp()
        {
            _extInf = new ExtInf();
            Assert.AreEqual("#EXTINF", _extInf.TagName);
            Assert.AreEqual(TagType.ExtInf, _extInf.TagType);
        }

        [Test]
        public void TestExtInfThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _extInf.Deserialize(null, 0));
        }

        [Test]
        public void TestExtInfThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _extInf.Deserialize(string.Empty, 0));
        }

        [Test]
        public void TestExtInfThrowsIfParsingNonNumericValue()
        {
            Assert.Throws<SerializationException>(() => _extInf.Deserialize("sdsdsd", 0));
        }

        [Test]
        public void TestExtInfThrowsIfParsingNoIntegerBeforeComma()
        {
           var exception = Assert.Throws<SerializationException>(() => _extInf.Deserialize("sdsdsd,121212", 0));
           Assert.AreEqual(typeof(FormatException), exception.InnerException.GetType());
        }

        [Test]
        public void TestExtInfParses()
        {
            _extInf.Deserialize("12121,title", 0);
            Assert.AreEqual(12121, _extInf.Duration);
            Assert.AreEqual("title", _extInf.Information);
        }

        [Test]
        public void TestExtInfParsesWithoutInfo()
        {
            _extInf.Deserialize("12121", 0);
            Assert.AreEqual(12121, _extInf.Duration);
            Assert.AreEqual(string.Empty, _extInf.Information);
        }

        [Test]
        public void TestExtInfCtorThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExtInf(-1, "This is a negative duration.", 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExtInf(12, "This is a negative version.", -1));
        }

        [Test]
        public void TestExtInfSerializesForCompatibilityVersionLessThan3([Range(0, 2)] int version)
        {
            var inf = new ExtInf(1000.01m, "1000 seconds", version);
            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            inf.Serialize(writer);
            var line = new PlaylistLine(inf.TagName, sb.ToString());
            _extInf.Deserialize(line.GetParameters(), version);

            Assert.AreEqual(inf.Duration, _extInf.Duration);
            Assert.AreEqual(inf.Information, _extInf.Information);
        }

        [Test]
        public void TestExtInfSerializesForCompatibilityVersionGreaterThanOrEqualTo3([Values("1000.01 seconds", "", null)] string title)
        {
            var inf = new ExtInf(1000.01m, title, 3);
            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            inf.Serialize(writer);
            var line = new PlaylistLine(inf.TagName, sb.ToString());
            _extInf.Deserialize(line.GetParameters(), 3);

            Assert.AreEqual(inf.Duration, _extInf.Duration);
            Assert.AreEqual(inf.Information ?? string.Empty, _extInf.Information);
        }
    }
}
