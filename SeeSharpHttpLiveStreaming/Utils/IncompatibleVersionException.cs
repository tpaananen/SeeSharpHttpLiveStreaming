using System;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Utils
{
    /// <summary>
    /// Exception that represents that a tag cannot be parsed due to too low version number.
    /// </summary>
    public class IncompatibleVersionException : Exception
    {

        /// <summary>
        /// Gets the current version.
        /// </summary>
        public int CurrentVersion { get; private set; }

        /// <summary>
        /// Gets the required version.
        /// </summary>
        public int RequiredVersion { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncompatibleVersionException" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="version">The version.</param>
        /// <param name="requiredVersion">The required version.</param>
        public IncompatibleVersionException(BaseTag tag, int version, int requiredVersion)
            : base ("Tag " + tag.TagName + " could not be parsed, current version " + version 
                    + " is incompatible with required version " + requiredVersion + ".")
        {
        }

    }
}
