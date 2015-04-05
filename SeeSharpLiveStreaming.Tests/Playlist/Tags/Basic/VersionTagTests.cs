using System;
using System.Globalization;
using System.Text;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Tests.Helpers;
using Version = SeeSharpHttpLiveStreaming.Playlist.Tags.BasicTags.Version;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Basic
{
    [TestFixture]
    public class VersionTagTests
    {
        private Version _tag;

        [SetUp]
        public void SetUp()
        {
            _tag = new Version();
            Assert.AreEqual("#EXT-X-VERSION", _tag.TagName);
            Assert.AreEqual(TagType.ExtXVersion, _tag.TagType);
        }

        [Test]
        public void ExtXVersionTagParsesTheVersionNumber([Range(0, 7)] int version)
        {
            string parameters = version.ToString(CultureInfo.InvariantCulture);
            _tag.Deserialize(parameters, 0);
            Assert.AreEqual(version, _tag.VersionNumber);
        }

        [Test]
        public void ExtXVersionTagThrowsInvalidOperationExceptionIfIncomingVersionIsNonZero()
        {
            Assert.Throws<InvalidOperationException>(() => _tag.Deserialize("3", 1));
        }

        [Test]
        public void ExtXVersionTagThrowsSerializationExceptionIfParsingOfVersioNumberFails()
        {
            Assert.Throws<FormatException>(() => _tag.Deserialize("NA", 0));
        }

        [Test]
        public void TestVersionNumberThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _tag.Deserialize(null, 0));
        }

        [Test]
        public void TestVersionNumberThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _tag.Deserialize(string.Empty, 0));
        }

        [Test]
        public void TestVersionNumberSerializationThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Version(12).Serialize(null));
        }

        [Test]
        public void TestVersionNumberIsSerializedCorrectly()
        {
            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);
            _tag = new Version(7);
            _tag.Serialize(writer);
            Assert.AreEqual(_tag.TagName + Tag.TagEndMarker + 7.ToString(CultureInfo.InvariantCulture) + Environment.NewLine, sb.ToString());
        }
    }
}
