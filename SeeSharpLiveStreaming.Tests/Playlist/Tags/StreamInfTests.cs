using System.Collections.Generic;
using NUnit.Framework;
using SeeSharpLiveStreaming.Playlist.Tags;
using SeeSharpLiveStreaming.Playlist.Tags.Master;

namespace SeeSharpLiveStreaming.Tests.Playlist.Tags
{
    [TestFixture]
    public class StreamInfTests
    {
        private StreamInf _streamInf;

        private const string ValidStreamInf = "BANDWIDTH=1212121,AVERAGE-BANDWIDTH=434343,CODECS=\"AAC,H264,OGG\",RESOLUTION=1920x1080,AUDIO=\"AUD\",VIDEO=\"VID\",SUBTITLES=\"SUBS\",CLOSED-CAPTIONS=\"CC\"";

        [SetUp]
        public void Setup()
        {
            _streamInf = new StreamInf();
        }

        [Test]
        public void TestParseStreamInf()
        {
            _streamInf.Deserialize(ValidStreamInf, 0);
            Assert.AreEqual(1212121, _streamInf.Bandwidth);
            Assert.AreEqual(434343, _streamInf.AverageBandwidth);
            CollectionAssert.AreEqual(new List<string> { "AAC", "H264", "OGG" }, _streamInf.Codecs);
            Assert.AreEqual(new Resolution(1920, 1080), _streamInf.Resolution);
            Assert.AreEqual("AUD", _streamInf.Audio);
            Assert.AreEqual("VID", _streamInf.Video);
            Assert.AreEqual("SUBS", _streamInf.Subtitles);
            Assert.AreEqual("CC", _streamInf.ClosedCaptions);
            Assert.That(_streamInf.HasClosedCaptions);
        }

    }
}
