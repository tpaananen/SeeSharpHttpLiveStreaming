using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using PlaylistType = SeeSharpHttpLiveStreaming.Playlist.Tags.Media.PlaylistType;
using EnumValues = SeeSharpHttpLiveStreaming.Playlist;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Media
{
    [TestFixture]
    public class PlaylistTypeTests
    {
        private PlaylistType _playlistType;

        [SetUp]
        public void SetUp()
        {
            _playlistType = new PlaylistType();
            Assert.AreEqual("#EXT-X-PLAYLIST-TYPE", _playlistType.TagName);
            Assert.AreEqual(TagType.ExtXPlaylistType, _playlistType.TagType);
        }

        [Test]
        public void TestPlaylistTypeThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _playlistType.Deserialize(null, 0));
        }

        [Test]
        public void TestPlaylistTypeThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _playlistType.Deserialize(string.Empty, 0));
        }

        [Test]
        public void TestPlaylistTypeDeserializationFailsIfInvalidValueProvided()
        {
            Assert.Throws<SerializationException>(() => _playlistType.Deserialize("FOO", 0));
        }

        [Test]
        public void TestPlaylistTypeIsParsed([Values(EnumValues.PlaylistType.Event, 
                                                     EnumValues.PlaylistType.Vod)] string value)
        {
            _playlistType.Deserialize(value, 0);
            Assert.AreEqual(value, _playlistType.PlaylistTypeValue);
        }

        
    }
}
