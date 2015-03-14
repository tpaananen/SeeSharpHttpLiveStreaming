using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Tags
{
    [TestFixture]
    public class BaseTagTests
    {

        [Test]
        public void TestBaseTagCreateThrowsIfStartTagIsTriedToCreate()
        {
            var line = new PlaylistLine(Tag.StartLine, Tag.StartLine);
            Assert.Throws<InvalidOperationException>(() => BaseTag.Create(line, 0));
        }

        // TODO: remove test when Serialize is abstract
        [Test]
        public void TestBaseTagThrowsNotImplementedExceptionIfDerivedClassDoesNotImplementSerialize()
        {
            var tags = TagFactory.TypeMapping;

            foreach (var tagName in tags.Keys)
            {
                var tag = TagFactory.Create(tagName);
                var methodInfo = tag.GetType().GetMethod("Serialize").GetBaseDefinition();
                
                if (methodInfo.DeclaringType != tag.GetType())
                {
                    using (var writer = new PlaylistWriter(new StringWriter(new StringBuilder())))
                    {
                        Assert.Throws<NotImplementedException>(() => tag.Serialize(writer), "BaseTag must throw NotImplementedException.");
                    }
                }
                else
                {
                    using (var writer = new PlaylistWriter(new StringWriter(new StringBuilder())))
                    {
                        Assert.DoesNotThrow(() => tag.Serialize(writer), "Tag " + tagName + " throws exception from Serialize.");
                    }
                }
            }
        }

    }
}
