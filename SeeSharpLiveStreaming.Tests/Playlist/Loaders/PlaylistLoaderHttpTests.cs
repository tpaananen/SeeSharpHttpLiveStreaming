using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Loaders;
using SeeSharpHttpLiveStreaming.Tests.Helpers;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Loaders
{
    [TestFixture]
    public class PlaylistLoaderHttpTests : PlaylistReadingTestBase
    {
        private Uri _uri;
        private ushort _port;

        [SetUp]
        public void SetUp()
        {
            _port = 65034;
            _uri = new Uri(string.Format("{0}://{1}:{2}/GetTempFile/temp.m3u8", Uri.UriSchemeHttp, "localhost", _port));
        }

        [Datapoints] 
        public static readonly string[] ContentTypes = PlaylistLoader.ValidContentTypes.ToArray();

        [Theory]
        public void TestPlaylistLoaderLoadsFromHttpServer(string contentType)
        {
            using (var server = new TestWebServer())
            {
                var content = CreateValidMasterPlaylist("\n");
                server.Listen(_port, contentType, content);

                var loader = new PlaylistLoader();
                var loadedContent = loader.Load(_uri);
                Assert.AreEqual(content, loadedContent);
            }
        }

        [Theory]
        public async Task TestPlaylistLoaderLoadsFromHttpServerAsync(string contentType)
        {
            using (var server = new TestWebServer())
            {
                var content = CreateValidMasterPlaylist("\n");
                server.Listen(_port, contentType, content);

                var loader = new PlaylistLoader();
                var loadedContent = await loader.LoadAsync(_uri).ConfigureAwait(false);
                Assert.AreEqual(content, loadedContent);
            }
        }

        [Test]
        public void TestPlaylistLoaderRefusesToLoadPlaylistFromHttpServerIfContentTypeIsInvalid()
        {
            using (var server = new TestWebServer())
            {
                var content = CreateValidMasterPlaylist("\n");
                server.Listen(_port, "invalid.content.type", content);

                var loader = new PlaylistLoader();
                Assert.Throws<IOException>(() => loader.Load(_uri));
            }
        }

        [Test]
        public void TestPlaylistLoaderRefusesToLoadPlaylistFromHttpServerIfContentTypeIsInvalidAsync()
        {
            using (var server = new TestWebServer())
            {
                var content = CreateValidMasterPlaylist("\n");
                server.Listen(_port, "invalid.content.type", content);

                var loader = new PlaylistLoader();
                Assert.Throws<IOException>(async () => await loader.LoadAsync(_uri).ConfigureAwait(false));
            }
        }
    }
}
