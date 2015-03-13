using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Master;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Master
{
    [TestFixture]
    public class SessionDataTests
    {
        private SessionData _sessionData;

        private const string ValidValueWithUri = "DATA-ID=\"com.example.movie.title\",URI=\"http://www.example.com/\",LANGUAGE=\"en\"";
        private const string ValidValueWithValue = "DATA-ID=\"com.example.movie.title\",VALUE=\"This is a human readable value.\",LANGUAGE=\"en\"";
        private const string InvalidValueWithBothUriAndValue = ValidValueWithUri + ",VALUE=\"This Should Not Exist But It Does\"";

        [SetUp]
        public void SetUp()
        {
            _sessionData = new SessionData();
            Assert.AreEqual("#EXT-X-SESSION-DATA", _sessionData.TagName);
            Assert.AreEqual(TagType.ExtXSessionData, _sessionData.TagType);
        }

        [Test]
        public void TestSessionDataThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sessionData.Deserialize(null, 0));
        }

        [Test]
        public void TestSessionDataThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sessionData.Deserialize(string.Empty, 0));
        }

        [Test]
        public void TestSessionDataWithUriIsParsed()
        {
            _sessionData.Deserialize(ValidValueWithUri, 0);
            Assert.AreEqual("com.example.movie.title", _sessionData.DataId);
            Assert.AreEqual(new Uri("http://www.example.com/"), _sessionData.Uri);
            Assert.AreEqual(null, _sessionData.Value);
            Assert.AreEqual("en", _sessionData.Language);
        }

        [Test]
        public void TestSessionDataWithValueIsParsed()
        {
            _sessionData.Deserialize(ValidValueWithValue, 0);
            Assert.AreEqual("com.example.movie.title", _sessionData.DataId);
            Assert.AreEqual(null, _sessionData.Uri);
            Assert.AreEqual("This is a human readable value.", _sessionData.Value);
            Assert.AreEqual("en", _sessionData.Language);
        }

        [Test]
        public void TestSessionDataFailsToParseIfBothUriAndValueExist()
        {
            Assert.Throws<SerializationException>(() => _sessionData.Deserialize(InvalidValueWithBothUriAndValue, 0));
        }

        [Test]
        public void TestSessionDataFailsToParseIfNotUriNorValueExists()
        {
            Assert.Throws<SerializationException>(() => _sessionData.Deserialize("DATA-ID=\"com.example.movie.title\"", 0));
        }
    }
}
