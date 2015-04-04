using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Loaders;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Tests.Helpers;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Loaders
{
    [TestFixture]
    public class PlaylistLoaderTests
    {
        private IPlaylistLoader _loader;

        [SetUp]
        public void SetUp()
        {
            var factory = new PlaylistLoaderFactory();
            _loader = factory.Create();
        }

        [Test]
        public void TestPlaylistLoaderThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _loader.Load(null));
        }

        [Test]
        public void TestPlaylistLoaderThrowsArgumentNullExceptionFromAsync()
        {
            Assert.Throws<ArgumentNullException>(async () => await _loader.LoadAsync(null));
        }

        [Test]
        public void TestPlaylistLoaderHandlesExceptionWhileLoadingContent()
        {
            var path = Assembly.GetExecutingAssembly().Location + Path.GetRandomFileName();
            Assert.Throws<IOException>(() => _loader.Load(new Uri(path)));
        }

        [Test]
        public void TestPlaylistLoaderHandlesExceptionWhileLoadingContentAsync()
        {
            var path = Assembly.GetExecutingAssembly().Location + Path.GetRandomFileName();
            Assert.Throws<IOException>(async () => await _loader.LoadAsync(new Uri(path)));
        }

        [Datapoints]
        public readonly string[] Extensions = PlaylistLoader.ValidFileExtensions.ToArray();

        [Theory]
        public void TestPlaylistLoaderLoadsThePlaylist(string extension)
        {
            const string originalContent = Tag.StartLine;
            TempFileCreator.RunInSafeContext(originalContent, extension, uri =>
            {
                var content = _loader.Load(uri);
                Assert.AreEqual(originalContent, content);
            });
        }

        [Theory]
        public async Task TestPlaylistLoaderLoadsThePlaylistAsync(string extension)
        {
            const string originalContent = Tag.StartLine;
            await TempFileCreator.RunInSafeContextAsync(originalContent, extension, async uri =>
            {
                var content = await _loader.LoadAsync(uri).ConfigureAwait(false);
                Assert.AreEqual(originalContent, content);
            });
        }

        [Test]
        public void TestPlaylistLoaderRefuseToLoadPlaylistWithInvalidFileExtensionAndInvalidContentType()
        {
            const string originalContent = Tag.StartLine;
            TempFileCreator.RunInSafeContext(originalContent, ".invalid", uri => 
                Assert.Throws<IOException>(() => _loader.Load(uri)));
        }

        [Test]
        public void TestPlaylistLoaderRefuseToLoadPlaylistWithInvalidFileExtensionAndInvalidContentTypeAsync()
        {
            const string originalContent = Tag.StartLine;
            Assert.Throws<IOException>(async () => 
                await TempFileCreator.RunInSafeContextAsync(originalContent, ".invalid", async uri =>
                {
                    await _loader.LoadAsync(uri);
                })
            );
        }

    }
}
