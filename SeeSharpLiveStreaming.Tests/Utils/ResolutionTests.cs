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
            Assert.IsFalse(resolution1.Equals((object)null));
        }

        [Test]
        public void TestResolutionInequality()
        {
            var resolution1 = new Resolution(1920, 1080);
            var resolution2 = new Resolution(1921, 1080);

            Assert.AreNotEqual(resolution1.GetHashCode(), resolution2.GetHashCode());
            Assert.That(resolution1 != resolution2);
            Assert.That(!resolution1.Equals((object)resolution2));
        }
    }
}
