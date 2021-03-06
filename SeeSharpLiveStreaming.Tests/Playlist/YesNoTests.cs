﻿using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    [TestFixture]
    public class YesNoTests
    {
        [Test]
        public void TestYesNoFromBooleanReturnsYes()
        {
            Assert.AreEqual(YesNo.Yes, YesNo.FromBoolean(true));
        }

        [Test]
        public void TestYesNoFromBooleanReturnsNo()
        {
            Assert.AreEqual(YesNo.No, YesNo.FromBoolean(false, true));
        }

        [Test]
        public void TestYesNoFromBooleanReturnsEmptyString()
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            Assert.AreEqual(string.Empty, YesNo.FromBoolean(false, false));
        }

        [Test]
        public void TestYesNoFromStringReturnsTrue()
        {
            Assert.IsTrue(YesNo.FromString(YesNo.Yes));
        }

        [Test]
        public void TestYesNoFromStringReturnsFalse([Values(YesNo.No, "")] string value)
        {
            Assert.IsFalse(YesNo.FromString(value));
        } 
    }
}
