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
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ToolkitUtils.Mod.Registries;

/// <summary>Represents a read-only registry that allows querying objects by their unique identifiers.</summary>
/// <typeparam name="T">The type of object stored in the registry, constrained to implement <see cref="IIdentifiable" />.</typeparam>
/// <remarks>
///     A read-only registry provides access to a fixed collection of objects. Modifications, such as adding or
///     removing objects, are not supported.
/// </remarks>
public interface IReadOnlyRegistry<T> where T : class, IIdentifiable
{
    /// <summary>Gets a read-only list of all registered items in the registry.</summary>
    /// <returns>A read-only list containing all the registered items.</returns>
    IReadOnlyList<T> AllRegistrants { get; }

    /// <summary>Retrieves an object by its unique identifier.</summary>
    /// <param name="id">The ID of the object to retrieve.</param>
    /// <returns>The registered object with the given ID, or <see langword="null" /> if no such object is found.</returns>
    T? GetById(string id);

    /// <summary>Retrieves an object by its name.</summary>
    /// <param name="name">The name of the object to receive.</param>
    /// <returns>The registered object with the given name, or <see langword="null" /> if no such object is found.</returns>
    T? GetByName(string name);

    /// <summary>Attempts to retrieve an object by its unique identifier.</summary>
    /// <param name="id">The ID of the object to retrieve.</param>
    /// <param name="obj">
    ///     When this method returns, contains the object associated with the specified ID, if found; otherwise,
    ///     <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if an object with the specified ID is found; otherwise, <see langword="false" />.</returns>
    bool TryGetById(string id, [NotNullWhen(true)] out T? obj);

    /// <summary>Attempts to retrieve an object by its name.</summary>
    /// <param name="name">The name of the object to retrieve.</param>
    /// <param name="obj">
    ///     When this method returns, contains the object associated with the specified name, if found;
    ///     otherwise, <see langword="null" />.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if an object with the specified name exists in the registry; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    bool TryGetByName(string name, [NotNullWhen(true)] out T? obj);
}
