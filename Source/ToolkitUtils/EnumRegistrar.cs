// MIT License
//
// Copyright (c) 2024 SirRandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils;

/// <summary>
///     A class for indexing a given enum for use in enumeration and case
///     insensitive keyed access.
/// </summary>
/// <typeparam name="T">The enum to index</typeparam>
public class EnumRegistrar<T> : IReadOnlyCollection<T>, IReadOnlyDictionary<string, T> where T : Enum
{
    private readonly List<T> _entities = [];
    private readonly Dictionary<string, T> _entitiesKeyed = new();
    private readonly Dictionary<T, string> _entitiesValueKeyed = new();

    public EnumRegistrar()
    {
        foreach (string name in Enum.GetNames(typeof(T)))
        {
            var inst = (T)Enum.Parse(typeof(T), name);

            _entities.Add(inst);
            _entitiesKeyed.Add(name.ToLowerInvariant(), inst);
            _entitiesValueKeyed.Add(inst, name.ToLowerInvariant());
        }
    }

    public string this[T key] => _entitiesValueKeyed[key];

    /// <inheritdoc cref="IReadOnlyCollection{T}.GetEnumerator" />
    public IEnumerator<T> GetEnumerator() => _entities.GetEnumerator();

    /// <inheritdoc cref="IEnumerable.GetEnumerator" />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc cref="IReadOnlyCollection{T}.Count" />
    public int Count => _entities.Count;

    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.GetEnumerator" />
    IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator() => _entitiesKeyed.GetEnumerator();

    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.ContainsKey" />
    public bool ContainsKey(string key) => _entitiesKeyed.ContainsKey(key.ToLowerInvariant());

    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.TryGetValue" />
    public bool TryGetValue(string key, out T value) => _entitiesKeyed.TryGetValue(key.ToLowerInvariant(), out value);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.this" />
    public T this[string key] => _entitiesKeyed[key.ToLowerInvariant()];

    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.Keys" />
    public IEnumerable<string> Keys => _entitiesKeyed.Keys;

    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.Values" />
    public IEnumerable<T> Values => _entitiesKeyed.Values;

    [ContractAnnotation("=> true, key: notnull; => false, key: null")]
    public bool TryGetKey(T value, out string key) => _entitiesValueKeyed.TryGetValue(value, out key);
}
