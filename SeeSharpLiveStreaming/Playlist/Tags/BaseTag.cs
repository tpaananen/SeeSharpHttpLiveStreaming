﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using SeeSharpLiveStreaming.Playlist.Tags.BasicTags;
using SeeSharpLiveStreaming.Playlist.Tags.Master;

namespace SeeSharpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Represents the base tag for each tag described in HTTP Live Streaming specification.
    /// </summary>
    public abstract class BaseTag : ISerializable
    {

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public abstract TagType TagType { get; }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        public abstract void Deserialize(string content, int version);

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

            BaseTag tagObject;
            switch (line.Tag)
            {
                case "#EXT-X-VERSION":
                    tagObject = new ExtXVersion();
                    break;

                case "#EXT-X-STREAM-INF":
                    tagObject = new StreamInf();
                    break;

                default:
                    throw new SerializationException("No handling for tag " + line.Tag);
            }

            tagObject.Deserialize(line.GetParameters(), version);
            return tagObject;

        }

    }
}