using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;

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
        public void TestExtInfThrowsIfParsingNoCommaSeparatedValue()
        {
            Assert.Throws<SerializationException>(() => _extInf.Deserialize("sdsdsd;title", 0));
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

    }
}
