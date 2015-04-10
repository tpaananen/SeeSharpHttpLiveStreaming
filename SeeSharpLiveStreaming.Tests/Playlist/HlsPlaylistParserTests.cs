using System;
using System.Net;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    /// <summary>
    /// Playlist examples: https://tools.ietf.org/html/draft-pantos-http-live-streaming-14#section-8
    /// </summary>
    [TestFixture]
    public class HlsPlaylistParserTests : PlaylistReadingTestBase
    {
        private static readonly Uri _uri = new Uri("http://localhost/video/storage/");

        [Datapoints]
        public static string[] NewLines = { "\r\n", "\n" };

        [Theory]
        public void TestParserCreatesMasterPlaylist(string newLine)
        {
            var playlist = CreateValidMasterPlaylist(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist, _uri);

            AssertMasterPlaylist(playlistObject);
        }

        [Theory]
        public void TestParserCreatesMediaPlaylist(string newLine)
        {
            var playlist = CreateValidMediaPlaylist(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist, _uri);

            AssertMediaPlaylist(playlistObject);
        }

        [Theory]
        public void TestParserCreatesMediaPlaylistWithIFramesOnly(string newLine)
        {
            var playlist = CreateValidMediaPlaylistWithIFramesOnly(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist, _uri);

            AssertMediaPlaylist(playlistObject);
        }

        [Theory]
        public void TestParserFailsToCreatePlaylistWithMissingUriSegments(string newLine)
        {
            var playlist = CreateInvalidMediaPlaylistMissingUriFromSegment(newLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist, _uri));
        }

        [Theory]
        public void TestParserFailsToCreatePlaylistWithIFramesOnlyFromInvalidFile(string newLine)
        {
            var playlist = CreateInvalidMediaPlaylistWithIFramesOnly(newLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist, _uri));
        }

        [Theory]
        public void TestParserCreatesMediaPlaylistButThrowsIfTriedToParseAgain(string newLine)
        {
            var playlist = CreateValidMediaPlaylist(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist, _uri);
            Assert.Throws<InvalidOperationException>(() => playlistObject.Parse(playlist));
        }

        [Test]
        public void TestPlaylistThrowsIfVersionIsRequestedBeforePlaylistIsInitialized()
        {
            var list = new HlsPlaylist(_uri);
#pragma warning disable 219
            int x = 0;
#pragma warning restore 219
            Assert.Throws<InvalidOperationException>(() => x = list.Version);
        }

        [Test]
        public void TestPlaylistThrowsIfIsMasterIsRequestedBeforePlaylistIsInitialized()
        {
            var list = new HlsPlaylist(_uri);
#pragma warning disable 219
            bool x = false;
#pragma warning restore 219
            Assert.Throws<InvalidOperationException>(() => x = list.IsMaster);
        }

        [Test]
        public void TestPlaylistIsNotCreatedIfInvalidStartTagIsFound()
        {
            var line = "#EXTM3U22" + Environment.NewLine;
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(line, _uri));
        }

        [Test]
        public void TestPlaylistIsNotCreatedWhenOnlyStartTagAndBasicTagsAreFound()
        {
            var line = "#EXTM3U" + Environment.NewLine + "#EXT-X-VERSION:6" + Environment.NewLine; 
            var exception = Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(line, _uri));
            Assert.That(exception.InnerException is ArgumentException);
        }

        [Test]
        public void TestPlaylistParserThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => HlsPlaylistParser.Parse(null, _uri));
            Assert.Throws<ArgumentNullException>(() => HlsPlaylistParser.Parse("content", null));
        }

        [Test]
        public void TestPlaylistParserThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => HlsPlaylistParser.Parse(string.Empty, _uri));
        }

        [Test]
        public void TestMediaPlaylistIsNotCreatedIfItContainsMasterTags()
        {
            var playlist = CreateInvalidMediaPlaylist(Environment.NewLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist, _uri));
        }

        [Test]
        public void TestMasterPlaylistIsNotCreatedIfItContainsMediaOrMediaSegmentTags()
        {
            var playlist = CreateInvalidMasterPlaylist(Environment.NewLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist, _uri));
        }

        [Theory]
        public void TestParserCreatesMasterPlaylistWithRenditions(string newLine)
        {
            var playlist = CreateValidMasterPlaylistWithAlternativeRenditions(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist, _uri);

            Assert.IsNotNull(playlistObject);
            Assert.That(playlistObject is HlsPlaylist);
            Assert.IsNotNull(playlistObject.Playlist);
            Assert.That(playlistObject.IsMaster);
            Assert.AreEqual(7, playlistObject.Version);
            var master = (MasterPlaylist) playlistObject.Playlist;
            Assert.AreEqual(4, master.VariantStreams.Count);
        }

        [Theory]
        public void TestParserCreatesMasterPlaylistWithRenditionsThreeGroups(string newLine)
        {
            var playlist = CreateValidMasterPlaylistWithAlternativeRenditionsTwoGroups(newLine);
            var playlistObject = HlsPlaylistParser.Parse(playlist, _uri);

            Assert.IsNotNull(playlistObject);
            Assert.That(playlistObject is HlsPlaylist);
            Assert.IsNotNull(playlistObject.Playlist);
            Assert.That(playlistObject.IsMaster);
            Assert.AreEqual(7, playlistObject.Version);
            var master = (MasterPlaylist) playlistObject.Playlist;
            Assert.AreEqual(4, master.VariantStreams.Count);
        }

        [Theory]
        public void TestMasterPlaylistIsNotCreatedIfRenditionsDoNotMathchingAttributes(string newLine)
        {
            var playlist = CreateInvalidMasterPlaylistWithAlternativeRenditionsTwoGroups(newLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist, _uri));
        }

        [Test]
        public void TestMasterPlaylistIsNotCreatedIfRenditionsContainMultipleDefaults()
        {
            var playlist = CreateInvalidMasterPlaylistWithAlternativeRenditionsMultipleDefaults(Environment.NewLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist, _uri));
        }

        [Test]
        public void TestMasterPlaylistIsNotCreatedIfRenditionsContainNonUniqueLanguagesWithAutoselectYes()
        {
            var playlist = CreateInvalidMasterPlaylistWithAlternativeRenditionsAutoselectWithNonUniqueLanguages(Environment.NewLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist, _uri));
        }

        [Test]
        public void TestMasterPlaylistIsNotCreatedIfRenditionsContainNonUniqueNames()
        {
            var playlist = CreateInvalidMasterPlaylistWithAlternativeRenditionsNonUniqueNames(Environment.NewLine);
            Assert.Throws<SerializationException>(() => HlsPlaylistParser.Parse(playlist, _uri));
        }
    }
}
