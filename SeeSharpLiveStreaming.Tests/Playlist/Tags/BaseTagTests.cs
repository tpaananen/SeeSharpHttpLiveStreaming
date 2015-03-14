using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        [Datapoints]
        public IEnumerable<string> Tags = TagFactory.TypeMapping.Keys.ToList();

        [Test]
        public void TestBaseTagCreateThrowsIfStartTagIsTriedToCreate()
        {
            var line = new PlaylistLine(Tag.StartLine, Tag.StartLine);
            Assert.Throws<InvalidOperationException>(() => BaseTag.Create(line, 0));
        }

        [Theory]
        public void TestTagsReportsHasAttributesCorrectly(string tagName)
        {
            var tag = TagFactory.Create(tagName);
            Assert.AreEqual(Tag.HasAttributes(tagName), tag.HasAttributes);
        }

    }
}
