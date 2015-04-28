using System;
using System.IO;
using System.Threading;
using Moq;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Loaders;
using SeeSharpHttpLiveStreaming.Session;

namespace SeeSharpHttpLiveStreaming.Tests.Session
{
    [TestFixture]
    public class MediaSessionTests
    {

        private Mock<IMediaLoader> _mediaLoaderMock;
        private MediaSession _session;

        [SetUp]
        public void SetUp()
        {
            _mediaLoaderMock = new Mock<IMediaLoader>();
            _session = new MediaSession(_mediaLoaderMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _session.Dispose();
        }

        [Test]
        public void TestMediaSessionCtorThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _session = new MediaSession(null));
        }

        [Test]
        public void TestMediaSessionEnqueuesSegment()
        {
            var bytes = new byte[128];
            _mediaLoaderMock.Setup(d => d.LoadAsync(It.IsAny<MediaSegment>())).ReturnsAsync(bytes).Verifiable();
            var segment1 = new MediaSegment(0, null);
            _session.Enqueue(segment1);
            Assert.LessOrEqual(_session.Uris.Count, 1);
            var segment2 = new MediaSegment(1, null);
            _session.Enqueue(segment2);
            Assert.LessOrEqual(_session.Uris.Count, 2);

            Thread.Sleep(10);

            _mediaLoaderMock.Verify(d => d.LoadAsync(segment1), Times.Once);
            _mediaLoaderMock.Verify(d => d.LoadAsync(segment2), Times.Once);
        }

        [Test]
        public void TestMediaSessionProvidesLoadedSegmentsAfterLoadingThem()
        {
            var bytes = new byte[128];
            _mediaLoaderMock.Setup(d => d.LoadAsync(It.IsAny<MediaSegment>())).ReturnsAsync(bytes);
            _session.Enqueue(new MediaSegment(0, null));
            
            Thread.Sleep(10);

            byte[] array;
            Assert.That(_session.TryDequeue(out array));
            Assert.AreEqual(bytes, array);
        }

        [Test]
        public void TestMediaSessionRetriesSegment()
        {
            _mediaLoaderMock.Setup(d => d.LoadAsync(It.IsAny<MediaSegment>()))
                            .ThrowsAsync(new IOException()).Verifiable();
            _session.Enqueue(new MediaSegment(0, null));

            Thread.Sleep(20);

            _mediaLoaderMock.VerifyAll();

            byte[] array;
            Assert.IsFalse(_session.TryDequeue(out array));
        }

        [Test]
        public void TestMediaSessionDoesNotRetrieSegmentAfterDispose()
        {
            _mediaLoaderMock.Setup(d => d.LoadAsync(It.IsAny<MediaSegment>()))
                            .ThrowsAsync(new ObjectDisposedException("disposed")).Verifiable();
            _session.Enqueue(new MediaSegment(0, null));

            Thread.Sleep(20);

            _mediaLoaderMock.VerifyAll();

            byte[] array;
            Assert.IsFalse(_session.TryDequeue(out array));
        }

        [Test]
        public void TestMediaSessionThrowsInvalidOperationExceptionIfFinalSegmentIsAlreadyProvided()
        {
            _mediaLoaderMock.Setup(d => d.LoadAsync(It.IsAny<MediaSegment>()))
                            .ThrowsAsync(new IOException()).Verifiable();
            _session.Enqueue(new MediaSegment(0, null));
            _session.Enqueue(new MediaSegment(1, null) { IsFinal = true });
            Assert.Throws<InvalidOperationException>(() => _session.Enqueue(new MediaSegment(2, null)));
        }
    }
}
