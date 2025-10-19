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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using ToolkitUtils.Mod;

namespace ToolkitUtils.Api;

/// <summary>
///     An abstract definition of a "registry", an object designated for centralized storing and retrieving unique
///     objects.
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public interface IRegistry<T> where T : class, IIdentifiable
{
    /// <summary>Returns the current objects registered within the registry.</summary>
    IReadOnlyList<T> AllRegistrants { get; }

    /// <summary>Returns the number of registrants within the registry.</summary>
    int Count { get; }

    /// <summary>Registers an object to the registry.</summary>
    /// <param name="obj">The object to register</param>
    void Register([DisallowNull] T obj);

    /// <summary>Unregisters an object from the registry.</summary>
    /// <param name="obj">The object to unregister.</param>
    /// <returns>Whether the object was unregistered.</returns>
    bool Unregister(T obj);

    /// <summary>Gets the object within the registry by the id, or <see langword="null" /> if the object doesn't exist.</summary>
    /// <param name="id">The id of the object being obtained.</param>
    /// <returns>The object registered within the registry, or the <see langword="null" /> if the object doesn't exist.</returns>
    T? Get(string id);
}
