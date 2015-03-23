using System.IO;
using System.Text;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Tests.Helpers
{
    internal static class TestPlaylistWriterFactory
    {
        /// <summary>
        /// Creates the writer with string builder as a backing storage.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <returns>
        /// The <see cref="IPlaylistWriter"/> instance.
        /// </returns>
        public static IPlaylistWriter CreateWithStringBuilder(out StringBuilder stringBuilder)
        {
            stringBuilder = new StringBuilder();
            var internalWriter = new StringWriter(stringBuilder);
            return new PlaylistWriter(internalWriter);
        }
    }
}
