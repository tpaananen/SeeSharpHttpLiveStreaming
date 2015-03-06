using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Media.MediaSegment
{
    [TestFixture]
    public class KeyTests
    {

        private Key _key;

        [SetUp]
        public void SetUp()
        {
            _key = new Key();
            Assert.AreEqual("#EXT-X-KEY", _key.TagName);
            Assert.AreEqual(TagType.ExtXKey, _key.TagType);
        }

        private static string GetLine(string encMethod)
        {
            var value = "#EXT-X-KEY:METHOD=" + encMethod;
            if (encMethod.ToEncryptionMethod() == EncryptionMethod.None)
            {
                return value;
            }

            value += ",URI=\"https://example.com/encryption\"";
            value += ",IV=0x1234";
            value += ",KEYFORMAT=\"somevalue\"";
            value += ",KEYFORMATVERSIONS=\"1/4/6\"";

            return value;
        } 

        [Test]
        public void TestKeyIsCreated([Values("NONE", "AES-128", "SAMPLE-AES")] string encryptionMethod)
        {
            var value = GetLine(encryptionMethod);
            _key = (Key)BaseTag.Create(new PlaylistLine("#EXT-X-KEY", value), 5);
            Assert.AreEqual(encryptionMethod.ToEncryptionMethod(), _key.Method);

            if (_key.Method == EncryptionMethod.None)
            {
                AssertMethodIsNone();
                return;
            }

            Assert.AreEqual("https://example.com/encryption", _key.Uri.AbsoluteUri);
            Assert.AreEqual("1234".PadLeft(16, '0'), _key.InitializationVector);
            Assert.AreEqual("somevalue", _key.KeyFormat);
            Assert.AreEqual(new List<int>{1,4,6}, _key.KeyFormatVersions);
        }

        [Test]
        public void TestKeyCreationFailsIfVersionNumberIsLessThanTwo()
        {
            var value = GetLine("AES-128");
            Assert.Throws<SerializationException>(() => BaseTag.Create(new PlaylistLine("#EXT-X-KEY", value), 1));
        }

        [Test]
        public void TestKeyCreationFailsIfKeyFormatIsLessThanFive()
        {
            var value = GetLine("AES-128");
            var exception = Assert.Throws<SerializationException>(() => BaseTag.Create(new PlaylistLine("#EXT-X-KEY", value), 4));
            Assert.AreEqual(typeof(IncompatibleVersionException), exception.InnerException.GetType());

            var ex = (IncompatibleVersionException) exception.InnerException;
            Assert.AreEqual("KEYFORMAT", ex.Attribute);
            Assert.AreEqual(_key.TagName, ex.TagName);
        }

        [Test]
        public void TestKeyCreationFailsIfKeyFormatVersionsIsLessThanFive()
        {
            var value = GetLine("AES-128");
            value = value.Replace(",KEYFORMAT=\"somevalue\"", "");
            var exception = Assert.Throws<SerializationException>(() => BaseTag.Create(new PlaylistLine("#EXT-X-KEY", value), 4));
            Assert.AreEqual(typeof(IncompatibleVersionException), exception.InnerException.GetType());

            var ex = (IncompatibleVersionException) exception.InnerException;
            Assert.AreEqual("KEYFORMATVERSIONS", ex.Attribute);
            Assert.AreEqual(_key.TagName, ex.TagName);
        }

        private void AssertMethodIsNone()
        {
            Assert.IsNull(_key.Uri);


        }

        [Test]
        public void TestKeyParsingThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _key.Deserialize(null, 0));
        }

        [Test]
        public void TestKeyParsingThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _key.Deserialize(string.Empty, 0));
        }
    }
}
