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
using System.Collections.Generic;
using JetBrains.Annotations;
using NLog;
using ToolkitUtils.Mod.Logging;
using ToolkitUtils.Mod.Messaging;
using Verse;

namespace ToolkitUtils.Mod.Localization;

/// <summary>
///     A concrete implementation of the <see cref="TranslationService" /> interface. This service is responsible for
///     managing and retrieving translations for specified keys. It also supports caching and invalidates caches upon
///     receiving relevant events.
/// </summary>
[PublicAPI]
public sealed class TranslationService : IDisposable
{
    private static readonly Lazy<TranslationService> InstanceField = new(() => new TranslationService(GlobalEventSystem.Instance));
    private readonly ConcurrentDictionary<Language, DefTranslationService> _defTranslationServices = new();
    private readonly GlobalEventSystem _eventSystem;
    private readonly Logger _logger = UtilsLogFactory.Instance.GetCurrentClassLogger();
    private readonly ConcurrentDictionary<string, string> _translationCache = new();

    private TranslationService(GlobalEventSystem eventSystem)
    {
        _eventSystem = eventSystem;
        _eventSystem.Subscribe<InvalidateTranslationCacheEventArgs>(RecacheTranslations);
    }

    /// <summary>
    ///     Provides the singleton instance of the <see cref="TranslationService" />. This instance is used to access
    ///     functionalities for managing and retrieving translations, including cache handling and event-based cache
    ///     invalidation.
    /// </summary>
    public static TranslationService Instance => InstanceField.Value;

    /// <inheritdoc />
    public void Dispose()
    {
        _eventSystem.Unsubscribe<InvalidateTranslationCacheEventArgs>(RecacheTranslations);
    }

    /// <summary>Retrieves the localized translation for the specified key.</summary>
    /// <param name="key">The key representing a specific translation entry.</param>
    /// <returns>
    ///     A <see cref="Translation" /> containing the localized value associated with the provided key, or the key
    ///     itself if no translation is found.
    /// </returns>
    public Translation GetTranslation(string key) => new(_translationCache.GetValueOrDefault(key, key));

    /// <summary>Retrieves the translation associated with the specified definition name, translation key, and language.</summary>
    /// <param name="defName">The name of the definition for which the translation is being retrieved.</param>
    /// <param name="key">The translation key specifying the type of translation (e.g., label, description).</param>
    /// <param name="language">The language for which the translation is requested.</param>
    /// <returns>
    ///     An instance of <see cref="Translation" /> representing the desired translation. If no matching translation is
    ///     found, returns a default empty translation.
    /// </returns>
    public Translation GetTranslation(string defName, DefTranslationKey key, Language language) =>
        _defTranslationServices.TryGetValue(language, out DefTranslationService? service) ? service.GetTranslation(defName, key) : Translation.None;

    private void RecacheTranslations(InvalidateTranslationCacheEventArgs invalidateTranslationCacheEventArgs)
    {
        if (LanguageDatabase.activeLanguage == null)
        {
            _logger.Warn("Active language was set to `null`; aborting translation recache...");

            return;
        }

        _translationCache.Clear();

        foreach (LoadedLanguage language in LanguageDatabase.AllLoadedLanguages) CacheDefTranslations(language);
    }

    private void CacheDefTranslations(LoadedLanguage language)
    {
        if (_defTranslationServices.ContainsKey(language.info.friendlyNameEnglish))
        {
            // We'll skip caching the language's injections if we already cached
            // them since they should never change.

            return;
        }

        var service = new DefTranslationService();
        service.CacheTranslations(language);

        _defTranslationServices[language.info.friendlyNameEnglish] = service;
    }
}
