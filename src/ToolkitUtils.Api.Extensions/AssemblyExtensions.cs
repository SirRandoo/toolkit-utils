using System;
using System.Reflection;
using JetBrains.Annotations;

namespace ToolkitUtils.Api.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        ///     Attempts to get the attribute provided.
        /// </summary>
        /// <param name="assembly">
        ///     The assembly containing the attribute being
        ///     retrieved.
        /// </param>
        /// <param name="attribute">
        ///     The attribute type being retrieved from the
        ///     assembly.
        /// </param>
        /// <typeparam name="T">The type of the attribute being retrieved.</typeparam>
        /// <returns>
        ///     Whether the attribute was retrieved from the assembly
        ///     metadata.
        /// </returns>
        [ContractAnnotation("=> true, attribute: notnull; => false, attribute: null")]
        public static bool TryGetAttribute<T>([NotNull] this Assembly assembly, out T attribute) where T : Attribute
        {
            object[] attributes = assembly.GetCustomAttributes(typeof(T), false);

            if (attributes.Length > 0)
            {
                attribute = attributes[0] as T;
            }
            else
            {
                attribute = null;
            }

            return attribute != null;
        }

        /// <summary>
        ///     Returns the version of the assembly, or <see langword="null"/>.
        /// </summary>
        /// <param name="assembly">The assembly whose version is being obtained.</param>
        /// <remarks>
        ///     The resolution order for the version is as follows:
        ///     <see cref="AssemblyInformationalVersionAttribute"/>,
        ///     <see cref="AssemblyFileVersionAttribute"/>,
        ///     then <see cref="AssemblyVersionAttribute"/>.
        /// </remarks>
        public static string GetVersion([NotNull] this Assembly assembly)
        {
            if (assembly.TryGetAttribute(out AssemblyInformationalVersionAttribute infoVersionAttr))
            {
                return infoVersionAttr.InformationalVersion;
            }

            if (assembly.TryGetAttribute(out AssemblyFileVersionAttribute fileVersionAttr))
            {
                return fileVersionAttr.Version;
            }

            return assembly.TryGetAttribute(out AssemblyVersionAttribute assemblyVersionAttr) ? assemblyVersionAttr.Version : null;
        }
    }
}
