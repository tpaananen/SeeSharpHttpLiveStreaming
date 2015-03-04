using System;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Represents the resolution value.
    /// </summary>
    public struct Resolution : IEquatable<Resolution>
    {
        /// <summary>
        /// The x component.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// The y component.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Resolution(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the default resolution of 0x0.
        /// </summary>
        public static Resolution Default
        {
            get { return new Resolution(0, 0); }
        }

        /// <summary>
        /// Determines whether the specified <see cref="Resolution" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Resolution" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="Resolution" /> is equal to this instance; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public bool Equals(Resolution other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Resolution && Equals((Resolution)obj);
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
                return (X * 397) ^ Y;
            }
        }

        #pragma warning disable 1591
        public static bool operator ==(Resolution left, Resolution right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Resolution left, Resolution right)
        {
            return !left.Equals(right);
        }
    }
}
