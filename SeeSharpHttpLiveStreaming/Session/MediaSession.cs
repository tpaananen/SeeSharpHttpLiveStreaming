using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SeeSharpHttpLiveStreaming.Playlist;
using SeeSharpHttpLiveStreaming.Playlist.Loaders;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Session
{
    internal class MediaSession : AbstractSessionBase
    {
        internal const int MaxRetries = 3;

        private int _isLoading;
        private volatile bool _hasFinalSegment;
        private readonly IMediaLoader _mediaLoader;

        /// <summary>
        /// Contains the list of segments to be downloaded.
        /// </summary>
        internal readonly ConcurrentQueue<MediaSegment> Uris = new ConcurrentQueue<MediaSegment>();

        /// <summary>
        /// Contains the list of downloaded segments.
        /// </summary>
        internal readonly ConcurrentQueue<MediaSegment> MediaSegments = new ConcurrentQueue<MediaSegment>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSession"/> class.
        /// </summary>
        /// <param name="mediaLoader">The media loader.</param>
        public MediaSession(IMediaLoader mediaLoader)
        {
            mediaLoader.RequireNotNull("mediaLoader");
            _mediaLoader = mediaLoader;
        }

        /// <summary>
        /// Enqueues the specified segment.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a new segment is tried to queue and the final segment has already been provided.
        /// </exception>
        internal void Enqueue(MediaSegment segment)
        {
            if (_hasFinalSegment)
            {
                throw new InvalidOperationException("The session has got its last segment already.");
            }
            Uris.Enqueue(segment);
            _hasFinalSegment = segment.IsFinal;
            Task.Run(async () => await BeginLoadData());
        }

        /// <summary>
        /// Dequeues an item from the queue of downloaded segments.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <returns>
        /// <b>True</b> if the segment is found; otherwise, <b>false</b>.
        /// </returns>
        // TODO: change to publisher
        internal bool TryDequeue(out byte[] segment)
        {
            MediaSegment mediaSegment;
            if (MediaSegments.TryDequeue(out mediaSegment))
            {
                segment = mediaSegment.Data;
                return true;
            }
            segment = null;
            return false;
        }

        private async Task BeginLoadData()
        {
            if (IsLoading())
            {
                return;
            }
            int retryCounter = 0;
            MediaSegment segment = null;
            while (retryCounter != 0 || Uris.TryDequeue(out segment))
            {
                try
                {
                    // ReSharper disable PossibleNullReferenceException
                    byte[] array = await _mediaLoader.LoadAsync(segment);
                    segment.SetData(array);
                    // ReSharper enable PossibleNullReferenceException
                    MediaSegments.Enqueue(segment);
                    retryCounter = 0;
                }
                catch (ObjectDisposedException)
                {
                    Trace.WriteLine("The session has been disposed of.");
                    break;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    ++retryCounter;
                    if (retryCounter >= MaxRetries)
                    {
                        Trace.WriteLine("Max retries attempted with segment " + segment.SequenceNumber);
                        break;
                    }

                }
            }
            SetNotLoading();
        }

        private bool IsLoading()
        {
            return Interlocked.CompareExchange(ref _isLoading, 1, 0) == 1;
        }

        private void SetNotLoading()
        {
            Interlocked.CompareExchange(ref _isLoading, 0, 1);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mediaLoader.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
