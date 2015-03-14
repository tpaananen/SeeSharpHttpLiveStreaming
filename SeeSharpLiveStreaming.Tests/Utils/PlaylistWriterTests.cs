using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Tests.Utils
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestFixture]
    [TestFixture]
    public class PlaylistWriterTests
    {
        private IPlaylistWriter _playlistWriter;
        private StringBuilder _stringBuilder;

        [SetUp]
        public void SetUp()
        {
            _stringBuilder = new StringBuilder();
            _playlistWriter = new PlaylistWriter(new StringWriter(_stringBuilder));
        }

        [TearDown]
        public void TearDown()
        {
            if (_playlistWriter != null)
            {
                _playlistWriter.Dispose();
            }
        }

        [Test]
        public void TestPlaylistWriterThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PlaylistWriter(null));
        }

        [Test]
        public void TestPlaylistWriterThrowsArgumentNullExceptionForNullLine()
        {
            Assert.Throws<ArgumentNullException>(() => _playlistWriter.WriteLine(null));
        }

        [Test]
        public void TestPlaylistWriterWritesThelines()
        {
            const string line = "Hey there.";
            _playlistWriter.WriteLine(line);
            Assert.AreEqual(_stringBuilder.ToString(), line + Environment.NewLine);
        }

        [Test]
        public void TestPlaylistWriterIsDisposedProperly()
        {
            _playlistWriter.Dispose();
            Assert.Throws<ObjectDisposedException>(() => _playlistWriter.WriteLine("You should be disposed of!"));

            var playlistWriter = new PlaylistWriter(new StringWriter(_stringBuilder));
            playlistWriter.Dispose(false);
            playlistWriter.Dispose(true);
            Assert.Throws<ObjectDisposedException>(() => playlistWriter.WriteLine("You should be disposed of!"));
        }
    }
}
