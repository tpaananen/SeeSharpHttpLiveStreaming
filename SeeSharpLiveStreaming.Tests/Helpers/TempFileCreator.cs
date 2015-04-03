using System;
using System.IO;
using System.Threading.Tasks;

namespace SeeSharpHttpLiveStreaming.Tests.Helpers
{
    /// <summary>
    /// Represents temp file creator.
    /// </summary>
    internal static class TempFileCreator
    {

        /// <summary>
        /// Provides safe context to run tests. In this context, a temp file is first created,
        /// then test <paramref name="action"/> is executed and finally the temp file is safely
        /// deleted.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="testDataContent">Content of the test data.</param>
        internal static void RunInSafeContext(string testDataContent, Action<Uri> action)
        {
            var uri = CreateTempFile(testDataContent);
            try
            {
                action(uri);
            }
            finally
            {
                DestroyFile(uri);
            }
        }

        /// <summary>
        /// Provides safe context to run tests. In this context, a temp file is first created,
        /// then test <paramref name="action"/> is executed and finally the temp file is safely
        /// deleted.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="testDataContent">Content of the test data.</param>
        internal static async Task RunInSafeContextAsync(string testDataContent, Func<Uri, Task> action)
        {
            var uri = CreateTempFile(testDataContent);
            try
            {
                await action(uri);
            }
            finally
            {
                DestroyFile(uri);
            }
        }

        /// <summary>
        /// Creates a temporary file, stores the <paramref name="content"/>
        /// into the file and returns the path to the file as <see cref="Uri"/>.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>
        /// The path to the file as <see cref="Uri"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times. File is left open by the factory and the internal stream.")]
        internal static Uri CreateTempFile(string content)
        {
            var file = Path.GetTempFileName();
            try
            {
                var uri = new Uri(file);
                using (var fs = File.OpenWrite(file))
                using (var writer = TestPlaylistWriterFactory.Create(fs))
                {
                    writer.Write(content);
                }
                return uri;
            }
            catch
            {
                DestroyFile(file);
                throw;
            }
        }

        /// <summary>
        /// Destroys the file.
        /// </summary>
        /// <param name="uri">The URI.</param>
        internal static void DestroyFile(Uri uri)
        {
            File.Delete(uri.OriginalString);
        }

        /// <summary>
        /// Destroys the file.
        /// </summary>
        /// <param name="path">The path.</param>
        internal static void DestroyFile(string path)
        {
            File.Delete(path);
        }

    }
}
