using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Static factory class to create dynamically tags based on their names.
    /// </summary>
    internal static class TagFactory
    {
        private const BindingFlags BindingFlag = BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance;

        internal static readonly IReadOnlyDictionary<string, Func<BaseTag>> TypeMapping;

        /// <summary>
        /// Initializes the <see cref="TagFactory"/> class.
        /// </summary>
        static TagFactory()
        {
            TypeMapping = new ReadOnlyDictionary<string, Func<BaseTag>>(FillDictionary());
        }

        /// <summary>
        /// Fills the dictionary with tag names and types for later use.
        /// </summary>
        private static IDictionary<string, Func<BaseTag>> FillDictionary()
        {
            // Fill the dictionary using reflection, done only once
            var assembly = Assembly.GetAssembly(typeof (BaseTag));
            var types = new List<Type>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract)
                {
                    continue;
                }
                if (typeof (BaseTag).IsAssignableFrom(type))
                {
                    types.Add(type);
                }
            }
            return FillDictionary(types);
        }

        /// <summary>
        /// Fills the dictionary.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <exception cref="InvalidOperationException">The tag  + instance.TagName +  is invalid.</exception>
        private static IDictionary<string, Func<BaseTag>> FillDictionary(IEnumerable<Type> types)
        {
            types.RequireNotNull("types");
            var dictionary = new Dictionary<string, Func<BaseTag>>();
            // We'll take only internal parameterless constructors
            
            foreach (var type in types)
            {
                var instanceCreator = CreateConstructor(type);
                var instance = instanceCreator();
                ValidateAndAddTag(instance.TagName, instanceCreator, dictionary);
            }
            return dictionary;
        }

        /// <summary>
        /// Validates and adds tag if it is valid.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="creator">The creator.</param>
        /// <param name="container">The container.</param>
        /// <exception cref="InvalidOperationException">Thrown if the tag with the <paramref name="name" /> already exists in the <paramref name="container" />.</exception>
        internal static void ValidateAndAddTag(string name, Func<BaseTag> creator, IDictionary<string, Func<BaseTag>> container)
        {
            container.RequireNotNull("container");
            if (!Tag.IsValid(name))
            {
                throw new InvalidOperationException("The tag " + name + " is invalid.");
            }
            if (container.ContainsKey(name))
            {
                throw new InvalidOperationException("The tag " + name + " already exists.");
            }
            container[name] = creator;
        }

        /// <summary>
        /// Creates a new instance of a tag based on its name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when the tag with the <paramref name="name"/> does not exist.
        /// </exception>
        internal static BaseTag Create(string name)
        {
            Func<BaseTag> creator;
            if (!TypeMapping.TryGetValue(name, out creator))
            {
                throw new NotSupportedException("The tag " + name + " is not supported.");
            }
            return creator();
        }

        /// <summary>
        /// Creates the constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal static Func<BaseTag> CreateConstructor(Type type)
        {
            var creator = type.GetConstructor(BindingFlag, null, CallingConventions.Any, new Type[0], null);
            var lambda = Expression.Lambda<Func<BaseTag>>(Expression.New(creator));
            return lambda.Compile();
        }

        /// <summary>
        /// Creates the tag specified by the <paramref name="line"/>.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when the <paramref name="line"/> contains only <see cref="Tag.StartLine"/>.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown if parsing of the content fails.
        /// </exception>
        internal static BaseTag Create(PlaylistLine line, int version)
        {
            if (line.Tag == Tag.StartLine)
            {
                throw new InvalidOperationException("The start tag cannot be created.");
            }

            var tagObject = Create(line.Tag);
            tagObject.Deserialize(line.GetParameters(), version);
            return tagObject;
        }
    }
}
