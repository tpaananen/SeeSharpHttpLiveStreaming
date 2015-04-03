using System;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist
{

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

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> is valid.
        /// </summary>
        /// <param name="value">The type.</param>
        /// <returns>
        /// <b>True</b> if the <paramref name="value"/> is one of the constants in this class.
        /// </returns>
        public static bool IsValid(string value)
        {
            return
                value == Audio ||
                value == Video ||
                value == Subtitles ||
                value == ClosedCaptions;
        }
    }

    /// <summary>
    /// Defines enumerated strings that represent boolean values.
    /// </summary>
    public static class YesNo
    {
        /// <summary>
        /// The YES option, translates to <b>true</b> in <see cref="bool"/>.
        /// </summary>
        public const string Yes = "YES";

        /// <summary>
        /// The NO option, translates to <b>false</b> in <see cref="bool"/>.
        /// </summary>
        public const string No = "NO";

        /// <summary>
        /// Determines whether the specified value is valid <see cref="YesNo"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        public static bool IsValid(string value)
        {
            return value == No || value == Yes;
        }

        /// <summary>
        /// Returns either <see cref="Yes" /> or <see cref="No" /> depending on the <paramref name="value" />.
        /// </summary>
        /// <param name="value">if set to <c>true</c> value means Yes.</param>
        /// <param name="required">if set to <c>true</c> the attribute is required and No value is added.</param>
        /// <returns>
        /// If the <paramref name="value" /> is <b>true</b>, returns <see cref="Yes" />; otherwise, <see cref="No" /> 
        /// except if <paramref name="required"/> is <b>false</b>, then return empty string.
        /// </returns>
        public static string FromBoolean(bool value, bool required = false)
        {
            return value ? Yes : required ? No : string.Empty;
        }

        /// <summary>
        /// Converts the <paramref name="value"/> to the <see cref="Boolean"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <b>True</b> if the <paramref name="value"/> is <see cref="Yes"/>; otherwise, returns <b>false</b>.
        /// </returns>
        public static bool FromString(string value)
        {
            return value == Yes;
        }
    }

    /// <summary>
    /// Encryption method extension methods.
    /// </summary>
    public static class EncryptionMethodExtensions
    {
        /// <summary>
        /// Converts the <paramref name="method"/> to the <see cref="EncryptionMethod"/> enumeration value.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The <see cref="EncryptionMethod"/> value.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="method"/> is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="method"/> is <b>null</b>.</exception>
        public static EncryptionMethod ToEncryptionMethod(this string method)
        {
            method.RequireNotNull("method");
            switch (method)
            {
                case "NONE":
                    return EncryptionMethod.None;
                case "AES-128":
                    return EncryptionMethod.Aes128;
                case "SAMPLE-AES":
                    return EncryptionMethod.SampleAes;
                default:
                    throw new ArgumentException("Invalid encryption method value " + method + ".");
            }
        }

        /// <summary>
        /// Converts the <paramref name="method"/> to a string value.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The <see cref="String"/> value.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="method"/> is invalid.</exception>
        public static string FromEncryptionMethod(this EncryptionMethod method)
        {
            switch (method)
            {
                case EncryptionMethod.None:
                    return "NONE";
                case EncryptionMethod.Aes128:
                    return "AES-128";
                case EncryptionMethod.SampleAes:
                    return "SAMPLE-AES";
                default:
                    throw new ArgumentException("Invalid encryption method value " + method + ".");
            }
        }
    }

    /// <summary>
    /// Contanis constants for possible playlist type values.
    /// </summary>
    public static class PlaylistType
    {
        /// <summary>
        /// If the EXT-X-PLAYLIST-TYPE value is EVENT, Media Segments can only be
        /// added to the end of the Media Playlist.
        /// </summary>
        public const string Event = "EVENT";

        /// <summary>
        /// If the EXT-X-PLAYLIST-TYPE
        /// value is VOD, the Media Playlist cannot change.
        /// </summary>
        public const string Vod = "VOD";

        /// <summary>
        /// Determines whether the specified value is valid <see cref="YesNo"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        public static bool IsValid(string value)
        {
            return value == Event || value == Vod;
        }
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
        // ReSharper disable once InconsistentNaming
        ExtXIFramesOnly = 13,
        ExtXMedia = 14,
        ExtXStreamInf = 15,
        // ReSharper disable once InconsistentNaming
        ExtXIFrameStreamInf = 16,
        ExtXSessionData = 17,
        ExtXIndependentSegments = 18,
        ExtXStart = 19
    }
}
