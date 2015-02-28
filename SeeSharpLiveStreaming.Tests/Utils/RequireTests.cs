using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SeeSharpLiveStreaming.Utils;

namespace SeeSharpLiveStreaming.Tests.Utils
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
