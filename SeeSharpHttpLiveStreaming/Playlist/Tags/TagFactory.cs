using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SeeSharpHttpLiveStreaming.Utils;

namespace SeeSharpHttpLiveStreaming.Playlist.Tags
{
    /// <summary>
    /// Static factory class to create dynamically tags based on their names.
    /// </summary>
    internal static class TagFactory
    {
        private static readonly IDictionary<string, Type> TypeMapping = new Dictionary<string, Type>();

        /// <summary>
        /// Initializes the <see cref="TagFactory"/> class.
        /// </summary>
        static TagFactory()
        {
            FillDictionary();
        }

        /// <summary>
        /// Fills the dictionary with tag names and types for later use.
        /// </summary>
        private static void FillDictionary()
        {
            // Fill the dictionary using reflection, done only once
            var assembly = Assembly.GetAssembly(typeof (BaseTag));
            var types = assembly.GetTypes()
                                .Where(t => !t.IsAbstract &&
                                            !t.IsInterface &&
                                            typeof(BaseTag).IsAssignableFrom(t))
                                .ToList();
            FillDictionary(types);
        }

        /// <summary>
        /// Fills the dictionary.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <exception cref="InvalidOperationException">The tag  + instance.TagName +  is invalid.</exception>
        private static void FillDictionary(IEnumerable<Type> types)
        {
            types.RequireNotNull("types");

            foreach (var type in types)
            {
                var instance = (BaseTag)Activator.CreateInstance(type);
                ValidateAndAddTag(instance.TagName, type);
            }
        }

        /// <summary>
        /// Validates and adds tag if it is valid.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="InvalidOperationException">The tag  + instance.TagName +  is invalid.</exception>
        internal static void ValidateAndAddTag(string name, Type type)
        {
            if (!Tag.IsValid(name))
            {
                throw new InvalidOperationException("The tag " + name + " is invalid.");
            }
            if (TypeMapping.ContainsKey(name))
            {
                throw new InvalidOperationException("The tag " + name + " already exists.");
            }
            TypeMapping[name] = type;
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
