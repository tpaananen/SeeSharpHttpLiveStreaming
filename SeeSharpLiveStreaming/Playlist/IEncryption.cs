﻿using System;

namespace SeeSharpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents the encryption interface.
    /// </summary>
    public interface IEncryption
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
