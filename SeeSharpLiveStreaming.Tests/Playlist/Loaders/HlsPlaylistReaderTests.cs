using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Loaders;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Tests.Helpers;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist.Loaders
{
    [TestFixture]
    public class HlsPlaylistReaderTests
    {
        private readonly Uri _uri = new Uri("http://example.com/");
        private Mock<IPlaylistLoaderFactory> _mockFactory;
        private Mock<IPlaylistLoader> _mockLoader;
        private HlsPlaylistReader _reader;

        [SetUp]
        public void SetUp()
        {
            _mockFactory = new Mock<IPlaylistLoaderFactory>();
            _mockLoader = new Mock<IPlaylistLoader>();
            _mockFactory.Setup(d => d.Create()).Returns(_mockLoader.Object).Verifiable();
            _reader = new HlsPlaylistReader(_mockFactory.Object);
        }

        [Test]
        public void TestHlsPlaylistReaderThrowsArgumentNullExceptionFromCtor()
        {
            Assert.Throws<ArgumentNullException>(() => new HlsPlaylistReader(null));
        }

        [Test]
        public void TestHlsPlaylistReaderReads()
        {
            _mockLoader.Setup(d => d.Load(_uri)).Returns(Tag.StartLine).Verifiable();
            var content = _reader.Read(_uri);
            Assert.AreEqual(Tag.StartLine, content);
            _mockLoader.VerifyAll();
            _mockFactory.VerifyAll();
        }

        [Test]
        public async Task TestHlsPlaylistReaderReadsAsync()
        {
            _mockLoader.Setup(d => d.LoadAsync(_uri)).Returns(TaskHelper.CreateFrom(Tag.StartLine)).Verifiable();
            var content = await _reader.ReadAsync(_uri);
            Assert.AreEqual(Tag.StartLine, content);
            _mockLoader.VerifyAll();
            _mockFactory.VerifyAll();
        }

        [Test]
        public async Task TestHlsPlaylistReaderReadsAsync2()
        {
            _mockLoader.Setup(d => d.LoadAsync(_uri)).Returns(Task.FromResult(Tag.StartLine)).Verifiable();
            var content = await _reader.ReadAsync(_uri);
            Assert.AreEqual(Tag.StartLine, content);
            _mockLoader.VerifyAll();
            _mockFactory.VerifyAll();
        }

    }
}
