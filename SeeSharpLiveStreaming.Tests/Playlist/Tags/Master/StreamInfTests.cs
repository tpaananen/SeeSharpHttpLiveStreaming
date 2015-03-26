using System;
using System.Collections.Generic;
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
    public class StreamInfTests
    {
        private StreamInf _streamInf;

        private const string ValidStreamInf = "BANDWIDTH=1212121,AVERAGE-BANDWIDTH=434343,CODECS=\"AAC,H264,OGG\",RESOLUTION=1920x1080,AUDIO=\"AUD\",VIDEO=\"VID\",SUBTITLES=\"SUBS\",CLOSED-CAPTIONS=\"CC\"";

        [SetUp]
        public void Setup()
        {
            _streamInf = new StreamInf();
            Assert.AreEqual("#EXT-X-STREAM-INF", _streamInf.TagName);
            Assert.AreEqual(TagType.ExtXStreamInf, _streamInf.TagType);
        }

        [Test]
        public void TestStreamInfThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _streamInf.Deserialize(null, 0));
        }

        [Test]
        public void TestStreamInfThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _streamInf.Deserialize(string.Empty, 0));
        }

        [Test]
        public void TestParseStreamInf()
        {
            _streamInf.Deserialize(ValidStreamInf, 0);
            Assert.AreEqual(1212121, _streamInf.Bandwidth);
            Assert.AreEqual(434343, _streamInf.AverageBandwidth);

            var list = new List<string>
            {
                "AAC",
                "H264",
                "OGG"
            };
            var resolution = new Resolution(1920, 1080);
            CollectionAssert.AreEqual(list, _streamInf.Codecs);
            Assert.AreEqual(resolution, _streamInf.Resolution);
            Assert.AreEqual("AUD", _streamInf.Audio);
            Assert.AreEqual("VID", _streamInf.Video);
            Assert.AreEqual("SUBS", _streamInf.Subtitles);
            Assert.AreEqual("CC", _streamInf.ClosedCaptions);
            Assert.That(_streamInf.HasClosedCaptions);
        }

        [Test]
        public void TestParsingFails()
        {
            const string invalid = "BANDWIDTH=121212g";
            Assert.Throws<SerializationException>(() => _streamInf.Deserialize(invalid, 0));
        }

        [Test]
        public void TestHasNotClosedCaptions()
        {
            string validStreamInf = "BANDWIDTH=1212121,AVERAGE-BANDWIDTH=434343,CODECS=\"AAC,H264,OGG\",RESOLUTION=1920x1080,AUDIO=\"AUD\",VIDEO=\"VID\",SUBTITLES=\"SUBS\",CLOSED-CAPTIONS=\"NONE\"";
            _streamInf.Deserialize(validStreamInf, 0);
            Assert.IsFalse(_streamInf.HasClosedCaptions);

            validStreamInf = "BANDWIDTH=1212121,AVERAGE-BANDWIDTH=434343,CODECS=\"AAC,H264,OGG\",RESOLUTION=1920x1080,AUDIO=\"AUD\",VIDEO=\"VID\",SUBTITLES=\"SUBS\"";
            _streamInf.Deserialize(validStreamInf, 0);
            Assert.IsFalse(_streamInf.HasClosedCaptions);

            _streamInf = new StreamInf();
            Assert.IsFalse(_streamInf.HasClosedCaptions);
        }

        [Test]
        public void TestStreamInfSerialization()
        {
            var codecArray = "AAC,OGG".Split(',');
            var streamInf = new StreamInf(1212121, 121212, codecArray, new Resolution(1920, 1080), 
                                          "AUD", "VID", "SUBS", "CC");
            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            streamInf.Serialize(writer);

            _streamInf.Deserialize(sb.ToString(), 0);
            
            Assert.AreEqual(streamInf.Video, _streamInf.Video);
            Assert.AreEqual(streamInf.Audio, _streamInf.Audio);
            Assert.AreEqual(streamInf.AverageBandwidth, _streamInf.AverageBandwidth);
            Assert.AreEqual(streamInf.Bandwidth, _streamInf.Bandwidth);
            Assert.AreEqual(streamInf.ClosedCaptions, _streamInf.ClosedCaptions);
            Assert.AreEqual(streamInf.Codecs, _streamInf.Codecs);
            Assert.AreEqual(streamInf.Resolution, _streamInf.Resolution);
            Assert.AreEqual(streamInf.Subtitles, _streamInf.Subtitles);
        }

        [Test]
        public void TestStreamInfSerializationWithNullEmptyOrDefaultValues(
            [Values(null, "")] string codecs,
            [Values(null, "")] string audio,
            [Values(null, "")] string video,
            [Values(null, "")] string subtitles,
            [Values(null, "")] string closedCaptions)
        {
            StringBuilder sb;
            var codecArray = codecs == null ? null : new string[0];
            var streamInf = new StreamInf(1212121, 0, codecArray, Resolution.Default, 
                                          audio, video, subtitles, closedCaptions);
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            streamInf.Serialize(writer);

            _streamInf.Deserialize(sb.ToString(), 0);
            
            Assert.AreEqual(streamInf.Video, _streamInf.Video);
            Assert.AreEqual(streamInf.Audio, _streamInf.Audio);
            Assert.AreEqual(streamInf.AverageBandwidth, _streamInf.AverageBandwidth);
            Assert.AreEqual(streamInf.Bandwidth, _streamInf.Bandwidth);
            Assert.AreEqual(streamInf.ClosedCaptions, _streamInf.ClosedCaptions);
            Assert.AreEqual(streamInf.Codecs, _streamInf.Codecs);
            Assert.AreEqual(streamInf.Resolution, _streamInf.Resolution);
            Assert.AreEqual(streamInf.Subtitles, _streamInf.Subtitles);
        }

        [Test]
        public void TestStreamInfSerializeWithNullWriter()
        {
            var streamInf = new StreamInf(1212121, 0, null, Resolution.Default, 
                                          "aud", "vid", "sub", "cc1");
            Assert.Throws<ArgumentNullException>(() => streamInf.Serialize(null));
        }

    }
}
