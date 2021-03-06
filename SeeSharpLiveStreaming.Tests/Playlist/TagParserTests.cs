﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    [TestFixture]
    public class TagParserTests
    {
        private static readonly Uri Uri = new Uri("http://example.com/playlist/load.m3u8");

        [Datapoints]
        internal static readonly string[] LineFeeds = {"\r\n", "\n"};

        private static string GetPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000" + lineFeed + "http://example.com/playlist/low.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000" + lineFeed + lineFeed + "http://example.com/playlist/mid.m3u8" + lineFeed + lineFeed + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000" + lineFeed + "http://example.com/playlist/hi.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\"" + lineFeed + "http://example.com/playlist/audio-only.m3u8" + lineFeed;
        }

        private static string GetPlaylistWithRelativeUris(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000" + lineFeed + "low.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000" + lineFeed + lineFeed + "mid.m3u8" + lineFeed + lineFeed + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000" + lineFeed + "hi.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\"" + lineFeed + "audio-only.m3u8" + lineFeed;
        }

        [Theory]
        public void TestReadFirstLine(string lineFeed)
        {
            var playlist = GetPlaylist(lineFeed);
            using (var reader = new StringReader(playlist))
            {
                TagParser.ReadFirstLine(reader);
            }
        }

        [Test]
        public void TestReadFirstLineThrowsException()
        {
            const string playlist = "ahskjshdjsgfhjgsjhgsgjshgdshjd\njshdjshdjshdjshdjshdjshdjsd";
            using (var reader = new StringReader(playlist))
            {
                Assert.Throws<SerializationException>(() => TagParser.ReadFirstLine(reader));
            }
        }

        [Test]
        public void TestParseTagReturnsEmptyString()
        {
            var tag = TagParser.ParseTag(null);
            Assert.AreEqual(string.Empty, tag);
            tag = TagParser.ParseTag(string.Empty);
            Assert.AreEqual(string.Empty, tag);
        }

        [Test]
        public void TestParseTagReturnsEmptyStringIfNoTagEndMarkerIsFoundForNotSupportedTag()
        {
            var tag = TagParser.ParseTag("#EXT-X-STREAM-INF-INV");
            Assert.AreEqual(string.Empty, tag);
        }

        [Test]
        public void TestParseTagReturnsTagIfNoTagEndMarkerIsFoundForTagWithAttributes()
        {
            var tag = TagParser.ParseTag("#EXT-X-STREAM-INF");
            Assert.AreEqual("#EXT-X-STREAM-INF", tag);
        }

        [Theory]
        public void TestParseTagReturnsEmptyStringIfUnknownTagIsFound(string lineFeed)
        {
            var line = "#EXT-X-STREAM-INT:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000" + lineFeed + "http://example.com/playlist/low.m3u8" + lineFeed;
            var tag = TagParser.ParseTag(line);
            Assert.AreEqual(string.Empty, tag);
        }

        [Theory]
        public void TestParseTagReturnsTagCorrectly(string lineFeed)
        {
            var line = "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000" + lineFeed + "http://example.com/playlist/low.m3u8" + lineFeed;
            var tag = TagParser.ParseTag(line);
            Assert.AreEqual("#EXT-X-STREAM-INF", tag);
        }

        [Theory]
        public void TestTagParserCreatesPlaylistCollection(string lineFeed)
        {
            var playlist = GetPlaylist(lineFeed);
            var list = TagParser.ReadLines(playlist, Uri);

            Assert.IsNotNull(list);
            Assert.AreEqual(4, list.Count);
            
            Assert.AreEqual("#EXT-X-STREAM-INF", list.ElementAt(0).Tag);
            Assert.AreEqual("#EXT-X-STREAM-INF", list.ElementAt(1).Tag);
            Assert.AreEqual("#EXT-X-STREAM-INF", list.ElementAt(2).Tag);
            Assert.AreEqual("#EXT-X-STREAM-INF", list.ElementAt(3).Tag);

            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000", list.ElementAt(0).Line);
            Assert.AreEqual(new Uri("http://example.com/playlist/low.m3u8"), list.ElementAt(0).Uri);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000", list.ElementAt(1).Line);
            Assert.AreEqual(new Uri("http://example.com/playlist/mid.m3u8"), list.ElementAt(1).Uri);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000", list.ElementAt(2).Line);
            Assert.AreEqual(new Uri("http://example.com/playlist/hi.m3u8"), list.ElementAt(2).Uri);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\"", list.ElementAt(3).Line);
            Assert.AreEqual(new Uri("http://example.com/playlist/audio-only.m3u8"), list.ElementAt(3).Uri);
        }

        [Theory]
        public void TestTagParserCreatesPlaylistCollectionFromPlaylistWithRelativeUris(string lineFeed)
        {
            var playlist = GetPlaylistWithRelativeUris(lineFeed);
            var list = TagParser.ReadLines(playlist, Uri);

            Assert.IsNotNull(list);
            Assert.AreEqual(4, list.Count);
            
            Assert.AreEqual("#EXT-X-STREAM-INF", list.ElementAt(0).Tag);
            Assert.AreEqual("#EXT-X-STREAM-INF", list.ElementAt(1).Tag);
            Assert.AreEqual("#EXT-X-STREAM-INF", list.ElementAt(2).Tag);
            Assert.AreEqual("#EXT-X-STREAM-INF", list.ElementAt(3).Tag);

            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000", list.ElementAt(0).Line);
            Assert.AreEqual(new Uri("http://example.com/playlist/low.m3u8"), list.ElementAt(0).Uri);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000", list.ElementAt(1).Line);
            Assert.AreEqual(new Uri("http://example.com/playlist/mid.m3u8"), list.ElementAt(1).Uri);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000", list.ElementAt(2).Line);
            Assert.AreEqual(new Uri("http://example.com/playlist/hi.m3u8"), list.ElementAt(2).Uri);
            Assert.AreEqual("#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\"", list.ElementAt(3).Line);
            Assert.AreEqual(new Uri("http://example.com/playlist/audio-only.m3u8"), list.ElementAt(3).Uri);
        }

        [Test]
        public void TestTagParserParsesDiscontinuityTag()
        {
            const string line = "#EXT-X-DISCONTINUITY";
            Assert.AreEqual(line, TagParser.ParseTag(line));
        }

        [Test]
        public void TestTagParserReturnEmptyStringIfNotValidTagWithoutEndMarker()
        {
            string line = "#EXT-X-DISCONTI";
            Assert.AreEqual(string.Empty, TagParser.ParseTag(line));

            line = "EXT-X-DISCONTI";
            Assert.AreEqual(string.Empty, TagParser.ParseTag(line));
        }

        [Test]
        public void TestTagParserReturnsTagIfTagWithoutAttributesIsFound()
        {
            foreach (var tag in Tag.HasNoAttributes)
            {
                var parsed = TagParser.ParseTag(tag);
                Assert.AreEqual(parsed, tag);
            }
        }
    }
}
