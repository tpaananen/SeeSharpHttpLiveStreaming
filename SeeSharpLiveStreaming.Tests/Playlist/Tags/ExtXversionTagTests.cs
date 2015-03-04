using System;
using System.Globalization;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Tags.BasicTags;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags
{
    [TestFixture]
    public class ExtXversionTagTests
    {
        private ExtXVersion _tag;

        [SetUp]
        public void SetUp()
        {
            _tag = new ExtXVersion();
        }

        [Test]
        public void ExtXVersionTagParsesTheVersionNumber([Range(0, 10)] int version)
        {
            string parameters = version.ToString(CultureInfo.InvariantCulture);
            _tag.Deserialize(parameters, 0);
            Assert.AreEqual(version, _tag.Version);
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

    }
}
