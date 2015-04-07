using NUnit.Framework;
using SeeSharpHttpLiveStreaming.Playlist;

namespace SeeSharpHttpLiveStreaming.Tests.Playlist
{
    /// <summary>
    /// Base class for tests reading and deserializing playlists.
    /// </summary>
    public abstract class PlaylistReadingTestBase
    {
        protected static string CreateValidMediaPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:6" + lineFeed +
                "#EXT-X-PLAYLIST-TYPE:VOD" + lineFeed +
                "#EXT-X-TARGETDURATION:10" + lineFeed +
                "#EXT-X-MEDIA-SEQUENCE:0" + lineFeed +
                "" + lineFeed +
                "#EXT-X-PROGRAM-DATE-TIME:2015-03-21T12:22:22.212+02:00" + lineFeed + 
                "#EXTINF:9.009,Some info" + lineFeed +
                "#EXT-X-BYTERANGE:1024@0" + lineFeed + 
                "http://media.example.com/first.ts" + lineFeed +
                "#EXT-X-DISCONTINUITY" + lineFeed +
                "#EXTINF:9.009,Some other info" + lineFeed +
                "http://media.example.com/second.ts" + lineFeed +
                "#EXTINF:3.003,Some short take" + lineFeed +
                "#EXT-X-BYTERANGE:1024@45" + lineFeed + 
                "http://media.example.com/third.ts" + lineFeed +
                 lineFeed;
        }

        protected static string CreateValidMediaPlaylistWithIFramesOnly(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:6" + lineFeed +
                "#EXT-X-I-FRAMES-ONLY" + lineFeed +
                "#EXT-X-TARGETDURATION:10" + lineFeed +
                "#EXT-X-MEDIA-SEQUENCE:0" + lineFeed +
                "" + lineFeed +
                "#EXTINF:9.009,Some info" + lineFeed +
                "#EXT-X-BYTERANGE:1024" + lineFeed +
                "http://media.example.com/media.ts" + lineFeed +
                "#EXTINF:9.009,Some other info" + lineFeed +
                "#EXT-X-BYTERANGE:1024@1024" + lineFeed +
                "http://media.example.com/media.ts" + lineFeed +
                "#EXTINF:8.121,Some short take" + lineFeed +
                "#EXT-X-BYTERANGE:889@2048" + lineFeed +
                "http://media.example.com/media.ts" + lineFeed +
                "#EXT-X-ENDLIST" + lineFeed + lineFeed + lineFeed;
        }

        protected static string CreateInvalidMediaPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:6" + lineFeed + 
                "#EXT-X-TARGETDURATION:10" + lineFeed +
                "" + lineFeed + 
                "     " + lineFeed + 
                "#EXTINF:9.009,Some info" + lineFeed +
                "" + lineFeed + 
                "     " + lineFeed + 
                "http://media.example.com/first.ts" + lineFeed +
                "#EXTINF:9.009,Some other info" + lineFeed +
                "http://media.example.com/second.ts" + lineFeed +
                "#EXT-X-DISCONTINUITY" + lineFeed + 
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000" + lineFeed + "http://example.com/mid.m3u8" + lineFeed +
                "#EXTINF:3.003,Some short take" + lineFeed +
                "http://media.example.com/third.ts" + lineFeed;
        }

        protected static string CreateInvalidMediaPlaylistMissingUriFromSegment(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:6" + lineFeed +
                "#EXT-X-TARGETDURATION:10" + lineFeed +
                "" + lineFeed +
                "     " + lineFeed +
                "#EXTINF:9.009,Some info" + lineFeed +
                "" + lineFeed +
                "     " + lineFeed +
                "http://media.example.com/first.ts" + lineFeed +
                "#EXTINF:9.009,Some other info" + lineFeed +
                "http://media.example.com/second.ts" + lineFeed +
                "#EXT-X-DISCONTINUITY" + lineFeed +
                "#EXTINF:3.003,last segment without URI" + lineFeed;
        }

        protected static string CreateInvalidMediaPlaylistWithIFramesOnly(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed +
                "#EXT-X-I-FRAMES-ONLY" + lineFeed +
                "#EXT-X-TARGETDURATION:10" + lineFeed +
                "#EXT-X-MEDIA-SEQUENCE:0" + lineFeed +
                "" + lineFeed +
                "#EXTINF:9.009,Some info" + lineFeed +
                "#EXT-X-BYTERANGE:1024" + lineFeed +
                "http://media.example.com/media.ts" + lineFeed +
                "#EXTINF:9.009,Some other info" + lineFeed + // Missing byte range
                "http://media.example.com/media.ts" + lineFeed +
                "#EXTINF:8.121,Some short take" + lineFeed +
                "#EXT-X-BYTERANGE:889@2048" + lineFeed + 
                "http://media.example.com/media.ts" + lineFeed;
        }

        protected static string CreateValidMasterPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed + 
                "#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000" + lineFeed + " http://example.com/low.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000" + lineFeed + "http://example.com/mid.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000" + lineFeed + "http://example.com/hi.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=65000,CODECS=\"mp4a.40.5\"" + lineFeed + "http://example.com/audio-only.m3u8" + lineFeed;
        }

        protected static string CreateInvalidMasterPlaylist(string lineFeed)
        {
            return
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed + 
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,AVERAGE-BANDWIDTH=1000000" + lineFeed + " http://example.com/low.m3u8" + lineFeed +
                "#EXT-X-DISCONTINUITY" + lineFeed + 
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,AVERAGE-BANDWIDTH=2000000" + lineFeed + "http://example.com/mid.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,AVERAGE-BANDWIDTH=6000000" + lineFeed + "http://example.com/hi.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\"" + lineFeed + "http://example.com/audio-only.m3u8" + lineFeed;
        }

        protected static string CreateValidMasterPlaylistWithAlternativeRenditions(string lineFeed)
        {
            return 
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"English\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Deutsch\"," + 
                "DEFAULT=NO,AUTOSELECT=YES,LANGUAGE=\"de\"," +
                "URI=\"main/german-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Commentary\"," +
                "DEFAULT=NO,AUTOSELECT=NO,LANGUAGE=\"en\"," + 
                "URI=\"commentary/audio-only.m3u8\"" + lineFeed +
                "#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=1280000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "low/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=2560000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "mid/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=7680000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "hi/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=65000,CODECS=\"mp4a.40.5\",AUDIO=\"aac\"" + lineFeed +
                "main/english-audio.m3u8" + lineFeed + lineFeed;
        }

        protected static string CreateValidMasterPlaylistWithAlternativeRenditionsTwoGroups(string lineFeed)
        {
            return 
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed +

                // Alternative audio "aac"
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"English\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"http://example.com/main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Deutsch\"," + 
                "DEFAULT=NO,AUTOSELECT=YES,LANGUAGE=\"de\"," +
                "URI=\"http://example.com/main/german-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Commentary\"," +
                "DEFAULT=NO,AUTOSELECT=NO,LANGUAGE=\"en\"," + 
                "URI=\"http://example.com/commentary/audio-only.m3u8\"" + lineFeed +

                // Alternative audio "mp3"
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"mp3\",NAME=\"English\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"http://example.com/main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"mp3\",NAME=\"Deutsch\"," + 
                "DEFAULT=NO,AUTOSELECT=YES,LANGUAGE=\"de\"," +
                "URI=\"http://example.com/main/german-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"mp3\",NAME=\"Commentary\"," +
                "DEFAULT=NO,AUTOSELECT=NO,LANGUAGE=\"en\"," + 
                "URI=\"http://example.com/commentary/audio-only.m3u8\"" + lineFeed +

                // Alternative video angles
                "#EXT-X-MEDIA:TYPE=VIDEO,GROUP-ID=\"angle-1\",NAME=\"Video Angle 1\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"http://example.com/main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=VIDEO,GROUP-ID=\"angle-2\",NAME=\"Video Angle 2\"," + 
                "DEFAULT=NO,AUTOSELECT=YES,LANGUAGE=\"de\"," +
                "URI=\"http://example.com/main/german-audio.m3u8\"" + lineFeed +

                // Variant streams
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/low/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/mid/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/hi/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/main/english-audio.m3u8" + lineFeed + lineFeed;
        }

        protected static string CreateInvalidMasterPlaylistWithAlternativeRenditionsNonUniqueNames(string lineFeed)
        {
            return 
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"English\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"http://example.com/main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"English\"," + 
                "DEFAULT=NO,AUTOSELECT=NO,LANGUAGE=\"de\"," + 
                "URI=\"http://example.com/main/german-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Commentary\"," +
                "DEFAULT=YES,LANGUAGE=\"fi\"," + 
                "URI=\"http://example.com/commentary/audio-only.m3u8\"" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/low/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/mid/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/hi/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/main/english-audio.m3u8" + lineFeed + lineFeed;
        }

        protected static string CreateInvalidMasterPlaylistWithAlternativeRenditionsMultipleDefaults(string lineFeed)
        {
            return 
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"English\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"http://example.com/main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Deutsch\"," + 
                "DEFAULT=NO,AUTOSELECT=NO,LANGUAGE=\"de\"," + 
                "URI=\"http://example.com/main/german-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Commentary\"," +
                "DEFAULT=YES,LANGUAGE=\"fi\"," + 
                "URI=\"http://example.com/commentary/audio-only.m3u8\"" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/low/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/mid/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/hi/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/main/english-audio.m3u8" + lineFeed + lineFeed;
        }

        protected static string CreateInvalidMasterPlaylistWithAlternativeRenditionsAutoselectWithNonUniqueLanguages(string lineFeed)
        {
            return 
                "#EXTM3U" + lineFeed +
                "#EXT-X-VERSION:7" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"English\"," + 
                "DEFAULT=YES,AUTOSELECT=YES,LANGUAGE=\"en\"," +
                "URI=\"http://example.com/main/english-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Deutsch\"," + 
                "DEFAULT=NO,AUTOSELECT=YES,LANGUAGE=\"en\"," + 
                "URI=\"http://example.com/main/german-audio.m3u8\"" + lineFeed +
                "#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"aac\",NAME=\"Commentary\"," +
                "DEFAULT=NO,AUTOSELECT=NO,LANGUAGE=\"de\"," + 
                "URI=\"http://example.com/commentary/audio-only.m3u8\"" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=1280000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/low/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=2560000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/mid/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=7680000,CODECS=\"aac\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/hi/video-only.m3u8" + lineFeed +
                "#EXT-X-STREAM-INF:BANDWIDTH=65000,CODECS=\"mp4a.40.5\",AUDIO=\"aac\"" + lineFeed +
                "http://example.com/main/english-audio.m3u8" + lineFeed + lineFeed;
        }

        protected static string CreateValidMediaPlaylistWithEncryptionDetails(string lineFeed)
        {
            return 
                "#EXTM3U" + lineFeed + 
                "#EXT-X-TARGETDURATION:10" + lineFeed + 
                "#EXT-X-VERSION:3" + lineFeed + 
                "#EXT-X-MEDIA-SEQUENCE:0" + lineFeed + 
                "#EXT-X-KEY:METHOD=AES-128,URI=\"https://priv.example.com/key.php?r=52\",IV=0x9c7db8778570d05c3177c349fd9236aa" + lineFeed + 
                "#EXTINF:10.0," + lineFeed + 
                "bumper0.ts" + lineFeed + 
                "#EXTINF:8.0," + lineFeed + 
                "bumper1.ts" + lineFeed + 
                "#EXT-X-DISCONTINUITY" + lineFeed + 
                "#EXT-X-KEY:METHOD=AES-128,URI=\"https://priv.example.com/key.php?r=53\",IV=0xc055ee9f6c1eb7aa50bfab02b0814972" + lineFeed + 
                "#EXTINF:10.0," + lineFeed + 
                "movieA.ts" + lineFeed + 
                "#EXTINF:10.0," + lineFeed + 
                "movieB.ts" + lineFeed;
        }

        internal static void AssertMasterPlaylist(IHlsPlaylist playlistObject)
        {
            Assert.IsNotNull(playlistObject);
            Assert.That(playlistObject is HlsPlaylist);
            Assert.IsNotNull(playlistObject.Playlist);
            Assert.That(playlistObject.IsMaster);
            Assert.AreEqual(7, playlistObject.Version);
            Assert.AreEqual(4, playlistObject.Playlist.Tags.Count);
            var master = (MasterPlaylist) playlistObject.Playlist;
            Assert.AreEqual(0, master.RenditionGroups.Count);
            Assert.AreEqual(4, master.VariantStreams.Count);
        }

        internal static void AssertMediaPlaylist(IHlsPlaylist playlistObject)
        {
            Assert.IsNotNull(playlistObject);
            Assert.That(playlistObject is HlsPlaylist);
            Assert.IsNotNull(playlistObject.Playlist);
            Assert.That(playlistObject.Playlist is MediaPlaylist);
            var mediaPlaylist = (MediaPlaylist) playlistObject.Playlist;
            Assert.AreEqual(3, mediaPlaylist.MediaSegments.Count);
            Assert.AreEqual(6, playlistObject.Version);
        }

        // TODO: test segment properties
    }
}
