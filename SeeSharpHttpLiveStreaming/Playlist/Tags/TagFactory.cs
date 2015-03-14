using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Static factory class to create dynamically tags based on their names.
    /// </summary>
    internal static class TagFactory
    {
        internal static readonly IReadOnlyDictionary<string, Type> TypeMapping;

        /// <summary>
        /// Initializes the <see cref="TagFactory"/> class.
        /// </summary>
        static TagFactory()
        {
            TypeMapping = new ReadOnlyDictionary<string, Type>(FillDictionary());
        }

        /// <summary>
        /// Fills the dictionary with tag names and types for later use.
        /// </summary>
        private static IDictionary<string, Type> FillDictionary()
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
        private static IDictionary<string, Type> FillDictionary(IEnumerable<Type> types)
        {
            types.RequireNotNull("types");
            var dictionary = new Dictionary<string, Type>();
            foreach (var type in types)
            {
                var instance = (BaseTag)Activator.CreateInstance(type);
                ValidateAndAddTag(instance.TagName, type, dictionary);
            }
            return dictionary;
        }

        /// <summary>
        /// Validates and adds tag if it is valid.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="container">The container.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the tag with the <paramref name="name"/> already exists in the <paramref name="container"/>.
        /// </exception>
        internal static void ValidateAndAddTag(string name, Type type, IDictionary<string, Type> container)
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
            container[name] = type;
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
            Type type;
            if (!TypeMapping.TryGetValue(name, out type))
            {
                throw new NotSupportedException("The tag " + name + " is not supported.");
            }

            // Use reflection, this will cache simple constructor 
            // calls so will be fast enough for this purpose
            // since in filling method each tag has been created.
            return (BaseTag)Activator.CreateInstance(type);
        }

    }
}
