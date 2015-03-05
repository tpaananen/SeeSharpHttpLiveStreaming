using System;
using System.Runtime.Serialization;
using SeeSharpHttpLiveStreaming.Playlist.Tags.BasicTags;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Master;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media;
using SeeSharpHttpLiveStreaming.Playlist.Tags.Media.MediaSegment;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Represents the base tag for each tag described in HTTP Live Streaming specification.
    /// </summary>
    public abstract class BaseTag : ISerializable
    {
        /// <summary>
        /// Gets the name of the tag, for example EXT-X-MEDIA.
        /// </summary>
        public abstract string TagName { get; }

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

            // TODO: replace switch case with some cacheable factory dictionary

            BaseTag tagObject;
            if (Tag.IsMediaSegmentTag(line.Tag))
            {
                tagObject = CreateMediaSegment(line);
            }
            else if (Tag.IsMasterTag(line.Tag))
            {
                tagObject = CreateMasterTag(line);
            }
            else if (Tag.IsMediaPlaylistTag(line.Tag))
            {
                tagObject = CreateMediaPlaylistTag(line);
            }
            else if (Tag.IsBasicTag(line.Tag))
            {
                tagObject = CreateBasicTag(line);
            }
            else
            {
                throw new SerializationException("No handling for tag " + line.Tag);
            }
            
            tagObject.Deserialize(line.GetParameters(), version);
            return tagObject;
        }

        private static BaseTag CreateMasterTag(PlaylistLine line)
        {
            switch (line.Tag)
            {
                case "#EXT-X-STREAM-INF":
                    return new StreamInf();

                case "#EXT-X-MEDIA":
                    return new ExtMedia();

                default:
                    throw new SerializationException("No handling for tag " + line.Tag);
            }
        }

        private static BaseTag CreateMediaPlaylistTag(PlaylistLine line)
        {
            switch (line.Tag)
            {
                case "#EXT-X-TARGETDURATION":
                    return new TargetDuration();

                default:
                    throw new SerializationException("No handling for tag " + line.Tag);
            }
        }

        private static BaseTag CreateBasicTag(PlaylistLine line)
        {
            if (line.Tag != "#EXT-X-VERSION")
            {
                throw new NotSupportedException("The basic tag " + line.Tag + " is not supported at this point.");
            }
            return new ExtXVersion();
        }

        private static BaseTag CreateMediaSegment(PlaylistLine line)
        {
            switch (line.Tag)
            {
                case "#EXTINF":
                    return new ExtInf();
                    
                case "#EXT-X-BYTERANGE":
                    return new ExtXByteRange();

                case "#EXT-X-DISCONTINUITY":
                    throw new NotImplementedException("EXT-X-DISCONTINUITY");

                case "#EXT-X-KEY":
                    throw new NotImplementedException("EXT-X-KEY");
                    
                case "#EXT-X-MAP":
                    throw new NotImplementedException("EXT-X-MAP");
                    
                case "#EXT-X-PROGRAM-DATE-TIME":
                    throw new NotImplementedException("EXT-X-PROGRAM-DATE-TIME");

                default:
                    throw new NotSupportedException("Tag " + line.Tag + " is not supported as Media Segment tag.");
            }
        }
    }
}
