using System;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Tests.Utils
{
    [TestFixture]
    public class ResolutionTests
    {
        [Test]
        public void TestResolutionEquality()
        {
            var resolution1 = new Resolution(1920, 1080);
            var resolution2 = new Resolution(1920, 1080);

            Assert.AreEqual(resolution1.GetHashCode(), resolution2.GetHashCode());
            Assert.That(resolution1 == resolution2);
            Assert.That(resolution1.Equals((object)resolution2));
            Assert.IsFalse(resolution1.Equals(null));
        }

        [Test]
        public void TestResolutionInequality()
        {
            var resolution1 = new Resolution(1920, 1080);
            var resolution2 = new Resolution(1921, 1080);

            Assert.AreNotEqual(resolution1.GetHashCode(), resolution2.GetHashCode());
            Assert.That(resolution1 != resolution2);
            Assert.IsFalse(resolution1.Equals((object)resolution2));
            Assert.IsFalse(resolution1.Equals(new object()));
        }

        [Test]
        public void TestResolutionThrowsIfValuesAreNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Resolution(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Resolution(0, -1));
        }

        [Test]
        public void TestResolutionStringRepresentation()
        {
            Assert.AreEqual("0" + Resolution.SeparatorChar + "0", Resolution.Default.ToString());
            Assert.AreEqual("1920" + Resolution.SeparatorChar + "1080", new Resolution(1920, 1080).ToString());
        }
    }
}
