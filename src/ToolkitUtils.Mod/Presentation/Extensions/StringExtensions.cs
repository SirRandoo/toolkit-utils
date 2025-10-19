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
using JetBrains.Annotations;

namespace ToolkitUtils.Mod.Presentation.Extensions;

/// <summary>A collection of extension methods for strings.</summary>
[PublicAPI]
public static class StringExtensions
{
    /// <summary>
    ///     An alternative to <see cref="Gen.ToStringSafe{T}" /> that returns <see cref="string.Empty" /> instead of
    ///     "null".
    /// </summary>
    /// <param name="instance">An instance of an object that's being turned into a string.</param>
    /// <typeparam name="T">The type of the object being turned into a string.</typeparam>
    /// <returns><see cref="string.Empty" /> if the object was <see langword="null" />, or the object's string representation.</returns>
    public static string ToStringNullable<T>(this T? instance) => Equals(instance, default(T)) ? string.Empty : instance!.ToString();
}
