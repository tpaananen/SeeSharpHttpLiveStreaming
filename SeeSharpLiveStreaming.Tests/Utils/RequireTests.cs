using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;
using SeeSharpHttpLiveStreaming.Utils;

// ReSharper disable ExpressionIsAlwaysNull
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
            var notNull = new object();
            notNull.RequireNotNull("notNull");
        }

        [Test]
        public void TestRequireCollectionNotNullThrowsArgumentNullExceptionForNullParameter()
        {
            IReadOnlyCollection<object> nullable = null;
            var exception = Assert.Throws<ArgumentNullException>(() => nullable.RequireNotNull("nullable"));
            Assert.AreEqual("nullable", exception.ParamName);
        }

        [Test]
        public void TestRequireCollectionDoesNotThrowExceptionForNonNullParameter()
        {
            IReadOnlyCollection<object> notNull = new ReadOnlyCollection<object>(new List<object>());
            notNull.RequireNotNull("notNull");
        }

        [Test]
        public void TestRequireCollectionNotEmptyThrowsArgumentExceptionForEmptyParameter()
        {
            IReadOnlyCollection<object> empty = new ReadOnlyCollection<object>(new List<object>());
            Assert.Throws<ArgumentException>(() => empty.RequireNotEmpty("notEmpty"));
        }

        [Test]
        public void TestRequireCollectionNotEmptyDoesNotThrow()
        {
            IReadOnlyCollection<object> notEmpty = new ReadOnlyCollection<object>(new List<object> { new object()});
            notEmpty.RequireNotEmpty("notEmpty");
        }

        [Test]
        public void TestRequireStringNotEmptyThrowsArgumentExceptionForEmptyParameter()
        {
            string empty = string.Empty; 
            Assert.Throws<ArgumentException>(() => empty.RequireNotEmpty("empty"));
        }

        [Test]
        public void TestRequireStringNotEmptyDoesNotThrow()
        {
            const string notEmpty = "121";
            notEmpty.RequireNotEmpty("notEmpty");
        }

        [Test]
        public void TestRequireRequiresThatNoDefaultConstructorWasUsed()
        {
            Assert.Throws<InvalidOperationException>(() => new ByteRange().RequireNoDefaultConstructor());
        }

        [Test]
        public void TestRequireRequiresThatNoDefaultConstructorWasUsedOk()
        {
            Assert.DoesNotThrow(() => new ByteRange(12,12).RequireNoDefaultConstructor());
        }
    }
}
