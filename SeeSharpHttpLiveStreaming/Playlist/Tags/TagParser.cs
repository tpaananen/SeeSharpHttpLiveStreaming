using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Represents a tag parser.
    /// </summary>
    internal static class TagParser
    {

        /// <summary>
        /// Parses the tag. If the tag cannot be parsed, returns an empty string that 
        /// means that the tag is not supported or could not be read. In this case tag 
        /// is ignored and the parser should move to the next line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>
        /// The tag parsed from the line or empty string if tag could not be parsed or the tag was not supported
        /// by the current version of the protocol.
        /// </returns>
        internal static string ParseTag(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return string.Empty;
            }

            if (!line.StartsWith(Tag.StartChar))
            {
                return string.Empty;
            }

            var indexOfEndMarker = line.IndexOf(Tag.TagEndMarker, StringComparison.Ordinal);
            if (indexOfEndMarker <= 1)
            {
                if (!Tag.HasAttributes(line))
                {
                    // some tags do not have anything but name
                    return line;
                }
                // Tag should have attributes, we will pass the control to the valid tag itself, 
                // or should we refuse to parse?
                return Tag.IsValid(line) ? line : string.Empty;
            }

            // The tag has an end marker, check if it is a valid tag
            var tag = line.Substring(0, indexOfEndMarker);
            return Tag.IsValid(tag) ? tag : string.Empty;
        }

        /// <summary>
        /// Reads the first line from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <exception cref="SerializationException">
        /// Thrown when the line read from the <paramref name="reader"/> is not the same as 
        /// <see cref="Tag.StartLine"/>.
        /// </exception>
        internal static void ReadFirstLine(TextReader reader)
        {
            var firstLine = reader.ReadLine();
            if (firstLine != Tag.StartLine)
            {
                throw new SerializationException(string.Format("The start tag of the playlist is not {0}.", Tag.StartLine));
            }
        }

        /// <summary>
        /// Reads the lines from the playlist. Returned list of lines does not contain the start line
        /// but is validated that the playlist starts correctly.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The collection of <see cref="PlaylistLine"/> structs.
        /// </returns>
        internal static IReadOnlyCollection<PlaylistLine> ReadLines(string playlist, Uri uri)
        {
            playlist.RequireNotEmpty("playlist");

            var lines = new List<PlaylistLine>();
            using (var reader = new StringReader(playlist))
            {
                ReadFirstLine(reader);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string tag = ParseTag(line);
                    if (string.IsNullOrEmpty(tag))
                    {
                        continue;
                    }
                    
                    ProcessTag(tag, reader, lines, line, uri);
                }
            }
            return new ReadOnlyCollection<PlaylistLine>(lines);
        }

        private static void ProcessTag(string tag, TextReader reader, ICollection<PlaylistLine> lines, string line, Uri baseUri)
        {
            while (true)
            {
                if (Tag.IsFollowedByUri(tag))
                {
                    // Some tags might be followed by uri
                    // read from the stream 1..n lines until got something
                    string uriOrTag = ReadUri(reader);

                    // media segment tags might not be followed by uri
                    // This should be a valid tag
                    if (uriOrTag.StartsWith(Tag.StartChar))
                    {
                        lines.Add(new PlaylistLine(tag, line));
                        // another tag found while trying to find the URI
                        // The tag was stored and heading to parse a new tag
                        tag = ParseTag(uriOrTag);
                        line = uriOrTag;
                        continue;
                    }

                    // now should be uri or fails to parse
                    if (uriOrTag == string.Empty)
                    {
                        throw new SerializationException("The URI is missing.");
                    }
                    var uri = CreateUri(uriOrTag, baseUri);
                    lines.Add(new PlaylistLine(tag, line, uri));
                }
                else
                {
                    // this is a valid tag without URI
                    lines.Add(new PlaylistLine(tag, line));
                }
                break;
            }
        }

        private static Uri CreateUri(string uriString, Uri baseUri)
        {
            return Uri.IsWellFormedUriString(uriString, UriKind.Absolute) 
                ? new Uri(uriString) 
                : new Uri(baseUri, uriString);
        }

        private static string ReadUri(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line != string.Empty && !line.All(Char.IsWhiteSpace))
                {
                    break;
                }
            }
            return line ?? string.Empty;
        }
    }
}
