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
                return;
            }

            Assert.AreEqual("https://example.com/encryption", _key.Uri.AbsoluteUri);
            Assert.AreEqual("1234".PadLeft(16, '0'), _key.InitializationVector);
            Assert.AreEqual("somevalue", _key.KeyFormat);
            Assert.AreEqual(new List<int>{1,4,6}, _key.KeyFormatVersions);
        }

        [Test]
        public void TestKeyIsCreatedWithOutInitializationVector()
        {
            var value = GetLine("AES-128");
            value = value.Replace(",IV=0x1234", "");
            _key = (Key)BaseTag.Create(new PlaylistLine("#EXT-X-KEY", value), 5);

            Assert.AreEqual("https://example.com/encryption", _key.Uri.AbsoluteUri);
            Assert.AreEqual("", _key.InitializationVector);
            Assert.AreEqual("somevalue", _key.KeyFormat);
            Assert.AreEqual(new List<int>{1,4,6}, _key.KeyFormatVersions);
        }

        [Test]
        public void TestKeyCreationFailsIfVersionNumberIsLessThanTwo()
        {
            var value = GetLine("AES-128");
            var exception = Assert.Throws<SerializationException>(() => BaseTag.Create(new PlaylistLine("#EXT-X-KEY", value), 1));
            Assert.AreEqual(typeof(IncompatibleVersionException), exception.InnerException.GetType());

            var ex = (IncompatibleVersionException) exception.InnerException;
            Assert.AreEqual("IV", ex.Attribute);
            Assert.AreEqual(_key.TagName, ex.TagName);
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

        [Test]
        public void TestKeyCreationSucceedsEvenWithoutKeFormatVersions()
        {
            var value = GetLine("AES-128");
            value = value.Replace(",KEYFORMATVERSIONS=\"1/4/6\"", "");
            var key = (Key)BaseTag.Create(new PlaylistLine("#EXT-X-KEY", value), 5);
            Assert.IsEmpty(key.KeyFormatVersions);
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

        [Test]
        public void TestKeyThrowsIfAttributesPresentAndNoEncryptionIsUsed()
        {
            var exception = Assert.Throws<SerializationException>(() => _key.Deserialize("METHOD=NONE,KEYFORMAT=\"some\"", 0));
            Assert.AreEqual(typeof(SerializationException), exception.InnerException.GetType());
        }

        [Test]
        public void TestToEncryptionMethodThrowsForInvalidMethod()
        {
            Assert.Throws<ArgumentException>(() => "FOO".ToEncryptionMethod());
        }

        [Test]
        public void TestToEncryptionMethods()
        {
            Assert.AreEqual(EncryptionMethod.None, "NONE".ToEncryptionMethod());
            Assert.AreEqual(EncryptionMethod.Aes128, "AES-128".ToEncryptionMethod());
            Assert.AreEqual(EncryptionMethod.SampleAes, "SAMPLE-AES".ToEncryptionMethod());
        }
    }
}
