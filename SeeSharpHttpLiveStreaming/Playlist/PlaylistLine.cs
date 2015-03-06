using System;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist
{
    /// <summary>
    /// Reprents the playlist line having the tag and the whole line.
    /// </summary>
    public struct PlaylistLine : IEquatable<PlaylistLine>
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        public readonly string Tag;

        /// <summary>
        /// Gets the line.
        /// </summary>
        public readonly string Line;

        /// <summary>
        /// The optional URI, this should exists on its own line.
        /// </summary>
        public readonly Uri Uri;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistLine" /> struct.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="line">The line.</param>
        internal PlaylistLine(string tag, string line)
        {
            tag.RequireNotNull("tag");
            line.RequireNotNull("line");

            if (!line.StartsWith(tag))
            {
                throw new ArgumentException("Line parameter does not start with the tag.");
            }

            Tag = tag;
            Line = line;
            Uri = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistLine"/> struct.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="line">The line.</param>
        /// <param name="uri">The URI.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the URI could not be created.
        /// </exception>
        internal PlaylistLine(string tag, string line, string uri)
            : this(tag, line)
        {
            Uri result;

            if (!Uri.TryCreate(uri, UriKind.Absolute, out result))
            {
                throw new ArgumentException("The URI could not be created.", "uri");
            }
            Uri = result;
        }

        /// <summary>
        /// Gets the parameters of the line. Returned string does not contain the tag itself.
        /// </summary>
        /// <returns></returns>
        public string GetParameters()
        {
            int tagLength = Tag.Length + Tags.Tag.TagEndMarker.Length;
            if (tagLength >= Line.Length)
            {
                return string.Empty;
            }
            return Line.Substring(tagLength);
        }

        #region Equality members

        /// <summary>
        /// Compares the <paramref name="other"/> to this instance.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        /// True if they are equal; otherwise, false.
        /// </returns>
        public bool Equals(PlaylistLine other)
        {
            return string.Equals(Tag, other.Tag) && 
                   string.Equals(Line, other.Line);
        }

        /// <summary>
        /// Compares the <paramref name="obj"/> to this instance.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>
        /// True if they are equal; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is PlaylistLine && Equals((PlaylistLine)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Tag.GetHashCode() * 397) ^ Line.GetHashCode();
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(PlaylistLine left, PlaylistLine right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(PlaylistLine left, PlaylistLine right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}
