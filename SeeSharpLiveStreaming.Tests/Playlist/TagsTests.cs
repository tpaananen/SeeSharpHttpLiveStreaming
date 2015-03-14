using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    [TestFixture]
    public class TagsTests
    {
        [Test]
        public void TestIsBasicTag()
        {
            Assert.That(Tag.IsBasicTag("#EXTM3U"));
            Assert.That(Tag.IsBasicTag("#EXT-X-VERSION"));
        }

        [Test]
        public void TestIsNotBasicTag()
        {
            Assert.IsFalse(Tag.IsBasicTag("EXTM3U"));
            Assert.IsFalse(Tag.IsBasicTag("EXT-X-VERSION"));
        }

        [Test]
        public void TestIsMediaPlaylistTag()
        {
            Assert.That(Tag.IsMediaPlaylistTag("#EXT-X-TARGETDURATION"));
            Assert.That(Tag.IsMediaPlaylistTag("#EXT-X-MEDIA-SEQUENCE"));
            Assert.That(Tag.IsMediaPlaylistTag("#EXT-X-DISCONTINUITY-SEQUENCE"));
            Assert.That(Tag.IsMediaPlaylistTag("#EXT-X-ENDLIST"));
            Assert.That(Tag.IsMediaPlaylistTag("#EXT-X-PLAYLIST-TYPE"));
            Assert.That(Tag.IsMediaPlaylistTag("#EXT-X-I-FRAMES-ONLY"));

            Assert.That(Tag.IsMediaPlaylistTag("#EXT-X-INDEPENDENT-SEGMENTS"));
            Assert.That(Tag.IsMediaPlaylistTag("#EXT-X-START"));

        }

        [Test]
        public void TestIsNotMediaPlaylistTag()
        {
            Assert.IsFalse(Tag.IsMediaPlaylistTag("EXT-X-TARGETDURATION"));
            Assert.IsFalse(Tag.IsMediaPlaylistTag("EXT-X-MEDIA-SEQUENCE"));
            Assert.IsFalse(Tag.IsMediaPlaylistTag("EXT-X-DISCONTINUITY-SEQUENCE"));
            Assert.IsFalse(Tag.IsMediaPlaylistTag("EXT-X-ENDLIST"));
            Assert.IsFalse(Tag.IsMediaPlaylistTag("EXT-X-PLAYLIST-TYPE"));
            Assert.IsFalse(Tag.IsMediaPlaylistTag("EXT-X-I-FRAMES-ONLY"));
        }

        [Test]
        public void TestIsMediaSegmentTag()
        {
            Assert.That(Tag.IsMediaSegmentTag("#EXTINF"));
            Assert.That(Tag.IsMediaSegmentTag("#EXT-X-BYTERANGE"));
            Assert.That(Tag.IsMediaSegmentTag("#EXT-X-DISCONTINUITY"));
            Assert.That(Tag.IsMediaSegmentTag("#EXT-X-KEY"));
            Assert.That(Tag.IsMediaSegmentTag("#EXT-X-MAP"));
            Assert.That(Tag.IsMediaSegmentTag("#EXT-X-PROGRAM-DATE-TIME"));
        }

        [Test]
        public void TestIsNotMediaSegmentTag()
        {
            Assert.IsFalse(Tag.IsMediaSegmentTag("EXTINF"));
            Assert.IsFalse(Tag.IsMediaSegmentTag("EXT-X-BYTERANGE"));
            Assert.IsFalse(Tag.IsMediaSegmentTag("EXT-X-DISCONTINUITY"));
            Assert.IsFalse(Tag.IsMediaSegmentTag("EXT-X-KEY"));
            Assert.IsFalse(Tag.IsMediaSegmentTag("EXT-X-MAP"));
            Assert.IsFalse(Tag.IsMediaSegmentTag("EXT-X-PROGRAM-DATE-TIME"));
        }

        [Test]
        public void TestIsMasterTag()
        {
            Assert.That(Tag.IsMasterTag("#EXT-X-MEDIA"));
            Assert.That(Tag.IsMasterTag("#EXT-X-STREAM-INF"));
            Assert.That(Tag.IsMasterTag("#EXT-X-I-FRAME-STREAM-INF"));
            Assert.That(Tag.IsMasterTag("#EXT-X-SESSION-DATA"));

            Assert.That(Tag.IsMasterTag("#EXT-X-INDEPENDENT-SEGMENTS"));
            Assert.That(Tag.IsMasterTag("#EXT-X-START"));
        }

        [Test]
        public void TestIsNotMasterTag()
        {
            Assert.IsFalse(Tag.IsMasterTag("EXT-X-MEDIA"));
            Assert.IsFalse(Tag.IsMasterTag("EXT-X-STREAM-INF"));
            Assert.IsFalse(Tag.IsMasterTag("EXT-X-I-FRAME-STREAM-INF"));
            Assert.IsFalse(Tag.IsMasterTag("EXT-X-SESSION-DATA"));
        }

        [Test]
        public void TestHasNoAttributes()
        {
            Assert.IsFalse(Tag.HasAttributes("#EXT-X-INDEPENDENT-SEGMENTS"));
            Assert.IsFalse(Tag.HasAttributes("#EXT-X-DISCONTINUITY"));
            Assert.IsFalse(Tag.HasAttributes("#EXT-X-I-FRAMES-ONLY"));
            Assert.IsFalse(Tag.HasAttributes("#EXT-X-ENDLIST"));
        }
    }
}
