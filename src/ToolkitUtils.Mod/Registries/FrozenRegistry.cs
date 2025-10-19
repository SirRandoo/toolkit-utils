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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace ToolkitUtils.Mod.Registries;

/// <summary>
///     Provides an immutable registry for identifiable objects of type <typeparamref name="T" />. This registry
///     ensures that once initialized, no further modifications can be made.
/// </summary>
/// <typeparam name="T">The type of objects stored in the registry. Must implement <see cref="IIdentifiable" />.</typeparam>
/// <remarks>
///     The <see cref="FrozenRegistry{T}" /> is ideal for scenarios where read-only access to a predefined collection
///     of objects is required, such as configuration data or static game assets. It ensures thread-safety by making the
///     underlying collections immutable.
/// </remarks>
[PublicAPI]
public sealed class FrozenRegistry<T> : IReadOnlyRegistry<T> where T : class, IIdentifiable
{
    private IReadOnlyDictionary<string, T> _registrantsKeyed = null!;

    private FrozenRegistry() {}

    /// <inheritdoc />
    public IReadOnlyList<T> AllRegistrants { get; private set; } = null!;

    /// <inheritdoc />
    public T? GetById(string id) => _registrantsKeyed.GetValueOrDefault(id);

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

    /// <summary>Creates a new instance of a <see cref="FrozenRegistry{T}" /> populated with the provided registrants.</summary>
    /// <param name="registrants">The list of objects to register within the registry.</param>
    /// <returns>A fully initialized <see cref="FrozenRegistry{T}" /> containing all the provided registrants.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="registrants" /> argument is
    ///     <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="registrants" /> list is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a duplicate ID is found among the registrants.</exception>
    /// <remarks>
    ///     This method ensures that the registry is immutable by using <see cref="IReadOnlyList{T}" /> and
    ///     <see cref="IReadOnlyDictionary{TKey,TValue}" />. If the same ID appears more than once in the provided list, an
    ///     <see cref="InvalidOperationException" /> will be thrown, as each registrant must have a unique identifier.
    /// </remarks>
    [PublicAPI]
    public static FrozenRegistry<T> CreateInstance(IList<T> registrants)
    {
        if (registrants == null) throw new ArgumentNullException(nameof(registrants));
        if (registrants.Count == 0) throw new ArgumentException(message: "Registry cannot be empty.", nameof(registrants));

        var keyedRegistrants = new Dictionary<string, T>();
        var instance = new FrozenRegistry<T>
        {
            AllRegistrants = registrants.ToList().AsReadOnly(), _registrantsKeyed = new ReadOnlyDictionary<string, T>(registrants.ToDictionary(r => r.Id)),
        };

        for (var i = 0; i < registrants.Count; i++)
        {
            T registrant = registrants[i];

            if (!keyedRegistrants.TryAdd(registrant.Id, registrant)) throw new InvalidOperationException($"An entry with the id '{registrant.Id} is already registered.");
        }

        return instance;
    }
}
