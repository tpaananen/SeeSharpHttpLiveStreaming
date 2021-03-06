﻿using System;
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
        /// The optional URI, this should exist on its own line.
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
            if (line.EndsWith("\n"))
            {
                line = line.Replace("\n", "").Replace("\r", "");
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
        /// Thrown when the <paramref name="uri"/> is <b>null</b>.
        /// </exception>
        internal PlaylistLine(string tag, string line, Uri uri)
            : this(tag, line)
        {
            uri.RequireNotNull("uri");
            Uri = uri;
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
            return GetHashCode() == other.GetHashCode();
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
                var value = (Tag.GetHashCode() * 397) ^ Line.GetHashCode();
                if (Uri != null)
                {
                    value ^= Uri.GetHashCode();
                }
                return value;
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
