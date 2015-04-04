using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Loaders;
using SeeSharpHttpLiveStreaming.Tests.Helpers;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    [TestFixture]
    public class HlsPlaylistCreatorTests : PlaylistReadingTestBase
    {

        private HlsPlaylistCreator _creator;
        private readonly string _extension = PlaylistLoader.ValidFileExtensions.ElementAt(0);

        [SetUp]
        public void SetUp()
        {
            _creator = new HlsPlaylistCreator();
        }

        [Datapoints]
        public static string[] NewLines = { "\r\n", "\n" };

        [Theory]
        public void TestParserCreatesMasterPlaylist(string newLine)
        {
            var playlist = CreateValidMasterPlaylist(newLine);
            
            TempFileCreator.RunInSafeContext(playlist, _extension, uri =>
            {
                var playlistObject = _creator.CreateFrom(uri);
                AssertMasterPlaylist(playlistObject);
            });
        }

        [Theory]
        public void TestParserCreatesMediaPlaylist(string newLine)
        {
            var playlist = CreateValidMediaPlaylist(newLine);
            TempFileCreator.RunInSafeContext(playlist, _extension, uri =>
            {
                var playlistObject = _creator.CreateFrom(uri);
                AssertMediaPlaylist(playlistObject);
            });
        }

        [Theory]
        public async Task TestParserCreatesMasterPlaylistAsync(string newLine)
        {
            var playlist = CreateValidMasterPlaylist(newLine);
            await TempFileCreator.RunInSafeContextAsync(playlist, _extension, async uri =>
            {
                var playlistObject = await _creator.CreateFromAsync(uri).ConfigureAwait(false);
                AssertMasterPlaylist(playlistObject);
            });
        }

        [Theory]
        public async Task TestParserCreatesMediaPlaylistAsync(string newLine)
        {
            var playlist = CreateValidMediaPlaylist(newLine);
            await TempFileCreator.RunInSafeContextAsync(playlist, _extension, async uri =>
            {
                var playlistObject = await _creator.CreateFromAsync(uri).ConfigureAwait(false);
                AssertMediaPlaylist(playlistObject);
            });
        }
    }
}
