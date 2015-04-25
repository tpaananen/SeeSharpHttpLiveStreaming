using System;
using System.Collections.ObjectModel;
using System.Linq;
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
    public class ExtMediaTests
    {

        private ExtMedia _extMedia;

        [SetUp]
        public void SetUp()
        {
            _extMedia = (ExtMedia) TagFactory.Create("#EXT-X-MEDIA");
            _extMedia.BaseUri = new Uri("http://example.com/");
            Assert.AreEqual("#EXT-X-MEDIA", _extMedia.TagName);
            Assert.AreEqual(TagType.ExtXMedia, _extMedia.TagType);
        }

        private static string GetAttributes(string type, string instreamId, string defaultAttr, bool forceInstreamId = false)
        {
            string value = 
                   "TYPE=" + type + "," + 
                   "GROUP-ID=\"GRID\"," +
                   "LANGUAGE=\"some-lang1\"," +
                   "ASSOC-LANGUAGE=\"some-lang2\"," + 
                   "NAME=\"My Name\"," + 
                   "DEFAULT=" + defaultAttr + "," + 
                   "CHARACTERISTICS=\"public.accessibility.transcribes-spoken-dialog,public.accessibility.transcribes-spoken-dialog2\"";

            value += ",AUTOSELECT=" + defaultAttr;

            if (type != MediaTypes.ClosedCaptions)
            {
                value += ",URI=\"https://example.com/video\"";
            }
            
            if (type == MediaTypes.ClosedCaptions || forceInstreamId)
            {
                value += ",INSTREAM-ID=\"" + instreamId + "\"";
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
        public void TestExtMediaIsParsedCorrectly(
            [Values(MediaTypes.Video, MediaTypes.Audio, MediaTypes.Subtitles, MediaTypes.ClosedCaptions)] string mediaType, 
            [Values("CC1", "CC2", "CC3", "CC4", "SERVICE1", "SERVICE63")] string instreamId,
            [Values(YesNo.No, YesNo.Yes, "")] string defaultAttr)
        {
            string input = GetAttributes(mediaType, instreamId, defaultAttr);
            _extMedia.Deserialize(input, 0);

            Assert.AreEqual(mediaType, _extMedia.Type);
            Assert.AreEqual("GRID", _extMedia.GroupId);
            Assert.AreEqual("some-lang1", _extMedia.Language);
            Assert.AreEqual("some-lang2", _extMedia.AssocLanguage);
            Assert.AreEqual("My Name", _extMedia.Name);
            Assert.AreEqual(defaultAttr == YesNo.Yes, _extMedia.Default);
            Assert.AreEqual(defaultAttr == YesNo.Yes, _extMedia.AutoSelect);
            Assert.AreEqual(2, _extMedia.Characteristics.Count);
            Assert.AreEqual("public.accessibility.transcribes-spoken-dialog", _extMedia.Characteristics.ElementAt(0));
            Assert.AreEqual("public.accessibility.transcribes-spoken-dialog2", _extMedia.Characteristics.ElementAt(1));
            
            if (mediaType != MediaTypes.ClosedCaptions)
            {
                Assert.AreEqual("https://example.com/video", _extMedia.Uri.AbsoluteUri);
            }
            else
            {
                Assert.AreEqual(instreamId, _extMedia.InstreamId);
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

        [Test]
        public void TestParsingFailsIfInstreamIdIsNotCCxNorServiceN([Values("", "INVALID")] string invalidValue)
        {
            string input = GetAttributes(MediaTypes.ClosedCaptions, invalidValue, YesNo.Yes);
            Assert.Throws<SerializationException>(() => _extMedia.Deserialize(input, 0));
        }

        [Test]
        public void TestParsingFailsIfInstreamIdIsNotCCxButServiceNumberIsOutOfRange([Values(0, 64)] int number)
        {
            string input = GetAttributes(MediaTypes.ClosedCaptions, "SERVICE" + number, YesNo.Yes);
            Assert.Throws<SerializationException>(() => _extMedia.Deserialize(input, 0));
        }

        [Test]
        public void TestParsingFailsWhenInstreamIdIsPresentButMustNot()
        {
            string input = GetAttributes(MediaTypes.Subtitles, "SERVICE1", YesNo.Yes, true);
            Assert.Throws<SerializationException>(() => _extMedia.Deserialize(input, 0));
        }

        [Test]
        public void TestParsingFailsWhenDefaultIsInvalid()
        {
            string input = GetAttributes(MediaTypes.Subtitles, "SERVICE1", "FOO");
            Assert.Throws<SerializationException>(() => _extMedia.Deserialize(input, 0));
        }

        [Test]
        public void TestParsingFailsWhenForcedIsPresentWhenTypeIsNotSubtitlesOrInvalidValueProvided([Values(YesNo.Yes, YesNo.No, "FOO")] string value)
        {
            string input = GetAttributes(MediaTypes.Audio, "SERVICE1", YesNo.Yes);
            input += ",FORCED=" + value;
            Assert.Throws<SerializationException>(() => _extMedia.Deserialize(input, 0));
        }

        [Test]
        public void TestParsingFailsWhenAutoSelectHasInvalidValueOrDefaultIsYesAndAutoSelectIsNo([Values(YesNo.No, "FOO")] string value)
        {
            string input = GetAttributes(MediaTypes.Audio, "SERVICE1", YesNo.Yes);
            input = input.Replace("AUTOSELECT=YES,", "AUTOSELECT=" + value + ",");
            Assert.Throws<SerializationException>(() => _extMedia.Deserialize(input, 0));
        }

        [Test]
        public void TestParsingFailsWhenUriIsProvidedAndTypeIsClosedCaptions()
        {
            string input = GetAttributes(MediaTypes.ClosedCaptions, "SERVICE1", YesNo.Yes);
            input += "URI=\"https://example.com\"";
            Assert.Throws<SerializationException>(() => _extMedia.Deserialize(input, 0));
        }

        [Test]
        public void TestExtMediaThrowsArgumentNullExceptionFromConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new ExtMedia(null, "groupId", "lang", "assoc-lang", "name", false, false, false, "instream-id", new string[0]));
            Assert.Throws<ArgumentNullException>(() => new ExtMedia(MediaTypes.Video, null, "lang", "assoc-lang", "name", false, false, false, "instream-id", new string[0]));
            Assert.Throws<ArgumentNullException>(() => new ExtMedia(MediaTypes.Video, "groupId", "lang", "assoc-lang", null, false, false, false, "instream-id", new string[0]));
            Assert.Throws<ArgumentNullException>(() => new ExtMedia(MediaTypes.ClosedCaptions, "groupId", "lang", "assoc-lang", "name", false, false, false, null, new string[0]));
        }

        [Test]
        public void TestExtMediaThrowsArgumentExceptionFromConstructor()
        {
            Assert.Throws<ArgumentException>(() => new ExtMedia("", "groupId", "lang", "assoc-lang", "name", false, false, false, "CC1", new string[0]));
            Assert.Throws<ArgumentException>(() => new ExtMedia("INVALID", "groupId", "lang", "assoc-lang", "name", false, false, false, "CC2", new string[0]));
            Assert.Throws<ArgumentException>(() => new ExtMedia(MediaTypes.Video, "", "lang", "assoc-lang", "name", false, false, false, "CC3", new string[0]));
            Assert.Throws<ArgumentException>(() => new ExtMedia(MediaTypes.Video, "groupId", "lang", "assoc-lang", "", false, false, false, "CC4", new string[0]));
            Assert.Throws<ArgumentException>(() => new ExtMedia(MediaTypes.ClosedCaptions, "groupId", "lang", "assoc-lang", "name", false, false, false, "", null));
            Assert.Throws<ArgumentException>(() => new ExtMedia(MediaTypes.ClosedCaptions, "groupId", "lang", "assoc-lang", "name", false, false, false, "FOO", null));
        }

        [Test]
        public void TestExtMediaIsSerializedWithUri(
            [Values(MediaTypes.Audio, MediaTypes.Video)] string mediaType,
            [Values(true, false)] bool autoSelect,
            [Values(true, false)] bool defaultVal)
        {
            var extMedia = new ExtMedia(mediaType, "groupId", "lang", "assoc-lang", "name", defaultVal, autoSelect, false,
                                        null, autoSelect ? new [] { "jeba" } : null, new Uri("http://example.com/"));

            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            extMedia.Serialize(writer);
            var playlist = new PlaylistLine(_extMedia.TagName, sb.ToString());
            _extMedia.Deserialize(playlist.GetParameters(), 0);

            AssertAreEqual(extMedia, _extMedia);
        }

        [Test]
        public void TestExtMediaIsSerializedWithClosedCaptions(
            [Values(true, false)] bool autoSelect,
            [Values(true, false)] bool defaultVal)
        {
            var extMedia = new ExtMedia(MediaTypes.ClosedCaptions, "groupId", "lang", "assoc-lang", "name", defaultVal, autoSelect, false,
                                        "CC1", autoSelect ? new [] { "jeba", "hoba" } : null, new Uri("http://example.com/"));

            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            extMedia.Serialize(writer);
            var playlist = new PlaylistLine(_extMedia.TagName, sb.ToString());
            _extMedia.Deserialize(playlist.GetParameters(), 0);

            AssertAreEqual(extMedia, _extMedia);
        }

        [Test]
        public void TestExtMediaIsSerializedWithSubtitles(
            [Values(true, false)] bool forced,
            [Values(true, false)] bool autoSelect,
            [Values(true, false)] bool defaultVal)
        {
            var extMedia = new ExtMedia(MediaTypes.Subtitles, "groupId", "lang", "assoc-lang", "name", defaultVal, autoSelect, forced,
                                        null, autoSelect ? new [] { "jeba" } : null);

            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            extMedia.Serialize(writer);
            var playlist = new PlaylistLine(_extMedia.TagName, sb.ToString());
            _extMedia.Deserialize(playlist.GetParameters(), 0);

            AssertAreEqual(extMedia, _extMedia);
        }

        [Test]
        public void TestExtMediaEqualityChecking2([Values(MediaTypes.Audio, MediaTypes.Subtitles, MediaTypes.ClosedCaptions)] string mediaType)
        {
            var expected = new ExtMedia(mediaType, "groupId", "en", "aen", "Suomi", true, true, false, GetInstreamId(mediaType), null);
            var media1 = new ExtMedia(mediaType, "groupId", "en", "aen", "Suomi", true, true, false, GetInstreamId(mediaType), null);
            AssertMediaEquality(expected, media1, "1");

            media1 = new ExtMedia(mediaType, "groupId", "", "aen", "Suomi", true, true, false, GetInstreamId(mediaType), null);
            AssertMediaEquality(expected, media1, "2");

            media1 = new ExtMedia(mediaType, "groupId", "en", "", "Suomi", true, true, false, GetInstreamId(mediaType), null);
            AssertMediaEquality(expected, media1, "3");

            media1 = new ExtMedia(mediaType, "groupId", "en", "aen", "Suomi2", true, true, false, GetInstreamId(mediaType), null);
            AssertMediaEquality(expected, media1, "4");

            media1 = new ExtMedia(mediaType, "groupId", "en", "aen", "Suomi", false, true, false, GetInstreamId(mediaType), null);
            AssertMediaEquality(expected, media1, "5");

            media1 = new ExtMedia(mediaType, "groupId", "en", "aen", "Suomi", true, false, false, GetInstreamId(mediaType), null);
            AssertMediaEquality(expected, media1, "6");

            media1 = new ExtMedia(mediaType, "groupId", "en", "aen", "Suomi", true, true, true, GetInstreamId(mediaType), null);
            AssertMediaEquality(expected, media1, "7");

            media1 = new ExtMedia(mediaType, "groupId", "en", "aen", "Suomi", true, true, false, GetInstreamId(mediaType), null);
            AssertMediaEquality(expected, media1, "8");

            media1 = new ExtMedia(mediaType, "groupId", "en", "aen", "Suomi", true, true, false, mediaType == MediaTypes.ClosedCaptions ? "CC2" : "", null);
            AssertMediaEquality(expected, media1, "9");

            media1 = new ExtMedia(mediaType, "groupId", "en", "aen", "Suomi", true, true, false, mediaType == MediaTypes.ClosedCaptions ? "CC2" : "", new ReadOnlyCollection<string>(new []{ "huu" }));
            AssertMediaEquality(expected, media1, "10");
        }

        private static string GetInstreamId(string mediaType)
        {
            return mediaType == MediaTypes.ClosedCaptions ? "CC1" : "";
        }

        private static void AssertMediaEquality(ExtMedia expected, ExtMedia actual, string message)
        {
            var isTrue = actual.Language == "en" &&
                         actual.AssocLanguage == "aen" &&
                         actual.Name == "Suomi" &&
                         actual.Default &&
                         actual.AutoSelect &&
                         !actual.Forced &&
                         new ReadOnlyCollection<string>(new string[0]).SequenceEqual(actual.Characteristics) &&
                         actual.InstreamId == (actual.Type == MediaTypes.ClosedCaptions ? "CC1" : "");

            Assert.AreEqual(isTrue, expected.EqualityCheck(new[] {actual}), message);
        }

        private static void AssertAreEqual(ExtMedia expected, ExtMedia actual)
        {
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.Uri, actual.Uri);
            Assert.AreEqual(expected.Language, actual.Language);
            Assert.AreEqual(expected.AssocLanguage, actual.AssocLanguage);
            Assert.AreEqual(expected.GroupId, actual.GroupId);
            Assert.AreEqual(expected.InstreamId, actual.InstreamId);
            Assert.AreEqual(expected.Forced, actual.Forced);
            Assert.AreEqual(expected.AutoSelect, actual.AutoSelect);
            Assert.AreEqual(expected.Default, actual.Default);
            Assert.AreEqual(expected.Characteristics, actual.Characteristics);
        }
    }
}
