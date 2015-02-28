using System;
using System.IO;
using System.Runtime.Serialization;
using SeeSharpLiveStreaming.Playlist.Tags;

namespace SeeSharpLiveStreaming.Utils
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
        /// Reads line by line from the <paramref name="playlist" /> until a non empty line is read.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>
        /// Non empty line if the line read property or <b>null</b> is no such line is found.
        /// </returns>
        internal static string ReadWhileNonEmptyLine(string playlist, int lineNumber)
        {
            int currentLineNumber = 0;
            using (var stringReader = new StringReader(playlist))
            {
                string line;
                do
                {
                    while (currentLineNumber < lineNumber)
                    {
                        if (!string.IsNullOrEmpty(stringReader.ReadLine()))
                        {
                            ++currentLineNumber;
                        }
                    }
                    
                    line = stringReader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        ++currentLineNumber;
                    }

                } while (line == Tag.StartLine || string.IsNullOrWhiteSpace(line));

                return line;
            }
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
    }
}
