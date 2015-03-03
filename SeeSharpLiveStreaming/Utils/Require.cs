using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SeeSharpLiveStreaming.Utils
{
    internal static class Require
    {

        /// <summary>
        /// Requires that the <paramref name="instance"/> is not <b>null</b>.
        /// If the <paramref name="instance"/> is <b>null</b>, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if the <paramref name="instance"/> is <b>null</b>.
        /// </exception>
        [DebuggerStepThrough]
        internal static void RequireNotNull(this object instance, string name = "")
        {
            if (instance == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        internal static void RequireNotEmpty<T>(this ICollection<T> collection, string name = "")
        {
            RequireNotNull(collection, name);
            if (collection.Count == 0)
            {
                throw new ArgumentException("The collection " + name + " is empty.", name);
            }
        }
    }
}
