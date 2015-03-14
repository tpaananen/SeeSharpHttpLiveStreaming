﻿using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.MasterOrMedia;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.MasterOrMedia
{
    [TestFixture]
    public class StartTagTests
    {
        private StartTag _startTag;

        private const string ValidAttributes = "TIME-OFFSET=12343.33,PRECISE={0}";

        [SetUp]
        public void SetUp()
        {
            _startTag = new StartTag();
            Assert.AreEqual("#EXT-X-START", _startTag.TagName);
            Assert.AreEqual(TagType.ExtXStart, _startTag.TagType);
        }

        [Test]
        public void TestStartTagThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _startTag.Deserialize(null, 0));
        }

        [Test]
        public void TestStartTagThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _startTag.Deserialize(string.Empty, 0));
        }

        [Test]
        public void TestStartTagIsCreatedWithPrecise([Values(YesNo.Yes, YesNo.No, "")] string precise)
        {
            _startTag.Deserialize(string.Format(ValidAttributes, precise), 0);
            Assert.AreEqual(12343.33m, _startTag.TimeOffset);
            Assert.AreEqual(precise == YesNo.Yes, _startTag.Precise);
        }

        [Test]
        public void TestStartTagIsCreatedWithoutPrecise()
        {
            _startTag.Deserialize("TIME-OFFSET=12343.21", 0);
            Assert.AreEqual(12343.21m, _startTag.TimeOffset);
            Assert.IsFalse(_startTag.Precise);
        }

        [Test]
        public void TestStartTagIsNotCreatedIfTimeOffsetIsNotPresent()
        {
            Assert.Throws<SerializationException>(() => _startTag.Deserialize("PRECISE=YES", 0));
        }

        [Test]
        public void TestStartTagIsNotCreatedIfTimeOffsetIsInvalid()
        {
            Assert.Throws<SerializationException>(() => _startTag.Deserialize("TIME-OFFSET=asdv", 0));
        }

        [Test]
        public void TestStartTagIsNotCreatedIfPreciseHasInvalidValue()
        {
            Assert.Throws<SerializationException>(() => _startTag.Deserialize(string.Format(ValidAttributes, "232"), 0));
        }
    }
}
