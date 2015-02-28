using System;
using System.Collections.Generic;

namespace SeeSharpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents a media segment.
    /// </summary>
    public class MediaSegment : IMediaSegment
    {
        /// <summary>
        /// Gets the URI of the segment.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets the optional encryption elements.
        /// </summary>
        public IEncryption Encryption { get; private set; }
    }
}
