namespace SeeSharpLiveStreaming.Playlist
{
    /// <summary>
    /// Enumerates the valid playlist types for HLS.
    /// </summary>
    public enum PlaylistType
    {
        /// <summary>
        /// The .m3u8
        /// </summary>
        M3U8 = 0,

        /// <summary>
        /// The m3u for compatibility.
        /// </summary>
        M3U = 1
    }

    /// <summary>
    /// Enumerates the valid contents type for HLS.
    /// </summary>
    public enum ContentType
    {
        /// <summary>
        /// application/vnd.apple.mpegurl
        /// </summary>
        AppleMpegUrl = 0,

        /// <summary>
        /// audio/mpegurl
        /// </summary>
        MpegUrl = 1
    }

    /// <summary>
    /// Enumerates the encryptions methods.
    /// </summary>
    /// <remarks>
    /// Read from the EXT-X-KEY tag, METHOD parameter.
    /// </remarks>
    public enum EncryptionMethod
    {
        /// <summary>
        /// No encryption is used.
        /// </summary>
        /// <value>
        /// NONE
        /// </value>
        None = 0,

        /// <value>
        /// AES-128
        /// </value>
        Aes128 = 1,

        /// <value>
        /// SAMPLE-AES
        /// </value>
        SampleAes = 2
    }

    /// <summary>
    /// Defines enumerated strings of media types used in master playlist tag EXT-X-MEDIA.
    /// </summary>
    public static class MediaTypes
    {
        public const string Audio = "AUDIO";
        public const string Video = "VIDEO";
        public const string Subtitles = "SUBTITLES";
        public const string ClosedCaptions = "CLOSED-CAPTIONS";
    }
}
