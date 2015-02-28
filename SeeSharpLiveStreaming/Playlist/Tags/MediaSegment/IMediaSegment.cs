using System;

namespace SeeSharpLiveStreaming.Playlist
{
    public interface IMediaSegment
    {

        /// <summary>
        /// Gets the URI of the segment.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Gets the optional encryption elements.
        /// </summary>
        IEncryption Encryption { get; }

    }
}
