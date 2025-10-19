// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ToolkitUtils.Api.Extensions;

/// <summary>A collection of extension methods for viewing information within assemblies.</summary>
/// <seealso cref="Assembly" />
public static class AssemblyExtensions
{
    /// <summary>Attempts to get the attribute provided.</summary>
    /// <param name="assembly">The assembly containing the attribute being retrieved.</param>
    /// <param name="attribute">The attribute type being retrieved from the assembly.</param>
    /// <typeparam name="T">The type of the attribute being retrieved.</typeparam>
    /// <returns>Whether the attribute was retrieved from the assembly metadata.</returns>
    public static bool TryGetAttribute<T>(this Assembly assembly, [NotNullWhen(returnValue: true)] out T? attribute) where T : Attribute
    {
        object[] attributes = assembly.GetCustomAttributes(typeof(T), inherit: false);

        if (attributes.Length > 0)
            attribute = attributes[0] as T;
        else
            attribute = null;

        return attribute != null;
    }

    /// <summary>Returns the version of the assembly, or <see langword="null" />.</summary>
    /// <param name="assembly">The assembly whose version is being obtained.</param>
    /// <remarks>
    ///     The resolution order for the version is as follows: <see cref="AssemblyInformationalVersionAttribute" />,
    ///     <see cref="AssemblyFileVersionAttribute" />, then <see cref="AssemblyVersionAttribute" />.
    /// </remarks>
    public static string GetVersion(this Assembly assembly)
    {
        if (assembly.TryGetAttribute(out AssemblyInformationalVersionAttribute? infoVersionAttr)) return infoVersionAttr.InformationalVersion;
        if (assembly.TryGetAttribute(out AssemblyFileVersionAttribute? fileVersionAttr)) return fileVersionAttr.Version;

        return assembly.TryGetAttribute(out AssemblyVersionAttribute? assemblyVersionAttr) ? assemblyVersionAttr.Version : string.Empty;
    }
}
