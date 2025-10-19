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

namespace ToolkitUtils.Api.Extensions;

/// <summary>A collection of extension methods for collection types.</summary>
public static class CollectionExtensions
{
    /// <summary>Attempts to get an integer from the array at the given index.</summary>
    /// <param name="array">The array to index.</param>
    /// <param name="index">The index to retrieve from the array.</param>
    /// <param name="value">
    ///     The parsed value that was retrieved from the array, or <see langword="default" /> if the value
    ///     doesn't exist in the array, or if the array isn't large enough to have the specified index.
    /// </param>
    /// <returns>Whether a valid integer was retrieved from the array at the given index.</returns>
    public static bool TryGetIntAt(this string[] array, int index, out int value)
    {
        if (array.TryGetAt(index, out string? valueAt)) return int.TryParse(valueAt, out value);

        value = 0;

        return false;
    }

    /// <summary>Attempts to get a value from the array at the given index.</summary>
    /// <param name="array">The array to index.</param>
    /// <param name="index">The index to retrieve from the array.</param>
    /// <param name="value">
    ///     The value retrieved from the array, or <see langword="default" /> if the value doesn't exist in the
    ///     array, or if the array isn't large enough to have the specified index.
    /// </param>
    /// <typeparam name="T">The type of value stored within the array.</typeparam>
    /// <returns>Whether a non-null value was retrieved from the array at the given index.</returns>
    public static bool TryGetAt<T>(this T[] array, int index, [NotNullWhen(returnValue: true)] out T? value)
    {
        value = index < array.Length ? array[index] : default(T);

        return value is not null;
    }

    /// <summary>Attempts to get a value from the list at the given index.</summary>
    /// <param name="list">The list to index.</param>
    /// <param name="index">The index to retrieve from the list.</param>
    /// <param name="value">
    ///     The value retrieved from the list, or <see langword="default" /> if the value doesn't exist in the
    ///     list, or if the list isn't large enough to have the specified index.
    /// </param>
    /// <typeparam name="T">The type of value stored within the list.</typeparam>
    /// <returns>Whether a non-null value was retrieved from the list at the given index.</returns>
    public static bool TryGetAt<T>(this IReadOnlyList<T> list, int index, [NotNullWhen(returnValue: true)] out T? value)
    {
        value = index < list.Count ? list[index] : default(T);

        return value is not null;
    }
}
