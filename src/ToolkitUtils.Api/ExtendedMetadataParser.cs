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
using System.Linq;
using JetBrains.Annotations;
using ToolkitUtils.Mod;

namespace ToolkitUtils.Api;

/// <summary>
///     An implementation for parsing ToolkitUtils' "extended metadata" syntax viewers use when purchasing products
///     from the store.
/// </summary>
[PublicAPI]
public sealed class ExtendedMetadataParser
{
    private readonly string _query;
    private HashSet<string> _metadataRaw = [];
    private string? _subjectRaw;

    private ExtendedMetadataParser(string input)
    {
        _query = input;
    }

    /// <summary>Parses the input string and returns an instance of <see cref="ExtendedMetadataParser" />.</summary>
    /// <param name="input">The input string to parse as extended metadata.</param>
    /// <returns>An instance of <see cref="ExtendedMetadataParser" /> initialized with the parsed input.</returns>
    public static ExtendedMetadataParser Parse(string input)
    {
        var parser = new ExtendedMetadataParser(input);
        parser.ParseInternal();

        return parser;
    }

    /// <summary>Retrieves a subject from a list of possible values that matches an internal subject name string.</summary>
    /// <typeparam name="T">The type of the objects in the possibleValues list, which must implement IIdentifiable.</typeparam>
    /// <param name="possibleValues">A read-only list of possible values from which to find the subject.</param>
    /// <returns>The matching object from the list if found, otherwise returns the default value of type T.</returns>
    public T? GetSubject<T>(IReadOnlyList<T> possibleValues) where T : IIdentifiable
    {
        for (var i = 0; i < possibleValues.Count; i++)
        {
            T value = possibleValues[i];

            if (string.Equals(value.Name, _subjectRaw, StringComparison.OrdinalIgnoreCase)) return value;
        }

        return default(T?);
    }

    /// <summary>Retrieves metadata from a list of possible values that matches the criteria specified in the collection.</summary>
    /// <param name="possibleValues">A list of potential values from which metadata will be obtained.</param>
    /// <typeparam name="T">
    ///     The type of the elements in <paramref name="possibleValues" /> that implement
    ///     <see cref="IIdentifiable" />.
    /// </typeparam>
    /// <returns>
    ///     An element of type <typeparamref name="T" /> that matches the metadata criteria, or the default value of
    ///     <typeparamref name="T" /> if no match is found.
    /// </returns>
    public T? GetMetadata<T>(IReadOnlyList<T> possibleValues) where T : IIdentifiable
    {
        for (var i = 0; i < possibleValues.Count; i++)
        {
            T obj = possibleValues[i];

            if (_metadataRaw.Contains(obj.Name.ToUpperInvariant())) return obj;
        }

        return default(T?);
    }

    /// <summary>Retrieves metadata information from a list of possible values based on a given criteria.</summary>
    /// <typeparam name="T">
    ///     The type of elements in the possible values list, which must implement <see cref="IIdentifiable" />
    ///     .
    /// </typeparam>
    /// <param name="possibleValues">A list of potential values from which metadata might be retrieved.</param>
    /// <param name="nameGetter">A function to extract the name or identifier from each element in the list.</param>
    /// <param name="defaultGetter">
    ///     An optional function to provide a default value if no metadata is found. If not provided,
    ///     the default value of the type T will be returned.
    /// </param>
    /// <returns>
    ///     The metadata of type T if found; otherwise, the result from <paramref name="defaultGetter" /> if provided, or
    ///     the default value of type T.
    /// </returns>
    public T? GetMetadata<T>(IReadOnlyList<T?> possibleValues, Func<T?, string> nameGetter, Func<T?>? defaultGetter = null)
    {
        for (var i = 0; i < possibleValues.Count; i++)
        {
            T? obj = possibleValues[i];
            string name = nameGetter(obj);

            if (_metadataRaw.Contains(name.ToUpperInvariant())) return obj;
        }

        return defaultGetter is null ? default(T?) : defaultGetter();
    }

    private void ParseInternal()
    {
        ReadOnlySpan<char> span = _query.AsSpan();
        var startIndex = 0;
        var endIndex = 0;
        var inMetadata = false;
        var metadataContainer = new List<string>();

        for (var i = 0; i < span.Length; i++)
        {
            ReadOnlySpan<char> slice;

            switch (span[i])
            {
                case '[':
                    _subjectRaw = GetTrimmed(span.Slice(startIndex, endIndex - startIndex));
                    startIndex = i + 1;
                    endIndex = startIndex;
                    inMetadata = true;

                    break;
                case ']':
                    inMetadata = false;
                    slice = span.Slice(startIndex, endIndex - startIndex);
                    metadataContainer.Add(GetTrimmed(slice));
                    _metadataRaw = metadataContainer.ToHashSet(StringComparer.OrdinalIgnoreCase);

                    break;
                case ',' when inMetadata:
                    slice = span.Slice(startIndex, endIndex - startIndex);
                    startIndex = i + 1;
                    endIndex = startIndex;

                    metadataContainer.Add(GetTrimmed(slice));

                    break;
                default: endIndex++; break;
            }
        }

        // Handles the case where if no metadata was specified, subject
        // would be nil.
        _subjectRaw ??= GetTrimmed(span.Slice(startIndex, endIndex - 1));
    }

    private static unsafe string GetTrimmed(ReadOnlySpan<char> span)
    {
        // A naive implementation that only removes the first space
        // from the start and end of a given string span. For the
        // use case, there will never be more than one space at either
        // end of the string since Twitch removes whitespace if it occurs
        // more than once one after the other.

        if (span.Length <= 0) return string.Empty;

        var start = 0;

        if (span[0] == ' ') start++;

        ReadOnlySpan<char> sliced = span[^1] == ' ' ? span.Slice(start, span.Length - 2) : span[start..];

        fixed (char* pointer = &sliced.GetPinnableReference()) { return new string(pointer, startIndex: 0, sliced.Length); }
    }
}
