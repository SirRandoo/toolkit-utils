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
using JetBrains.Annotations;

namespace ToolkitUtils.Mod.Extensions;

/// <summary>Provides helper methods and extensions for processing and transforming input strings.</summary>
[PublicAPI]
public static class StringExtensions
{
    /// <summary>
    ///     Converts the source string to a standardized "Toolkit" format by removing spaces and converting all characters
    ///     to lowercase.
    /// </summary>
    /// <param name="source">The input string to be transformed.</param>
    /// <returns>A new string with all spaces removed and all characters in lowercase.</returns>
    public static string ToToolkit(this string source)
    {
        ReadOnlySpan<char> span = source.AsSpan();

        int finalLength = source.Length;

        for (var i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case ' ': finalLength--; break;
            }
        }

        var index = 0;
        Span<char> copy = stackalloc char[finalLength];

        for (var i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case ' ': continue;
                default:  copy[index++] = char.ToLower(span[i]); break;
            }
        }

        return new string(copy.ToArray());
    }
}
