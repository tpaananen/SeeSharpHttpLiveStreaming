using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Master;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Master
{
    [TestFixture]
    public class ExtIFrameStreamInfTests
    {
        private const string ValidIFrameStreamInf = "URI=\"http://example.com/iframestreaminf/\",BANDWIDTH=454545,AVERAGE-BANDWIDTH=98989,CODECS=\"AAC,H264,OGG\",RESOLUTION=1920x1080";

        private ExtIFrameStreamInf _frame;

        [SetUp]
        public void SetUp()
        {
            _frame = new ExtIFrameStreamInf();
            Assert.AreEqual("#EXT-X-I-FRAME-STREAM-INF", _frame.TagName);
            Assert.AreEqual(TagType.ExtXIFrameStreamInf, _frame.TagType);
        }

        [Test]
        public void TestIFrameStreamInfThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _frame.Deserialize(null, 0));
        }

        [Test]
        public void TestIFrameStreamInfThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _frame.Deserialize(string.Empty, 0));
        }

        [Test]
        public void TestIFrameStreamInfIsParsed()
        {
            _frame.Deserialize(ValidIFrameStreamInf, 0);
            Assert.AreEqual("http://example.com/iframestreaminf/", _frame.Uri.AbsoluteUri);
            Assert.AreEqual(454545, _frame.Bandwidth);
            Assert.AreEqual(98989, _frame.AverageBandwidth);
            
            var list = new List<string>
            {
                "AAC",
                "H264",
                "OGG"
            };
            var resolution = new Resolution(1920, 1080);

            Assert.AreEqual(list, _frame.Codecs);
            Assert.AreEqual(resolution, _frame.Resolution);

        }

        [Test]
        public void TestIFrameStreamInfIsParsedWithVideo()
        {
            const string valid = ValidIFrameStreamInf + ",VIDEO=\"VID\"";
            _frame.Deserialize(valid, 0);
            Assert.AreEqual("VID", _frame.Video);
        }

        [Test]
        public void TestIFrameStreamInfFailsToParse()
        {
            var invalid = ValidIFrameStreamInf.Replace("http://example.com/iframestreaminf/", "jshdjshd");
            Assert.Throws<SerializationException>(() => _frame.Deserialize(invalid, 0));
        }

    }
}
