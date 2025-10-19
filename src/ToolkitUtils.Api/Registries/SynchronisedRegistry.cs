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
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using ToolkitUtils.Mod;

namespace ToolkitUtils.Api;

/// <summary>
///     A registry implementation that uses <see cref="Interlocked.CompareExchange(ref object, object, object)" /> to
///     modify the registry's contents.
/// </summary>
/// <typeparam name="T">The type of the class being represented within the registry.</typeparam>
[PublicAPI]
public sealed class SynchronisedRegistry<T>(IReadOnlyList<T>? allRegistrants = null) : IRegistry<T> where T : class, IIdentifiable
{
    /// <inheritdoc />
    public IReadOnlyList<T> AllRegistrants { get; private set; } = allRegistrants ?? [];

    /// <inheritdoc />
    public void Register([System.Diagnostics.CodeAnalysis.NotNull] T obj)
    {
        IReadOnlyList<T> original, modified;

        do
        {
            original = AllRegistrants;
            var copy = new List<T>(original)
            {
                obj,
            };
            modified = copy;
        } while (Interlocked.CompareExchange(ref original, modified, original) != original);
    }

    /// <inheritdoc />
    public bool Unregister(T obj)
    {
        IReadOnlyList<T> original, modified;

        do
        {
            original = AllRegistrants;
            var copy = new List<T>(original);
            copy.Remove(obj);
            modified = copy;
        } while (Interlocked.CompareExchange(ref original, modified, original) != original);

        return true;
    }

    /// <inheritdoc />
    public T? Get(string id)
    {
        for (var i = 0; i < AllRegistrants.Count; i++)
        {
            T? registrant = AllRegistrants[i];

            if (string.Equals(id, registrant.Id, StringComparison.Ordinal)) return registrant;
        }

        return null;
    }

    /// <inheritdoc />
    public int Count => AllRegistrants.Count;

    public T? GetByName(string name)
    {
        for (var i = 0; i < AllRegistrants.Count; i++)
        {
            T? registrant = AllRegistrants[i];

            if (string.Equals(name, registrant.Name, StringComparison.Ordinal)) return registrant;
        }

        return null;
    }

    public void Deconstruct(out IReadOnlyList<T>? allRegistrants)
    {
        allRegistrants = AllRegistrants;
    }
}
