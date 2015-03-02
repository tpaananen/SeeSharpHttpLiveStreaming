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
        /// <summary>
        /// The audio media type.
        /// </summary>
        public const string Audio = "AUDIO";
        /// <summary>
        /// The video media type.
        /// </summary>
        public const string Video = "VIDEO";
        /// <summary>
        /// The subtitles media type.
        /// </summary>
        public const string Subtitles = "SUBTITLES";
        /// <summary>
        /// The closed captions media type.
        /// </summary>
        public const string ClosedCaptions = "CLOSED-CAPTIONS";
    }

    /// <summary>
    /// Enumerates the tag types.
    /// </summary>
    public enum TagType
    {
        #pragma warning disable 1591
        ExtM3U = 0,
        ExtXVersion = 1,
        ExtInf = 2,
        ExtXByteRange = 3,
        ExtXDiscontinuity = 4,
        ExtXKey = 5,
        ExtXMap = 6,
        ExtXProgramDateTime = 7,
        ExtXTargetDuration = 8,
        ExtXMediaSequence = 9,
        ExtXDiscontinuitySequence = 10,
        ExtXEndList = 11,
        ExtXPlaylistType = 12,
        ExtXIFramesOnly = 13,
        ExtXMedia = 14,
        ExtXStreamInf = 15,
        ExtXIFrameStreamInf = 16,
        ExtXSessionData = 17,
        ExtXIndependentSegments = 18,
        ExtXStart = 19
    }
}
