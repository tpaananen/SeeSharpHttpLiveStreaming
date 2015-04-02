using System;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Tests.Helpers;
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

        [Test]
        public void TestPlaylistTypeCtorThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PlaylistType(null));
        }

        [Test]
        public void TestPlaylistTypeCtorThrowsArgumentException([Values("", "invalidValue")] string value)
        {
            Assert.Throws<ArgumentException>(() => new PlaylistType(value));
        }

        [Test]
        public void TestPlaylistTypeSerializes([Values(EnumValues.PlaylistType.Event, EnumValues.PlaylistType.Vod)] string value)
        {
            var playlistType = new PlaylistType(value);
            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            playlistType.Serialize(writer);
            var line = new PlaylistLine(playlistType.TagName, sb.ToString());
            _playlistType.Deserialize(line.GetParameters(), 0);

            Assert.AreEqual(playlistType.PlaylistTypeValue, _playlistType.PlaylistTypeValue);
        }
    }
}
