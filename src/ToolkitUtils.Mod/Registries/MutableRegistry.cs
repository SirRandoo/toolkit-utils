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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace ToolkitUtils.Mod.Registries;

/// <summary>
///     A simple mutable registry for managing unique objects that implement the <see cref="IIdentifiable" />
///     interface. This class allows for adding, removing, and retrieving objects using their unique identifiers.
/// </summary>
/// <typeparam name="T">The type of objects stored in the registry. Must implement <see cref="IIdentifiable" />.</typeparam>
[PublicAPI]
public class MutableRegistry<T> : IRegistry<T> where T : class, IIdentifiable
{
    private Dictionary<string, T> _allRegistrantsKeyed = null!;
    private IReadOnlyList<T>? _cachedRegistrantView;

    private MutableRegistry() {}

    /// <summary>
    ///     Gets the current list of all registered objects as a read-only view. The list is cached for performance and
    ///     will be updated if any modifications occur.
    /// </summary>
    public IReadOnlyList<T> AllRegistrants
    {
        get { return _cachedRegistrantView ??= _allRegistrantsKeyed.Values.ToList(); }
    }

    /// <summary>
    ///     Registers a new object in the registry. If an object with the same identifier already exists, the registration
    ///     will fail.
    /// </summary>
    /// <param name="obj">The object to register.</param>
    /// <returns><see langword="true" /> if the object was successfully registered; otherwise, <see langword="false" />.</returns>
    public bool Register([DisallowNull] T obj)
    {
        if (!_allRegistrantsKeyed.TryAdd(obj.Id, obj)) return false;

        _cachedRegistrantView = null;

        return true;
    }

    /// <summary>
    ///     Unregisters an existing object from the registry. If the object does not exist in the registry, the operation
    ///     will fail.
    /// </summary>
    /// <param name="obj">The object to unregister.</param>
    /// <returns><see langword="true" /> if the object was successfully unregistered; otherwise, <see langword="false" />.</returns>
    public bool Unregister([DisallowNull] T obj)
    {
        if (!_allRegistrantsKeyed.Remove(obj.Id)) return false;

        _cachedRegistrantView = null;

        return true;
    }

    /// <summary>
    ///     Retrieves an object from the registry using its unique identifier. Returns <see langword="null" /> if the
    ///     object does not exist.
    /// </summary>
    /// <param name="id">The unique identifier of the object to retrieve.</param>
    /// <returns>The object associated with the specified identifier, or <see langword="null" /> if not found.</returns>
    public T? GetById(string id) => _allRegistrantsKeyed.GetValueOrDefault(id);

    /// <inheritdoc />
    public T? GetByName(string name)
    {
        for (var i = 0; i < AllRegistrants.Count; i++)
            if (string.Equals(AllRegistrants[i].Name, name, StringComparison.CurrentCultureIgnoreCase))
                return AllRegistrants[i];

        return null;
    }

    /// <inheritdoc />
    public bool TryGetById(string id, [NotNullWhen(true)] out T? obj)
    {
        obj = GetById(id);

        return obj != null;
    }

    /// <inheritdoc />
    public bool TryGetByName(string name, [NotNullWhen(true)] out T? obj)
    {
        obj = GetByName(name);

        return obj != null;
    }

    /// <summary>
    ///     Creates an instance of <see cref="MutableRegistry{T}" /> and populates it with initial registrants. This
    ///     method checks for duplicate identifiers in the initial list and throws an exception if found.
    /// </summary>
    /// <param name="registrants">The initial list of objects to register.</param>
    /// <returns>A new instance of <see cref="MutableRegistry{T}" /> populated with the provided registrants.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a duplicate id is detected in the initial registrants.</exception>
    public static MutableRegistry<T> CreateInstance(IReadOnlyList<T> registrants)
    {
        var registrantsKeyed = new Dictionary<string, T>();

        for (var i = 0; i < registrants.Count; i++)
        {
            T registrant = registrants[i];

            if (!registrantsKeyed.TryAdd(registrant.Id, registrant)) throw new InvalidOperationException($"An entry with the id '{registrant.Id}' is already registered.");
        }

        return new MutableRegistry<T>
        {
            _allRegistrantsKeyed = registrantsKeyed,
        };
    }
}
