using System;
using System.Globalization;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Utils;
using SeeSharpHttpLiveStreaming.Utils.Writers;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Represents the base tag for each tag described in HTTP Live Streaming specification.
    /// </summary>
    public abstract class BaseTag : ISerializable
    {
        /// <summary>
        /// The attribute separator.
        /// </summary>
        protected const string AttributeSeparator = ",";
        
        /// <summary>
        /// The decimal format specifier.
        /// </summary>
        protected const string DecimalFormatSpecifier = "F2";

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public abstract string TagName { get; }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public abstract TagType TagType { get; }

        /// <summary>
        /// Gets a value indicating whether this tag has attributes.
        /// </summary>
        public virtual bool HasAttributes
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class deserializes the tag from the <paramref name="content" />.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="SerializationException">Thrown when the deserialization fails.</exception>
        public abstract void Deserialize(string content, int version);

        /// <summary>
        /// Serializes the tag to the <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="SerializationException">Thrown when the serialization fails.</exception>
        public void Serialize(IPlaylistWriter writer)
        {
            writer.RequireNotNull("writer");

            writer.Write(TagName);
            BeginWriteAttributes(writer);
            SerializeAttributes(writer);
            writer.WriteLineEnd();
        }

        private void BeginWriteAttributes(IPlaylistWriter writer)
        {
            if (HasAttributes)
            {
                writer.Write(Tag.TagEndMarker);
            }
        }

        /// <summary>
        /// When overridden in a derived class serializes the attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void SerializeAttributes(IPlaylistWriter writer)
        {
        }

        /// <summary>
        /// Creates the specified content.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">The start tag cannot be created.</exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">Thrown if parsing of the content fails.</exception>
        internal static BaseTag Create(PlaylistLine line, int version)
        {
            if (line.Tag == Tag.StartLine)
            {
                throw new InvalidOperationException("The start tag cannot be created.");
            }

            var tagObject = TagFactory.Create(line.Tag);
            tagObject.Deserialize(line.GetParameters(), version);
            return tagObject;
        }

        /// <summary>
        /// Formats the decimal value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Decimal as a string.
        /// </returns>
        protected virtual string FormatDecimal(decimal value)
        {
            return value.ToString(DecimalFormatSpecifier, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Writes the attribute separator.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="hasPreviousAttributes">if set to <c>true</c> has previous attributes.</param>
        protected static void WriteAttributeSeparator(IPlaylistWriter writer, bool hasPreviousAttributes)
        {
            if (hasPreviousAttributes)
            {
                writer.Write(AttributeSeparator);
            }
        }

        /// <summary>
        /// Writes a quoted string.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        /// <param name="hasPreviousAttribute">if set to <c>true</c> [has previous attribute].</param>
        protected static void WriteQuotedString(IPlaylistWriter writer, string attribute, string value, ref bool hasPreviousAttribute)
        {
            const string template = "{0}=\"{1}\"";
            WriteAttributeSeparator(writer, hasPreviousAttribute);
            writer.Write(string.Format(template, attribute, value));
            hasPreviousAttribute = true;
        }

        /// <summary>
        /// Writes a quoted string.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        /// <param name="hasPreviousAttribute">if set to <c>true</c> [has previous attribute].</param>
        protected static void WriteEnumeratedString(IPlaylistWriter writer, string attribute, string value, ref bool hasPreviousAttribute)
        {
            const string template = "{0}={1}";
            WriteAttributeSeparator(writer, hasPreviousAttribute);
            writer.Write(string.Format(template, attribute, value));
            hasPreviousAttribute = true;
        }
    }
}
