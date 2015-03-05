﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                                            typeof(BaseTag).IsAssignableFrom(t))
                                .ToList();
            
            foreach (var type in types)
            {
                var instance = (BaseTag)Activator.CreateInstance(type);
                TypeMapping[instance.TagName] = type;
            }
        }

        /// <summary>
        /// Creates the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">The tag  + name +  is not supported.</exception>
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