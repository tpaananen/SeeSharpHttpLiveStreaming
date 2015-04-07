using System;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Tests.Utils
{
    [TestFixture]
    public class UriUtilsTests
    {
        private static readonly Uri BaseUri = new Uri("http://go.com/");

        [Test]
        public void TestUriUtilCreatesFromAbsoluteUri()
        {
            Assert.IsTrue(BaseUri.IsAbsoluteUri);
            string absUriString = BaseUri.AbsoluteUri;
            var uri = UriUtils.CreateUri(absUriString, BaseUri);
            Assert.AreEqual(BaseUri, uri);
        }

        [Test]
        public void TestUriUtilCreatesFromRelativeUri()
        {
            Assert.IsTrue(BaseUri.IsAbsoluteUri);
            const string relUriString = "home/land/index.php";
            var uri = UriUtils.CreateUri(relUriString, BaseUri);
            Assert.AreEqual(BaseUri.AbsoluteUri + relUriString, uri.AbsoluteUri);
        }
    }
}
