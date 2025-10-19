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
using System.Data;
using System.Diagnostics.CodeAnalysis;
using ToolkitUtils.Mod;

namespace ToolkitUtils.Api;

/// <summary>A registry implementation that restricts modifications after the registry is instantiated.</summary>
/// <param name="AllRegistrants"></param>
/// <typeparam name="T"></typeparam>
public record FrozenRegistry<T>(IReadOnlyList<T> AllRegistrants) : IRegistry<T> where T : class, IIdentifiable
{
    private readonly IReadOnlyList<T> _allRegistrants = AllRegistrants;
    private readonly IReadOnlyDictionary<string, T> _registrantsKeyed = new Dictionary<string, T>();

    /// <inheritdoc />
    public IReadOnlyList<T> AllRegistrants
    {
        get => _allRegistrants;
        init
        {
            _allRegistrants = value;

            var container = new Dictionary<string, T>();

            for (var index = 0; index < _allRegistrants.Count; index++)
            {
                T obj = _allRegistrants[index];

                container[obj.Id] = obj;
            }

            _registrantsKeyed = container;
        }
    }

    /// <inheritdoc />
    public void Register([NotNull] T obj)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc />
    public bool Unregister(T obj) => throw new ReadOnlyException();

    /// <inheritdoc />
    public T? Get(string id) => _registrantsKeyed.GetValueOrDefault(id);

    /// <inheritdoc />
    public int Count => _allRegistrants.Count;
}
