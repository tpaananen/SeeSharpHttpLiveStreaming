using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.ValueParsers;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment
{
    /// <summary>
    /// Represents the EXT-X-KEY tag.
    /// </summary>
    /// <remarks>
    /// Media Segments MAY be encrypted. The EXT-X-KEY tag specifies how to
    /// decrypt them. It applies to every Media Segment that appears between
    /// it and the next EXT-X-KEY tag in the Playlist file with the same
    /// KEYFORMAT attribute (or the end of the Playlist file). Two or more
    /// EXT-X-KEY tags with different KEYFORMAT attributes MAY apply to the
    /// same Media Segment if they ultimately produce the same decryption
    /// key. The format is:
    /// #EXT-X-KEY:&lt;attribute-list&gt;
    /// </remarks>
    internal sealed class Key : BaseTag
    {
        private const int RequiredIvVersion = 2;

        private const int RequiredKeyFormatVersion = 5;

        private const string DefaultKeyFormatValue = "identity";

        private const string KeyFormatVersionSeparator = "/";

        private const int SizeOfKey = 128; // bits

        /// <summary>
        /// Initializes a new instance of the <see cref="Key"/> class.
        /// </summary>
        internal Key()
        {
            UsingDefaultCtor = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Key"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="version">The version.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="initializationVector">The initialization vector.</param>
        /// <param name="keyFormat">The key format.</param>
        /// <param name="keyFormatVersions">The key format versions.</param>
        public Key(EncryptionMethod method, int version = 0, Uri uri = null, string initializationVector = null, 
                   string keyFormat = null, IReadOnlyCollection<int> keyFormatVersions = null)
        {
            if (method != EncryptionMethod.None)
            {
                ValidateCtorParameters(version, uri, initializationVector, keyFormat, keyFormatVersions);
                Uri = uri;
                InitializationVector = CreateHexValue(initializationVector);
                KeyFormat = string.IsNullOrEmpty(keyFormat) ? DefaultKeyFormatValue : keyFormat;
                KeyFormatVersions = keyFormatVersions ?? new ReadOnlyCollection<int>(new int[0]);
            }
            Method = method;
        }

        private static string CreateHexValue(string initializationVector)
        {
            return string.IsNullOrEmpty(initializationVector) 
                    ? string.Empty 
                    : ValueParser.CreateHexValue(initializationVector, SizeOfKey);
        }

        // ReSharper disable once UnusedParameter.Local
        private void ValidateCtorParameters(int version, Uri uri, string initializationVector, string keyFormat,
                                            IReadOnlyCollection<int> keyFormatVersions)
        {
            uri.RequireNotNull("uri");
            if (!string.IsNullOrEmpty(initializationVector) && version < RequiredIvVersion)
            {
                throw new IncompatibleVersionException(TagName, "IV", version, RequiredIvVersion);
            }
            if (version < RequiredKeyFormatVersion)
            {
                if (!string.IsNullOrEmpty(keyFormat))
                {
                    throw new IncompatibleVersionException(TagName, "KEYFORMAT", version, RequiredKeyFormatVersion);
                }
                if (keyFormatVersions != null && keyFormatVersions.Count > 0)
                {
                    throw new IncompatibleVersionException(TagName, "KEYFORMATVERSIONS", version, RequiredKeyFormatVersion);
                }
            }
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public override string TagName
        {
            get { return "#EXT-X-KEY"; }
        }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public override TagType TagType
        {
            get { return TagType.ExtXKey; }
        }

        /// <summary>
        /// The value is an enumerated-string that specifies the encryption
        /// method. This attribute is REQUIRED.
        ///
        /// The methods defined are: NONE, AES-128, and SAMPLE-AES.
        ///
        /// An encryption method of NONE means that Media Segments are not
        /// encrypted. If the encryption method is NONE, other attributes MUST
        /// NOT be present.
        /// 
        /// An encryption method of AES-128 signals that Media Segments are
        /// completely encrypted using the Advanced Encryption Standard [AES_128]
        /// with a 128-bit key, Cipher Block Chaining, and PKCS7 padding
        /// [RFC5652]. CBC is restarted on each segment boundary, using either
        /// the IV attribute value or the Media Sequence Number as the IV; see
        /// Section 5.2.  The URI attribute is REQUIRED for this METHOD.
        /// 
        /// An encryption method of SAMPLE-AES means that the Media Segments
        /// contain media samples, such as audio or video, that are encrypted
        /// using the Advanced Encryption Standard [AES_128]. How these media
        /// streams are encrypted and encapsulated in a segment depends on the
        /// media encoding and the media format of the segment. The encryption
        /// format for H.264 [H_264], AAC [ISO_14496] and AC-3 [AC_3] media
        /// streams is described by [SampleEnc]. The IV attribute MAY be
        /// present; see Section 5.2.
        /// </summary>
        public EncryptionMethod Method { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing a URI that specifies how to
        /// obtain the key. This attribute is REQUIRED unless the METHOD is NONE.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets or sets the initialization vector (IV) attribute value.
        /// 
        /// The value is a hexadecimal-sequence that specifies a 128-bit unsigned
        /// integer Initialization Vector to be used with the key. Use of the IV
        /// attribute REQUIRES a compatibility version number of 2 or greater.
        /// See Section 5.2 for when the IV attribute is used.
        /// </summary>
        public string InitializationVector { get; private set; }

        /// <summary>
        /// The value is a quoted-string that specifies how the key is
        /// represented in the resource identified by the <see cref="Key.Uri"/>; see Section 5 for
        /// more detail. This attribute is OPTIONAL; its absence indicates an
        /// implicit value of "identity". Use of the KEYFORMAT attribute
        /// REQUIRES a compatibility version number of 5 or greater.
        /// </summary>
        public string KeyFormat { get; private set; }

        /// <summary>
        /// The value is a quoted-string containing one or more positive integers
        /// separated by the "/" character (for example, "1/3").  If more than
        /// one version of a particular KEYFORMAT is defined, this attribute can
        /// be used to indicate which version(s) this instance complies with.
        /// This attribute is OPTIONAL; if it is not present, its value is
        /// considered to be "1".  Use of the KEYFORMATVERSIONS attribute
        /// REQUIRES a compatibility version number of 5 or greater.
        ///
        /// If the Media Playlist file does not contain an EXT-X-KEY tag then
        /// Media Segments are not encrypted.
        ///
        /// See Section 5 for the format of the key file, and Section 5.2,
        /// Section 6.2.3 and Section 6.3.6 for additional information on Media
        /// Segment encryption.
        /// </summary>
        public IReadOnlyCollection<int> KeyFormatVersions { get; private set; }

        /// <summary>
        /// Deserializes the tag from the <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public override void Deserialize(string content, int version)
        {
            content.RequireNotEmpty("content");
            try
            {
                ParseMethod(content);
                if (Method == EncryptionMethod.None)
                {
                    return;
                }
                ParseUri(content);
                ParseInitializationVector(content, version);
                ParseKeyFormat(content, version);
                ParseKeyFormatVersions(content, version);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to parse EXT-X-KEY tag.", ex);
            }
        }

        /// <summary>
        /// Serializes the attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void SerializeAttributes(IPlaylistWriter writer)
        {
            bool hasPreviousAttributes = false;
            WriteEnumeratedString(writer, "METHOD", Method.FromEncryptionMethod(), ref hasPreviousAttributes);
            if (Method == EncryptionMethod.None)
            {
                return;
            }

            WriteUri(writer, "URI", Uri, ref hasPreviousAttributes);
            WriteEnumeratedString(writer, "IV", InitializationVector, ref hasPreviousAttributes);
            WriteQuotedString(writer, "KEYFORMAT", KeyFormat, ref hasPreviousAttributes);
            WriteQuotedString(writer, "KEYFORMATVERSIONS", string.Join(KeyFormatVersionSeparator, KeyFormatVersions), ref hasPreviousAttributes);
        }

        private void ParseUri(string content)
        {
            const string name = "URI";
            Uri = ParseUri(name, content, true);
        }

        private void ParseMethod(string content)
        {
            const string name = "METHOD";
            var value = ValueParser.ParseEnumeratedString(name, content, true);
            Method = value.ToEncryptionMethod();
            RequireNoAttributesIfNoEncryption(content);
        }

        private void ParseInitializationVector(string content, int version)
        {
            const string name = "IV";
            var value = ValueParser.ParseHexadecimal(name, content, false, SizeOfKey);
            if (value != string.Empty && version < RequiredIvVersion)
            {
                throw new IncompatibleVersionException(TagName, name, version, RequiredIvVersion);
            }
            InitializationVector = value;
        }

        private void ParseKeyFormat(string content, int version)
        {
            const string name = "KEYFORMAT";
            var value = ValueParser.ParseQuotedString(name, content, false);
            if (value != string.Empty && version < RequiredKeyFormatVersion)
            {
                throw new IncompatibleVersionException(TagName, name, version, RequiredKeyFormatVersion);
            }
            KeyFormat = value == string.Empty ? DefaultKeyFormatValue : value;
        }

        private void ParseKeyFormatVersions(string content, int version)
        {
            const string name = "KEYFORMATVERSIONS";
            var values = ValueParser.ParseSeparatedQuotedString(name, content, false, int.Parse, KeyFormatVersionSeparator[0]);
            if (values.Count != 0)
            {
                if (version < RequiredKeyFormatVersion)
                {
                    throw new IncompatibleVersionException(TagName, name, version, RequiredKeyFormatVersion);
                }
            }
            KeyFormatVersions = new ReadOnlyCollection<int>(values);
        }

        // ReSharper disable once UnusedParameter.Local
        private void RequireNoAttributesIfNoEncryption(string content)
        {
            const string validNoneAttributeList = "METHOD=NONE";
            if (Method == EncryptionMethod.None && content != validNoneAttributeList)
            {
                throw new SerializationException("The EXT-X-KEY tag must not have any other attributes than METHOD if the METHOD is NONE.");
            }
        }
    }
}
