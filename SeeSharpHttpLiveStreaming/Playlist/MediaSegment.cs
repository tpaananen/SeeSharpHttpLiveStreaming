using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SeeSharpHttpLiveStreaming.Playlist.Tags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Represents a media segment.
    /// </summary>
    /// <remarks>
    /// Each Media Segment is specified by a series of Media Segment tags
    /// followed by a URI. Some Media Segment tags apply to just the next
    /// segment; others apply to all subsequent segments until another
    /// instance of the same tag.
    /// 
    /// A Media Segment tag MUST NOT appear in a Master Playlist. Clients
    /// SHOULD fail to parse Playlists that contain both Media Segment Tags
    /// and Master Playlist tags (Section 4.3.4).
    /// </remarks>
    internal class MediaSegment
    {
        private readonly IDictionary<string, Key> _keys = new Dictionary<string, Key>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSegment" /> class.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="map">
        /// The map. Can be <b>null</b>. This may be updated by the read method.
        /// </param>
        public MediaSegment(long sequenceNumber, Map map)
        {
            SequenceNumber = sequenceNumber;
            Map = map;
        }

        /// <summary>
        /// Gets the sequence number.
        /// </summary>
        public long SequenceNumber { get; private set; }

        /// <summary>
        /// Gets the duration of the segment.
        /// </summary>
        public decimal Duration { get; private set; }

        /// <summary>
        /// Gets the information.
        /// </summary>
        public string Information { get; private set; }

        /// <summary>
        /// Gets the byte range.
        /// </summary>
        public ByteRange ByteRange { get; private set; }

        /// <summary>
        /// Gets the URI.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets the program date time.
        /// </summary>
        public DateTimeOffset ProgramDateTime { get; private set; }

        /// <summary>
        /// Gets the keys. There may be multiple keys with different key formats.
        /// </summary>
        public IReadOnlyDictionary<string, Key> Keys
        {
            get { return new ReadOnlyDictionary<string, Key>(_keys); }
        }

        /// <summary>
        /// Gets the map.
        /// </summary>
        public Map Map { get; private set; }

        /// <summary>
        /// Reads the tag and either accepts or rejects it.
        /// If the segment rejects the tag, parser should create a new segment
        /// which will accept the tag.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="version">The version.</param>
        /// <returns>
        /// When this method returns <b>false</b> it indicates that the
        /// segment is ready and a new segment should be created for
        /// next lines.
        /// </returns>
        public bool ReadTag(PlaylistLine line, Uri baseUri, int version)
        {
            ProcessTag(TagFactory.Create(line, baseUri, version));
            if (line.Uri != null)
            {
                // Accept tags as long there is a URI on the line
                Uri = line.Uri;
                return false;
            }
            return true;
        }

        private void ProcessTag(BaseTag tag)
        {
            if (tag.TagType == TagType.ExtXByteRange)
            {
                ByteRange = (ByteRange) tag;
            }
            else if (tag.TagType == TagType.ExtInf)
            {
                HandleExtInf((ExtInf)tag);
            }
            else if (tag.TagType == TagType.ExtXProgramDateTime)
            {
                ProgramDateTime = ((ProgramDateTime)tag).DateTime;
            }
            else if (tag.TagType == TagType.ExtXKey)
            {
                HandleKey((Key)tag);
            }
            else if (tag.TagType == TagType.ExtXMap)
            {
                Map = tag as Map;
            }
        }

        private void HandleExtInf(ExtInf inf)
        {
            Information = inf.Information;
            Duration = inf.Duration;
        }

        private void HandleKey(Key key)
        {
            if (!string.IsNullOrEmpty(key.KeyFormat))
            {
                key.SetSequenceNumber(SequenceNumber);
                _keys[key.KeyFormat] = key;
            }
        }
    }
}
