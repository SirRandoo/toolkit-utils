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
using System.Diagnostics.CodeAnalysis;

namespace ToolkitUtils.Mod.Registries;

/// <summary>Represents a modifiable registry for storing and managing uniquely identifiable objects.</summary>
/// <typeparam name="T">The type of object stored in the registry, constrained to implement <see cref="IIdentifiable" />.</typeparam>
/// <remarks>
///     Registries act as centralized stores for managing collections of objects that can be identified by unique keys
///     (IDs). This interface allows adding, removing, and retrieving registered objects.
/// </remarks>
public interface IRegistry<T> : IReadOnlyRegistry<T> where T : class, IIdentifiable
{
    /// <summary>Registers an object in the registry.</summary>
    /// <param name="obj">The object to register.</param>
    /// <returns>
    ///     <see langword="true" /> if the object was successfully registered; otherwise, <see langword="false" /> if it
    ///     was already registered.
    /// </returns>
    bool Register([DisallowNull] T obj);

    /// <summary>Unregisters an object from the registry.</summary>
    /// <param name="obj">The object to remove.</param>
    /// <returns>
    ///     <see langword="true" /> if the object was successfully removed; otherwise, <see langword="false" /> if the
    ///     object was not found.
    /// </returns>
    bool Unregister([DisallowNull] T obj);
}
