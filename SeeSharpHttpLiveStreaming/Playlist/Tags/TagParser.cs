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
        /// The tag parsed from the line or empty string if tag could not be parsed or the .
        /// </returns>
        internal static string ParseTag(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return string.Empty;
            }

            var indexOfEndMarker = line.IndexOf(Tag.TagEndMarker, StringComparison.Ordinal);
            if (indexOfEndMarker <= 1)
            {
                if (line.StartsWith("#") && !Tag.HasAttributes(line))
                {
                    // some tags do not have anything but name
                    return line;
                }

                // The start of the line is '#' and there should be something other characters also, so assume
                // we need at least 1 char more, typically we need many more but this is enough...
                return string.Empty;
            }

            // tag has no end marker.
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
        /// <returns></returns>
        internal static IReadOnlyCollection<PlaylistLine> ReadLines(string playlist)
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
                    
                    ProcessTag(tag, reader, lines, line);
                }
            }
            return new ReadOnlyCollection<PlaylistLine>(lines);
        }

        private static void ProcessTag(string tag, StringReader reader, List<PlaylistLine> lines, string line)
        {
            // Change this so that if it is media segment,
            // the next line can be uri but it might be another segment tag
            if (Tag.IsFollowedByUri(tag))
            {
                // Some tags might be followed by uri
                string uriOrTag = ReadUri(reader);
                if (uriOrTag == string.Empty)
                {
                    return;
                }

                // media segment tags might not be followed by uri
                if (uriOrTag.StartsWith("#"))
                {
                    lines.Add(new PlaylistLine(tag, line));
                    // another tag, so again
                    tag = ParseTag(uriOrTag);
                    ProcessTag(tag, reader, lines, uriOrTag);
                }
                else
                {
                    // now should be uri
                    lines.Add(new PlaylistLine(tag, line, uriOrTag));
                }
            }
            else
            {
                lines.Add(new PlaylistLine(tag, line));
            }
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
