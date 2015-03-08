using System;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    /// <summary>
    /// Playlist examples: https://tools.ietf.org/html/draft-pantos-http-live-streaming-14#section-8
    /// </summary>
    [TestFixture]
    public class HlsPlaylistParserTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        private static string CreateValidMediaPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:6" + lineFeed +
                "#EXT-X-TARGETDURATION:10" + lineFeed +
                "" + lineFeed +
                "#EXTINF:9.009,Some info" + lineFeed +
                "http://media.example.com/first.ts" + lineFeed +
                "#EXT-X-BYTERANGE:1024@0" + lineFeed +
                "#EXT-X-DISCONTINUITY" + lineFeed + 
                "#EXTINF:9.009,Some other info" + lineFeed +
                "http://media.example.com/second.ts" + lineFeed +
                "#EXT-X-BYTERANGE:1024@512" + lineFeed +
                "#EXT-X-DISCONTINUITY" + lineFeed + 
                "#EXTINF:3.003,Some short take" + lineFeed +
                "http://media.example.com/third.ts" + lineFeed +
                "#EXT-X-BYTERANGE:1024@45" + lineFeed +
                "#EXT-X-DISCONTINUITY" + lineFeed;
        }

        private static string CreateInvalidMediaPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:6" + lineFeed + 
                "#EXT-X-TARGETDURATION:10" + lineFeed +
                "" + lineFeed +
                "#EXTINF:9.009,Some info" + lineFeed +
                "http://media.example.com/first.ts" + lineFeed +
                "#EXTINF:9.009,Some other info" + lineFeed +
                "http://media.example.com/second.ts" + lineFeed +
                "#EXT-X-DISCONTINUITY" + lineFeed + 
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000" + lineFeed + "http://example.com/mid.m3u8" + lineFeed +
                "#EXTINF:3.003,Some short take" + lineFeed +
                "http://media.example.com/third.ts" + lineFeed;
        }

        private static string CreateValidMasterPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed + 
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000" + lineFeed + " http://example.com/low.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000" + lineFeed + "http://example.com/mid.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000" + lineFeed + "http://example.com/hi.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\"" + lineFeed + "http://example.com/audio-only.m3u8" + lineFeed;
        }

        private static string CreateInvalidMasterPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed + 
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000" + lineFeed + " http://example.com/low.m3u8" + lineFeed +
                "#EXT-X-DISCONTINUITY" + lineFeed + 
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000" + lineFeed + "http://example.com/mid.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000" + lineFeed + "http://example.com/hi.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\"" + lineFeed + "http://example.com/audio-only.m3u8" + lineFeed;
        }

        private static string CreateValidMasterPlaylistWithAlternativeRenditions(string lineFeed)
        {
            return 
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"English\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"http://example.com/main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Deutsch\"," + 
                "DEFAULT=NO,AUTOSELECT=YES,LANGUAGE=\"de\"," +
                "URI=\"http://example.com/main/german-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Commentary\"," +
                "DEFAULT=NO,AUTOSELECT=NO,LANGUAGE=\"en\"," + 
                "URI=\"http://example.com/commentary/audio-only.m3u8\"" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/low/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/mid/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/hi/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/main/english-audio.m3u8" + lineFeed;
        }

        private static string CreateValidMasterPlaylistWithAlternativeRenditionsTwoGroups(string lineFeed)
        {
            return 
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed +

                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"English\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"http://example.com/main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Deutsch\"," + 
                "DEFAULT=NO,AUTOSELECT=YES,LANGUAGE=\"de\"," +
                "URI=\"http://example.com/main/german-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Commentary\"," +
                "DEFAULT=NO,AUTOSELECT=NO,LANGUAGE=\"en\"," + 
                "URI=\"http://example.com/commentary/audio-only.m3u8\"" + lineFeed +

                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"mp3\",NAME=\"English\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"http://example.com/main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"mp3\",NAME=\"Deutsch\"," + 
                "DEFAULT=NO,AUTOSELECT=YES,LANGUAGE=\"de\"," +
                "URI=\"http://example.com/main/german-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"mp3\",NAME=\"Commentary\"," +
                "DEFAULT=NO,AUTOSELECT=NO,LANGUAGE=\"en\"," + 
                "URI=\"http://example.com/commentary/audio-only.m3u8\"" + lineFeed +

                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/low/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/mid/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/hi/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/main/english-audio.m3u8" + lineFeed;
        }

        private static string CreateInvalidMasterPlaylistWithAlternativeRenditionsNonUniqueNames(string lineFeed)
        {
            return 
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"English\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"http://example.com/main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"English\"," + 
                "DEFAULT=NO,AUTOSELECT=NO,LANGUAGE=\"de\"," + 
                "URI=\"http://example.com/main/german-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Commentary\"," +
                "DEFAULT=YES,LANGUAGE=\"fi\"," + 
                "URI=\"http://example.com/commentary/audio-only.m3u8\"" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/low/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/mid/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/hi/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/main/english-audio.m3u8" + lineFeed;
        }

        private static string CreateInvalidMasterPlaylistWithAlternativeRenditionsMultipleDefaults(string lineFeed)
        {
            return 
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"English\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"http://example.com/main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Deutsch\"," + 
                "DEFAULT=NO,AUTOSELECT=NO,LANGUAGE=\"de\"," + 
                "URI=\"http://example.com/main/german-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Commentary\"," +
                "DEFAULT=YES,LANGUAGE=\"fi\"," + 
                "URI=\"http://example.com/commentary/audio-only.m3u8\"" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/low/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/mid/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/hi/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/main/english-audio.m3u8" + lineFeed;
        }

        private static string CreateInvalidMasterPlaylistWithAlternativeRenditionsAutoselectWithNonUniqueLanguages(string lineFeed)
        {
            return 
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"English\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"http://example.com/main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Deutsch\"," + 
                "DEFAULT=NO,AUTOSELECT=YES,LANGUAGE=\"en\"," + 
                "URI=\"http://example.com/main/german-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Commentary\"," +
                "DEFAULT=NO,AUTOSELECT=NO,LANGUAGE=\"de\"," + 
                "URI=\"http://example.com/commentary/audio-only.m3u8\"" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/low/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/mid/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/hi/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/main/english-audio.m3u8" + lineFeed;
        }

        [Datapoints]
        public static string[] NewLines = { "\r\n", "\n" };

        [Theory]
        public void TestParserCreatesMasterPlaylist(string newLine)
        {
            var playlist = CreateValidMasterPlaylist(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist);

            Assert.IsNotNull(playlistObject);
            Assert.That(playlistObject is HlsPlaylist);
            Assert.IsNotNull(playlistObject.Playlist);
            Assert.That(playlistObject.IsMaster);
            Assert.AreEqual(7, playlistObject.Version);
            Assert.AreEqual(4, playlistObject.Playlist.Tags.Count);
            var master = (MasterPlaylist) playlistObject.Playlist;
            Assert.AreEqual(0, master.RenditionGroups.Count);
        }

        [Theory]
        public void TestParserCreatesMediaPlaylist(string newLine)
        {
            var playlist = CreateValidMediaPlaylist(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist);

            Assert.IsNotNull(playlistObject);
            Assert.That(playlistObject is HlsPlaylist);
            Assert.IsNotNull(playlistObject.Playlist);
            Assert.That(playlistObject.Playlist is MediaPlaylist);
            var mediaPlaylist = (MediaPlaylist)playlistObject.Playlist;
            Assert.AreEqual(3, mediaPlaylist.MediaSegments.Count );
            Assert.That(mediaPlaylist.MediaSegments.All(x => x.Tags != null)); // parsing not proper yet
            Assert.AreEqual(6, playlistObject.Version);
            Assert.AreEqual(1, playlistObject.Playlist.Tags.Count);
        }

        [Theory]
        public void TestParserCreatesMediaPlaylistButThrowsIfTriedToParseAgain(string newLine)
        {
            var playlist = CreateValidMediaPlaylist(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist);
            Assert.Throws<InvalidOperationException>(() => playlistObject.Parse(playlist));
        }

        [Test]
        public void TestPlaylistThrowsIfVersionIsRequestedBeforePlaylistIsParsed()
        {
            var list = new HlsPlaylist();
#pragma warning disable 219
            int x = 0;
#pragma warning restore 219
            Assert.Throws<InvalidOperationException>(() => x = list.Version);
        }

        [Test]
        public void TestPlaylistIsNotCreatedIfInvalidStartTagIsFound()
        {
            var line = "#EXTM3U22" + Environment.NewLine;
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(line));
        }

        [Test]
        public void TestPlaylistIsNotCreatedWhenOnlyStartTagAndBasicTagsAreFound()
        {
            var line = "#EXTM3U" + Environment.NewLine + "#EXT-X-VERSION:6" + Environment.NewLine; 
            var exception = Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(line));
            Assert.That(exception.InnerException is ArgumentException);
        }

        [Test]
        public void TestPlaylistParserThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => HlsPlaylistParser.Parse(null));
        }

        [Test]
        public void TestPlaylistParserThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => HlsPlaylistParser.Parse(string.Empty));
        }

        [Test]
        public void TestMediaPlaylistIsNotCreatedIfItContainsMasterTags()
        {
            var playlist = CreateInvalidMediaPlaylist(Environment.NewLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist));
        }

        [Test]
        public void TestMasterPlaylistIsNotCreatedIfItContainsMediaOrMediaSegmentTags()
        {
            var playlist = CreateInvalidMasterPlaylist(Environment.NewLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist));
        }

        [Theory]
        public void TestParserCreatesMasterPlaylistWithRenditions(string newLine)
        {
            var playlist = CreateValidMasterPlaylistWithAlternativeRenditions(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist);

            Assert.IsNotNull(playlistObject);
            Assert.That(playlistObject is HlsPlaylist);
            Assert.IsNotNull(playlistObject.Playlist);
            Assert.That(playlistObject.IsMaster);
            Assert.AreEqual(7, playlistObject.Version);
            Assert.AreEqual(7, playlistObject.Playlist.Tags.Count);
            var master = (MasterPlaylist) playlistObject.Playlist;
            Assert.AreEqual(1, master.RenditionGroups.Count);
        }

        [Theory]
        public void TestParserCreatesMasterPlaylistWithRenditionsTwoGroups(string newLine)
        {
            var playlist = CreateValidMasterPlaylistWithAlternativeRenditionsTwoGroups(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist);

            Assert.IsNotNull(playlistObject);
            Assert.That(playlistObject is HlsPlaylist);
            Assert.IsNotNull(playlistObject.Playlist);
            Assert.That(playlistObject.IsMaster);
            Assert.AreEqual(7, playlistObject.Version);
            Assert.AreEqual(10, playlistObject.Playlist.Tags.Count);
            var master = (MasterPlaylist) playlistObject.Playlist;
            Assert.AreEqual(2, master.RenditionGroups.Count);
        }

        [Test]
        public void TestMasterPlaylistIsNotCreatedIfRenditionsContainMultipleDefaults()
        {
            var playlist = CreateInvalidMasterPlaylistWithAlternativeRenditionsMultipleDefaults(Environment.NewLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist));
        }

        [Test]
        public void TestMasterPlaylistIsNotCreatedIfRenditionsContainNonUniqueLanguagesWithAutoselectYes()
        {
            var playlist = CreateInvalidMasterPlaylistWithAlternativeRenditionsAutoselectWithNonUniqueLanguages(Environment.NewLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist));
        }

        [Test]
        public void TestMasterPlaylistIsNotCreatedIfRenditionsContainNonUniqueNames()
        {
            var playlist = CreateInvalidMasterPlaylistWithAlternativeRenditionsNonUniqueNames(Environment.NewLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist));
        }
    }
}
