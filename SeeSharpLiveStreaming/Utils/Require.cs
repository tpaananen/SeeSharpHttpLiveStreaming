using System;
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


    }
}
