using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Master;
using SeeSharpHttpLiveStreaming.Tests.Helpers;

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

        [Test]
        public void TestIFrameStreamInfSerialization()
        {
            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            var frame = new ExtIFrameStreamInf(454545, 98989, new[] {"AAC", "H264", "OGG"}, 
                                               new Resolution(1920, 1080),
                                               new Uri("http://example.com/iframestreaminf/"), "VID");
            frame.Serialize(writer);
            _frame.Deserialize(sb.ToString().Replace(_frame.TagName + Tag.TagEndMarker, string.Empty), 0);

            Assert.AreEqual(frame.Video, _frame.Video);
            Assert.AreEqual(frame.Uri.AbsoluteUri, _frame.Uri.AbsoluteUri);
            Assert.AreEqual(frame.AverageBandwidth, _frame.AverageBandwidth);
            Assert.AreEqual(frame.Bandwidth, _frame.Bandwidth);
            Assert.AreEqual(frame.Codecs, _frame.Codecs);
            Assert.AreEqual(frame.Resolution, _frame.Resolution);
        }

        [Test]
        public void TestIFrameStreamInfSerializationWithNullOrEmptyInput(
                    [Values(null, "")] string codecs,
                    [Values(null, "")] string video)
        {
            var codecArray = codecs == null ? null : new string[0];
            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            var frame = new ExtIFrameStreamInf(454545, 98989, codecArray, 
                                               new Resolution(1920, 1080),
                                               new Uri("http://example.com/iframestreaminf/"), video);
            frame.Serialize(writer);
            _frame.Deserialize(sb.ToString().Replace(_frame.TagName + Tag.TagEndMarker, string.Empty), 0);

            Assert.AreEqual(frame.Video, _frame.Video);
            Assert.AreEqual(frame.Uri.AbsoluteUri, _frame.Uri.AbsoluteUri);
            Assert.AreEqual(frame.AverageBandwidth, _frame.AverageBandwidth);
            Assert.AreEqual(frame.Bandwidth, _frame.Bandwidth);
            Assert.AreEqual(frame.Codecs, _frame.Codecs);
            Assert.AreEqual(frame.Resolution, _frame.Resolution);
        }

        [Test]
        public void TestIFrameStreamInfConstructorThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ExtIFrameStreamInf(0, 0, new [] { "" }, Resolution.Default, null, ""));
        }
    }
}
