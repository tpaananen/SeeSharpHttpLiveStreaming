using System;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents the encryption interface.
    /// </summary>
    internal interface IEncryption
    {
        /// <summary>
        /// Gets the encryption method.
        /// </summary>
        EncryptionMethod Method { get; }
        
        /// <summary>
        /// Gets the URI where the key is located.
        /// </summary>
        Uri Uri { get; }
    }
}
