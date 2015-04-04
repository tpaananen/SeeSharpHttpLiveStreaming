using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;
using SeeSharpHttpLiveStreaming.Tests.Helpers;
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

        private static string GetLine(EncryptionMethod encMethod)
        {
            var value = "#EXT-X-KEY:METHOD=" + encMethod.FromEncryptionMethod();
            if (encMethod == EncryptionMethod.None)
            {
                return value;
            }

            value += ",URI=\"https://example.com/encryption\"";
            value += ",IV=0x1234";
            value += ",KEYFORMAT=\"somevalue\"";
            value += ",KEYFORMATVERSIONS=\"1/4/6\"";

            return value;
        } 

        [Theory]
        public void TestKeyIsCreated(EncryptionMethod encryptionMethod)
        {
            var value = GetLine(encryptionMethod);
            _key = (Key)TagFactory.Create(new PlaylistLine("#EXT-X-KEY", value), 5);
            Assert.AreEqual(encryptionMethod, _key.Method);

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
            var value = GetLine(EncryptionMethod.Aes128);
            value = value.Replace(",IV=0x1234", "");
            _key = (Key)TagFactory.Create(new PlaylistLine("#EXT-X-KEY", value), 5);

            Assert.AreEqual("https://example.com/encryption", _key.Uri.AbsoluteUri);
            Assert.AreEqual("", _key.InitializationVector);
            Assert.AreEqual("somevalue", _key.KeyFormat);
            Assert.AreEqual(new List<int>{1,4,6}, _key.KeyFormatVersions);
        }

        [Test]
        public void TestKeyCreationFailsIfVersionNumberIsLessThanRequiredVersion([Range(0, 1)] int version)
        {
            var value = GetLine(EncryptionMethod.Aes128);
            var exception = Assert.Throws<SerializationException>(() => TagFactory.Create(new PlaylistLine("#EXT-X-KEY", value), version));
            Assert.AreEqual(typeof(IncompatibleVersionException), exception.InnerException.GetType());

            var ex = (IncompatibleVersionException) exception.InnerException;
            Assert.AreEqual("IV", ex.Attribute);
            Assert.AreEqual(_key.TagName, ex.TagName);
        }

        [Test]
        public void TestKeyCreationFailsIfKeyFormatIsLessThanRequiredVersion([Range(0, 4)] int version)
        {
            var value = GetLine(EncryptionMethod.Aes128).Replace(",IV=0x1234", "");
            var exception = Assert.Throws<SerializationException>(() => TagFactory.Create(new PlaylistLine("#EXT-X-KEY", value), version));
            Assert.AreEqual(typeof(IncompatibleVersionException), exception.InnerException.GetType());

            var ex = (IncompatibleVersionException) exception.InnerException;
            Assert.AreEqual("KEYFORMAT", ex.Attribute);
            Assert.AreEqual(_key.TagName, ex.TagName);
        }

        [Test]
        public void TestKeyCreationFailsIfKeyFormatVersionsIsLessThanRequired([Range(0, 4)] int version)
        {
            var value = GetLine(EncryptionMethod.Aes128);
            value = value.Replace(",KEYFORMAT=\"somevalue\"", "").Replace(",IV=0x1234", "");
            var exception = Assert.Throws<SerializationException>(() => TagFactory.Create(new PlaylistLine("#EXT-X-KEY", value), version));
            Assert.AreEqual(typeof(IncompatibleVersionException), exception.InnerException.GetType());

            var ex = (IncompatibleVersionException) exception.InnerException;
            Assert.AreEqual("KEYFORMATVERSIONS", ex.Attribute);
            Assert.AreEqual(_key.TagName, ex.TagName);
        }

        [Test]
        public void TestKeyCreationSucceedsEvenWithoutKeFormatVersions()
        {
            var value = GetLine(EncryptionMethod.Aes128);
            value = value.Replace(",KEYFORMATVERSIONS=\"1/4/6\"", "");
            var key = (Key)TagFactory.Create(new PlaylistLine("#EXT-X-KEY", value), 5);
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
        public void TestFromEncryptionMethodThrowsForInvalidMethod()
        {
            Assert.Throws<ArgumentException>(() => ((EncryptionMethod)14567).FromEncryptionMethod());
        }

        [Test]
        public void TestToEncryptionMethods()
        {
            Assert.AreEqual(EncryptionMethod.None, "NONE".ToEncryptionMethod());
            Assert.AreEqual(EncryptionMethod.Aes128, "AES-128".ToEncryptionMethod());
            Assert.AreEqual(EncryptionMethod.SampleAes, "SAMPLE-AES".ToEncryptionMethod());
        }

        [Test]
        public void TestFromEncryptionMethods()
        {
            Assert.AreEqual("NONE", EncryptionMethod.None.FromEncryptionMethod());
            Assert.AreEqual("AES-128", EncryptionMethod.Aes128.FromEncryptionMethod());
            Assert.AreEqual("SAMPLE-AES", EncryptionMethod.SampleAes.FromEncryptionMethod());
        }

        [Test]
        public void TestKeyCtorThrowsArgumentNullException([Values(EncryptionMethod.Aes128, EncryptionMethod.SampleAes)] EncryptionMethod encryptionMethod)
        {
            Assert.Throws<ArgumentNullException>(() => new Key(encryptionMethod));
        }

        [Test]
        public void TestKeyCtorThrowsIncompatibleVersionExceptionIfIvIsLessThanRequiredVersion([Values(EncryptionMethod.Aes128, EncryptionMethod.SampleAes)] EncryptionMethod method,[Range(0, 1)] int version)
        {
            Assert.Throws<IncompatibleVersionException>(() => new Key(method, version, new Uri("http://e.com"), "0x1233"));
        }

        [Test]
        public void TestKeyCtorThrowsIncompatibleVersionExceptionIfKeyFormatIsLessThanRequiredVersion([Values(EncryptionMethod.Aes128, EncryptionMethod.SampleAes)] EncryptionMethod method,[Range(0, 4)] int version)
        {
            Assert.Throws<IncompatibleVersionException>(() => new Key(method, version, new Uri("http://e.com"), null, "1212"));
        }

        [Test]
        public void TestKeyCtorThrowsIncompatibleVersionExceptionIfKeyFormatVersionIsLessThanRequiredVersion([Values(EncryptionMethod.Aes128, EncryptionMethod.SampleAes)] EncryptionMethod method,[Range(0, 4)] int version)
        {
            Assert.Throws<IncompatibleVersionException>(() => new Key(method, version, new Uri("http://e.com"), null, null, new [] { 1, 2, 4}));
        }

        [Theory]
        public void TestKeyIsCreatedUsingPublicCtor(
            EncryptionMethod encryptionMethod, 
            [Values(null, "", "0x43434")] string iv, 
            [Values(null, "", "HOO")] string keyFormat, 
            [Values(null, "", "hoo")] string keyFormatVersions)
        {
            var kfv = keyFormatVersions == null ? new[] {1, 3, 5} : new int[0];
            var uri = new Uri("http://e.com/");
            var key = new Key(encryptionMethod, 5, uri, iv, keyFormat, kfv);

            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);

            key.Serialize(writer);
            var line = new PlaylistLine(key.TagName, sb.ToString());
            _key.Deserialize(line.GetParameters(), 5);

            AssertAreEqual(_key, key);
        }

        [Theory]
        public void TestKeyIsCreatedUsingPublicCtorWithoutIvAndKeyFormatsWithLessThanRequiredVersion(
            EncryptionMethod encryptionMethod, 
            [Values(null, "")] string iv, 
            [Values(null, "")] string keyFormat, 
            [Values(null, "")] string keyFormatVersions)
        {
            var kfv = keyFormatVersions == null ? null : new int[0];
            var uri = new Uri("http://e.com/");
            var key = new Key(encryptionMethod, 1, uri, iv, keyFormat, kfv);

            StringBuilder sb;
            var writer = TestPlaylistWriterFactory.CreateWithStringBuilder(out sb);

            key.Serialize(writer);
            var line = new PlaylistLine(key.TagName, sb.ToString());
            _key.Deserialize(line.GetParameters(), 5);

            AssertAreEqual(_key, key);
        }

        private static void AssertAreEqual(Key expected, Key actual)
        {
            Assert.AreEqual(expected.Method, actual.Method);
            Assert.AreEqual(expected.Uri, actual.Uri);
            Assert.AreEqual(expected.InitializationVector, actual.InitializationVector);
            Assert.AreEqual(expected.KeyFormat, actual.KeyFormat);
            Assert.AreEqual(expected.KeyFormatVersions, actual.KeyFormatVersions);
        }
    }
}
