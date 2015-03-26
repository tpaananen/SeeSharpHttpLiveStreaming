using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SeeSharpHttpLiveStreaming.Playlist.Tags;

namespace SeeSharpHttpLiveStreaming.Utils
{
    /// <summary>
    /// Contains set of extension methods for argument validation.
    /// </summary>
    [DebuggerStepThrough]
    internal static class Require
    {

        /// <summary>
        /// Requires that the <paramref name="instance"/> is not <b>null</b>.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if the <paramref name="instance"/> is <b>null</b>.
        /// </exception>
        internal static void RequireNotNull(this object instance, string name = "")
        {
            if (instance == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// Requires that the <paramref name="instance"/> is not <b>null</b> or empty.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if the <paramref name="instance"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the <paramref name="instance"/> is empty.
        /// </exception>
        internal static void RequireNotEmpty(this string instance, string name = "")
        {
            RequireNotNull(instance, name);
            if (instance == string.Empty)
            {
                throw new ArgumentException("Validation failed: The " + name + " is empty.");
            }
        }

        /// <summary>
        /// Requires that the <paramref name="collection"/> is not <b>null</b> or empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if the <paramref name="collection"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the <paramref name="collection"/> is empty.
        /// </exception>
        internal static void RequireNotEmpty<T>(this IReadOnlyCollection<T> collection, string name = "")
        {
            RequireNotNull(collection, name);
            if (collection.Count == 0)
            {
                throw new ArgumentException("Validation failed: The collection " + name + " is empty.", name);
            }
        }

        /// <summary>
        /// Requires that the default constructor was not used.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="caller">The caller.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <paramref name="tag"/> was created using default constructor.
        /// </exception>
        internal static void RequireNoDefaultConstructor(this BaseTag tag, [CallerMemberName] string caller = null)
        {
            if (tag.UsingDefaultCtor)
            {
                throw new InvalidOperationException("The method " + caller + " requires that the " + tag.GetType() 
                    + " was not created using default constructor.");
            }
        }
    }
}
