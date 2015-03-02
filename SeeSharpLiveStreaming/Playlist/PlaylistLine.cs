using System;
using SeeSharpLiveStreaming.Utils;

namespace SeeSharpLiveStreaming.Playlist
{
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
        /// Initializes a new instance of the <see cref="PlaylistLine"/> struct.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="line">The line.</param>
        internal PlaylistLine(string tag, string line)
        {
            tag.RequireNotNull("tag");
            line.RequireNotNull("line");
            Tag = tag;
            Line = line;
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

        public bool Equals(PlaylistLine other)
        {
            return string.Equals(Tag, other.Tag) && 
                   string.Equals(Line, other.Line);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is PlaylistLine && Equals((PlaylistLine)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Tag.GetHashCode() * 397) ^ Line.GetHashCode();
            }
        }

        public static bool operator ==(PlaylistLine left, PlaylistLine right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlaylistLine left, PlaylistLine right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}
