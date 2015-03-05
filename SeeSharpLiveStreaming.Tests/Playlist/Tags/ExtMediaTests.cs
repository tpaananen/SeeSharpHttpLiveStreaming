using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Master;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags
{
    [TestFixture]
    public class ExtMediaTests
    {

        private ExtMedia _extMedia;

        [SetUp]
        public void SetUp()
        {
            _extMedia = new ExtMedia();
            Assert.AreEqual("#EXT-X-MEDIA", _extMedia.TagName);
            Assert.AreEqual(TagType.ExtXMedia, _extMedia.TagType);
        }

        private static string GetAttributes(string type)
        {
            string value = 
                   "TYPE=" + type + "," + 
                   "GROUP-ID=\"GRID\"," +
                   "LANGUAGE=\"some-lang1\"," +
                   "ASSOC-LANGUAGE=\"some-lang2\"," + 
                   "NAME=\"My Name\"," + 
                   "DEFAULT=YES," + 
                   "AUTOSELECT=YES," + 
                   "CHARACTERISTICS=\"public.accessibility.transcribes-spoken-dialog,public.accessibility.transcribes-spoken-dialog2\"";

            if (type != MediaTypes.ClosedCaptions)
            {
                value += ",URI=\"https://example.com/video\"";
            }
            else
            {
                value += ",INSTREAM-ID=\"CC1\"";
            }
            if (type == MediaTypes.Subtitles)
            {
                value += ",FORCED=YES";
            }
            return value;
        }

        [Test]
        public void TestExtMediaThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _extMedia.Deserialize(null, 0));
        }

        [Test]
        public void TestExtMediaThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _extMedia.Deserialize(string.Empty, 0));
        }

        [Test]
        public void TestExtMediaIsParsedCorrectly([Values(MediaTypes.Video, MediaTypes.Audio, MediaTypes.Subtitles, MediaTypes.ClosedCaptions)] string mediaType)
        {
            string input = GetAttributes(mediaType);
            _extMedia.Deserialize(input, 0);

            Assert.AreEqual(mediaType, _extMedia.Type);
            Assert.AreEqual("GRID", _extMedia.GroupId);
            Assert.AreEqual("some-lang1", _extMedia.Language);
            Assert.AreEqual("some-lang2", _extMedia.AssocLanguage);
            Assert.AreEqual("My Name", _extMedia.Name);
            Assert.AreEqual(true, _extMedia.Default);
            Assert.AreEqual(true, _extMedia.AutoSelect);
            Assert.AreEqual(2, _extMedia.Characteristics.Count);
            Assert.AreEqual("public.accessibility.transcribes-spoken-dialog", _extMedia.Characteristics.ElementAt(0));
            Assert.AreEqual("public.accessibility.transcribes-spoken-dialog2", _extMedia.Characteristics.ElementAt(1));
            
            if (mediaType != MediaTypes.ClosedCaptions)
            {
                Assert.AreEqual("https://example.com/video", _extMedia.Uri.AbsoluteUri);
            }
            else
            {
                Assert.AreEqual("CC1", _extMedia.InstreamId);
            }
            if (mediaType == MediaTypes.Subtitles)
            {
                Assert.That(_extMedia.Forced);
            }
        }

        [Test]
        public void TestParsingFails()
        {
            Assert.Throws<SerializationException>(() => _extMedia.Deserialize("TYPE=\"asasasasas\"", 0));
        }
    }
}
