using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Tests.Utils
{
    [TestFixture]
    public class IncompatibleVersionExceptionTests
    {
        [Test]
        public void TestIncompatibleVersionException()
        {
            var exception = new IncompatibleVersionException(new ByteRange(), "", 12, 122);
            var info = new SerializationInfo(typeof (IncompatibleVersionException), new FormatterConverter());
            exception.GetObjectData(info, new StreamingContext());
            Assert.AreEqual(exception.TagName, info.GetValue("TagName", typeof(string)));
            Assert.AreEqual(exception.Attribute, info.GetValue("Attribute", typeof(string)));
            Assert.AreEqual(exception.RequiredVersion, info.GetValue("RequiredVersion", typeof(int)));
            Assert.AreEqual(exception.CurrentVersion, info.GetValue("CurrentVersion", typeof(int)));
        }

    }
}
