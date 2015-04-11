namespace SeeSharpHttpLiveStreaming.Playlist.Tags.Master
{
    /// <summary>
    /// Represents a variant stream in a master playlist.
    /// </summary>
    internal sealed class VariantStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariantStream" /> class.
        /// </summary>
        /// <param name="defaultStream">The default stream.</param>
        /// <param name="alternativeVideo">The alternative video.</param>
        /// <param name="alternativeAudio">The alternative audio.</param>
        /// <param name="alternativeSubtitles">The alternative subtitles.</param>
        /// <param name="alternativeClosedCaptions">The alternative closed captions.</param>
        internal VariantStream(StreamInf defaultStream, 
                               RenditionGroup alternativeVideo, RenditionGroup alternativeAudio,
                               RenditionGroup alternativeSubtitles, RenditionGroup alternativeClosedCaptions)
        {
            DefaultStream = defaultStream;
            AlternativeVideo = alternativeVideo;
            AlternativeAudio = alternativeAudio;
            AlternativeSubtitles = alternativeSubtitles;
            AlternativeClosedCaptions = alternativeClosedCaptions;
        }

        /// <summary>
        /// Gets the default stream.
        /// </summary>
        public StreamInf DefaultStream { get; private set; }
        
        /// <summary>
        /// Gets the alternative video.
        /// </summary>
        public RenditionGroup AlternativeVideo { get; private set; }

        /// <summary>
        /// Gets the alternative audio.
        /// </summary>
        public RenditionGroup AlternativeAudio { get; private set; }

        /// <summary>
        /// Gets the alternative subtitles.
        /// </summary>
        public RenditionGroup AlternativeSubtitles { get; private set; }

        /// <summary>
        /// Gets the alternative closed captions.
        /// </summary>
        public RenditionGroup AlternativeClosedCaptions { get; private set; }
    }
}
