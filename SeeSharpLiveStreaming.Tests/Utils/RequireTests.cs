using System;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Tests.Utils
{
    [TestFixture]
    public class RequireTests
    {

        [Test]
        public void TestRequireThrowsArgumentNullExceptionForNullParameter()
        {
            object nullable = null;
            var exception = Assert.Throws<ArgumentNullException>(() => nullable.RequireNotNull("nullable"));
            Assert.AreEqual("nullable", exception.ParamName);
        }

        [Test]
        public void TestRequireDoesNotThrowExceptionForNonNullParameter()
        {
            object notNull = new object();
            notNull.RequireNotNull("notNull");
        }
    }
}
