using System;
using System.Globalization;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
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
        public void ExtXVersionTagParsesTheVersionNumber([Range(0, 10)] int version)
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
            Assert.Throws<SerializationException>(() => _tag.Deserialize("NA", 0));
        }

        [Test]
        public void TestVersionIsZeroIfNoContent()
        {
            _tag.Deserialize("", 0);
            Assert.AreEqual(0, _tag.VersionNumber);
        }

        [Test]
        public void TestVersionNumberThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _tag.Deserialize(null, 0));
        }

    }
}
