﻿using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags.Media.MediaSegment
{
    [TestFixture]
    public class ExtXByteRangeTests
    {
        private ByteRange _byteRange;

        [SetUp]
        public void SetUp()
        {
            _byteRange = new ByteRange();
            Assert.AreEqual("#EXT-X-BYTERANGE", _byteRange.TagName);
            Assert.AreEqual(TagType.ExtXByteRange, _byteRange.TagType);
        }

        [Test]
        public void TestParseExtXByteRangeWithStartIndex()
        {
            const string validValue = "11211@1212";
            _byteRange.Deserialize(validValue, 7);
            Assert.AreEqual(11211, _byteRange.Length);
            Assert.AreEqual(1212, _byteRange.StartIndex);
        }

        [Test]
        public void TestParseExtXByteRangeWithoutStartIndex()
        {
            const string validValue = "11211";
            _byteRange.Deserialize(validValue, 7);
            Assert.AreEqual(11211, _byteRange.Length);
            Assert.AreEqual(0, _byteRange.StartIndex);
        }

        [Test]
        public void TestParseExtXByteRangeRefusesToParseIfVersionIsBelowFour()
        {
            const string validValue = "11211";
            Assert.Throws<IncompatibleVersionException>(() => _byteRange.Deserialize(validValue, 3));
        }

        [Test]
        public void TestParseExtXByteRangeFailsWithInvalidInput()
        {
            const string invalidValue = "11211@1212@121";
            Assert.Throws<SerializationException>(() => _byteRange.Deserialize(invalidValue, 7));
        }

        [Test]
        public void TestParseExtXByteRangeFailsWithEmptyInput()
        {
            Assert.Throws<ArgumentException>(() => _byteRange.Deserialize(string.Empty, 7));
        }

        [Test]
        public void TestParseExtXByteRangeFailsWithNullInput()
        {
            Assert.Throws<ArgumentNullException>(() => _byteRange.Deserialize(null, 7));
        }

        [Test]
        public void TestByteRangesAreEqual()
        {
            var first = new ByteRange(1, 1);
            var second = new ByteRange(1, 1);
            Assert.That(first.Equals(first));
            Assert.That(first.Equals((object)first));
            Assert.AreEqual(first, second);
            Assert.That(first == second);
            Assert.That(first.Equals((object)second));
            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }

        [Test]
        public void TestByteRangesAreNotEqual()
        {
            var first = new ByteRange(1, 2);
            var second = new ByteRange(2, 1);
            Assert.AreNotEqual(first, second);
            Assert.That(first != second);
            Assert.IsFalse(first.Equals(null));
            Assert.IsFalse(first.Equals((object)null));
            Assert.IsFalse(first.Equals(new object()));
        }
    }
}
