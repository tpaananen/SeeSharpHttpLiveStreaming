using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Moq;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Tests.Helpers;
using SeeSharpHttpLiveStreaming.Utils.Writers;
using Version = SeeSharpHttpLiveStreaming.Playlist.Tags.BasicTags.Version;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags
{
    [TestFixture]
    public class BaseTagTests
    {
        [Datapoints]
        public IEnumerable<string> Tags = TagFactory.TypeMapping.Keys.ToList();

        

        [Theory]
        public void TestTagsReportsHasAttributesCorrectly(string tagName)
        {
            var tag = TagFactory.Create(tagName);
            Assert.AreEqual(Tag.HasAttributes(tagName), tag.HasAttributes);
        }

        [Theory]
        public void TestSerializeThrowsIfTagCreatedWithDefaultCtor(string tagName)
        {
            var tag = TagFactory.Create(tagName);
            if (tag.HasAttributes)
            {
                Assert.Throws<InvalidOperationException>(() => tag.Serialize(TestPlaylistWriterFactory.Create()));
            }
            else
            {
                Assert.DoesNotThrow(() => tag.Serialize(TestPlaylistWriterFactory.Create()));
            }
        }

        [Test]
        public void TestSerializeThrowsSerializationExceptionIfAnErrorOccurs()
        {
            var mockWriter = new Mock<IPlaylistWriter>();
            mockWriter.Setup(x => x.Write(It.IsAny<string>())).Throws<IOException>().Verifiable();

            var tag = new Version(12);
            Assert.Throws<SerializationException>(() => tag.Serialize(mockWriter.Object));
            mockWriter.VerifyAll();
        }
    }
}
