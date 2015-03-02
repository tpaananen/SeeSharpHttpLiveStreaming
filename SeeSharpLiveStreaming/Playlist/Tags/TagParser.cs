﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace SeeSharpLiveStreaming.Playlist.Tags
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
                // The start of the line is '#' and there should be something other characters also, so assume
                // we need at least 1 char more, typically we need many more but this is enough...
                return string.Empty;
            }

            // tag is reprented without the end marker.
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
        internal static IList<PlaylistLine> ReadLines(string playlist)
        {
            var lines = new List<PlaylistLine>();
            using (var reader = new StringReader(playlist))
            {
                ReadFirstLine(reader);

                bool parseUriNotTag = false;

                string tag = null;
                string currentLine = null;

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (parseUriNotTag)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }
                        parseUriNotTag = false;
                        lines.Add(new PlaylistLine(tag, currentLine, line));
                        currentLine = null;
                    }
                    else
                    {
                        tag = ParseTag(line);
                        if (Tag.IsFollowedByUri(tag))
                        {
                            // Media segment tags are followed by uri
                            currentLine = line;
                            parseUriNotTag = true;
                            continue;
                        }

                        if (!string.IsNullOrEmpty(tag))
                        {
                            lines.Add(new PlaylistLine(tag, line));
                        }
                    }
                }
            }
            return lines;
        }
    }
}
