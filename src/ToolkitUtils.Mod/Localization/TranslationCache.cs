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
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace ToolkitUtils.Mod.Localization;

/// <summary>Provides a thread-safe cache for storing translations with expiration capabilities.</summary>
public sealed class TranslationCache
{
    private readonly ConcurrentDictionary<int, CachedTranslation> _cachedTranslations = new();

    [SuppressMessage(category: "ReSharper", checkId: "NotAccessedField.Local")]
    private readonly Timer _timer;

    public TranslationCache()
    {
        _timer = new Timer(CleanupExpiredEntries, state: null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    /// <summary>Retrieves a cached translation associated with the provided hash code if it exists and has not expired.</summary>
    /// <param name="hashCode">The hash code associated with the desired cached translation.</param>
    /// <returns>The cached translation if it exists and has not expired; otherwise, null.</returns>
    public string? GetCachedTranslation(int hashCode)
    {
        if (!_cachedTranslations.TryGetValue(hashCode, out CachedTranslation? translation)) return null;

        DateTimeOffset now = DateTimeOffset.UtcNow;

        if (now <= translation.ExpiresOn) return translation.Translation;

        _cachedTranslations.TryRemove(hashCode, out CachedTranslation _);

        return null;
    }

    /// <summary>Adds a new translation to the cache or updates an existing one with a specified expiration duration.</summary>
    /// <param name="hashCode">The hash code that uniquely identifies the translation.</param>
    /// <param name="translation">The translation text to store in the cache.</param>
    /// <param name="expirationDuration">The duration after which the translation expires and will be removed from the cache.</param>
    public void AddOrUpdateTranslation(int hashCode, string translation, TimeSpan expirationDuration)
    {
        DateTimeOffset expiresOn = DateTimeOffset.UtcNow.Add(expirationDuration);
        _cachedTranslations[hashCode] = new CachedTranslation(expiresOn, translation);
    }

    private void CleanupExpiredEntries(object state)
    {
        foreach (int key in _cachedTranslations.Keys)
        {
            if (!_cachedTranslations.TryGetValue(key, out CachedTranslation? translation)) continue;
            if (translation.ExpiresOn <= DateTimeOffset.UtcNow) _cachedTranslations.TryRemove(key, out CachedTranslation _);
        }
    }

    private sealed record CachedTranslation(DateTimeOffset ExpiresOn, string Translation);
}
