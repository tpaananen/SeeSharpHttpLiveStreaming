using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Tests.Helpers;
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
            _playlistWriter = TestPlaylistWriterFactory.CreateWithStringBuilder(out _stringBuilder);
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
        public void TestPlaylistWriterThrowsArgumentNullExceptionForNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => _playlistWriter.Write(null));
        }

        [Test]
        public void TestPlaylistWriterWritesThelines()
        {
            const string line = "Hey there.";
            _playlistWriter.Write(line);
            _playlistWriter.WriteLineEnd();
            Assert.AreEqual(_stringBuilder.ToString(), line + Environment.NewLine);
        }

        [Test]
        public void TestPlaylistWriterIsDisposedProperly()
        {
            _playlistWriter.Dispose();
            Assert.Throws<ObjectDisposedException>(() => _playlistWriter.Write("You should be disposed of!"));

            var playlistWriter = (PlaylistWriter)TestPlaylistWriterFactory.Create();
            playlistWriter.Close(false);
            playlistWriter.Close(true);
            Assert.Throws<ObjectDisposedException>(playlistWriter.WriteLineEnd);
        }

        [Test]
        public void TestPlaylistWriterThrowsArgumentExceptionIfInternalWriterUsesOtherThanUtf8Encoding()
        {
            Assert.Throws<ArgumentException>(() => new PlaylistWriter(new StreamWriter(new MemoryStream(), new UnicodeEncoding())));
        }

        [Test]
        public void TestPlaylistWriterThrowsArgumentExceptionIfInternalWriterUsesUtf8EncodingWithBom()
        {
            Assert.Throws<ArgumentException>(() => new PlaylistWriter(new StreamWriter(new MemoryStream(), new UTF8Encoding(true))));
        }
    }
}
