using System;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    [TestFixture]
    public class MediaPlaylistTypeCodeTests
    {

        [Test]
        public void TestMediaPlaylistTypeCodeEventMapsToEvent()
        {
            Assert.AreEqual(MediaPlaylistType.Event, MediaPlaylistTypeCode.ToType(MediaPlaylistTypeCode.Event));
        }

        [Test]
        public void TestMediaPlaylistTypeCodeVodMapsToVideoOnDemand()
        {
            Assert.AreEqual(MediaPlaylistType.VideoOnDemand, MediaPlaylistTypeCode.ToType(MediaPlaylistTypeCode.Vod));
        }

        [Test]
        public void TestMediaPlaylistTypeCodeNullOrEmptyMapsToNone([Values("", null)] string value)
        {
            Assert.AreEqual(MediaPlaylistType.None, MediaPlaylistTypeCode.ToType(value));
        }

        [Test]
        public void TestInvlidMediaPlaylistTypeCodeThrowsArgumentexception()
        {
            Assert.Throws<ArgumentException>(() => MediaPlaylistTypeCode.ToType("invalid value"));
        }

        [Test]
        public void TestMediaPlaylistTypeEventMapsToCodeEvent()
        {
            Assert.AreEqual(MediaPlaylistTypeCode.Event, MediaPlaylistTypeCode.FromType(MediaPlaylistType.Event));
        }

        [Test]
        public void TestMediaPlaylistTypeVideoOnDemandMapsToCodeVod()
        {
            Assert.AreEqual(MediaPlaylistTypeCode.Vod, MediaPlaylistTypeCode.FromType(MediaPlaylistType.VideoOnDemand));
        }

        [Test]
        public void TestMediaPlaylistTypeNoneMapsToCodeEmpty()
        {
            Assert.AreEqual(string.Empty, MediaPlaylistTypeCode.FromType(MediaPlaylistType.None));
        }

        [Test]
        public void TestInvalidMediaPlaylistTypeThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => MediaPlaylistTypeCode.FromType((MediaPlaylistType)32324));
        }
    }
}
