using System;

namespace SeeSharpHttpLiveStreaming.Utils
{
    internal static class UriUtils
    {
        /// <summary>
        /// Creates a new URI using provided parameters.
        /// </summary>
        /// <param name="uriString">The URI string.</param>
        /// <param name="baseUri">The base URI.</param>
        /// <returns>
        /// If the <paramref name="uriString"/> is well formed absolute URI, the return value is 
        /// created only using <paramref name="uriString"/>; otherwise also <paramref name="baseUri"/>
        /// is used to create a new URI.
        /// </returns>
        internal static Uri CreateUri(string uriString, Uri baseUri)
        {
            return Uri.IsWellFormedUriString(uriString, UriKind.Absolute) 
                ? new Uri(uriString) 
                : new Uri(baseUri, uriString);
        }
    }
}
