using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SeeSharpHttpLiveStreaming.Utils
{
    /// <summary>
    /// Exception that represents that a tag cannot be parsed due to too low version number.
    /// </summary>
    [Serializable]
    internal class IncompatibleVersionException : Exception
    {
        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public string TagName { get; private set; }

        /// <summary>
        /// Gets the current version.
        /// </summary>
        public int CurrentVersion { get; private set; }

        /// <summary>
        /// Gets the required version.
        /// </summary>
        public int RequiredVersion { get; private set; }

        /// <summary>
        /// Gets the attribute name.
        /// </summary>
        public string Attribute { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncompatibleVersionException" /> class.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="version">The version.</param>
        /// <param name="requiredVersion">The required version.</param>
        public IncompatibleVersionException(string tagName, string attribute, int version, int requiredVersion)
            : base ("Tag " + tagName + " could not be parsed, current version " + version 
                    + " is incompatible with required version " + requiredVersion 
                    + " while parsing attribute " + attribute + ".")
        {
            TagName = tagName;
            CurrentVersion = version;
            RequiredVersion = requiredVersion;
            Attribute = attribute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncompatibleVersionException" /> class.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="version">The version.</param>
        /// <param name="requiredVersion">The required version.</param>
        public IncompatibleVersionException(string tagName, int version, int requiredVersion)
            : base ("Tag " + tagName + " could not be parsed, current version " + version 
                    + " is incompatible with required version " + requiredVersion + ".")
        {
            TagName = tagName;
            CurrentVersion = version;
            RequiredVersion = requiredVersion;
        }

        /// <summary>
        /// Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("TagName", TagName);
            info.AddValue("CurrentVersion", CurrentVersion);
            info.AddValue("RequiredVersion", RequiredVersion);
            info.AddValue("Attribute", Attribute);
        }
    }
}
