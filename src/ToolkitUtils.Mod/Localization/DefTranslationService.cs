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
using System.Collections.Concurrent;
using Verse;

namespace ToolkitUtils.Mod.Localization;

/// <summary>
///     The DefTranslationService class is responsible for managing and retrieving translations for definitions (defs)
///     based on provided keys and languages.
/// </summary>
public sealed class DefTranslationService
{
    private readonly ConcurrentDictionary<string, DefInjectedTranslation> _defTranslations = new();

    /// <summary>Retrieves the translation for a given definition name and key.</summary>
    /// <param name="defName">The name of the definition for which the translation is requested.</param>
    /// <param name="key">The specific key indicating which part of the definition should be translated.</param>
    /// <returns>A string containing the translation if available; otherwise, null.</returns>
    public Translation GetTranslation(string defName, DefTranslationKey key)
    {
        if (!_defTranslations.TryGetValue(defName, out DefInjectedTranslation? value)) return Translation.None;

        return key switch
        {
            DefTranslationKey.Label       => new Translation(value.Label),
            DefTranslationKey.Description => new Translation(value.Description),
            var _                         => Translation.None,
        };
    }

    /// <summary>Caches the translations for the provided language.</summary>
    /// <param name="language">The language whose translations are to be cached.</param>
    public void CacheTranslations(LoadedLanguage language)
    {
        foreach (DefInjectionPackage injection in language.defInjections) ProcessDefInjection(injection);
    }

    private void ProcessDefInjection(DefInjectionPackage injection)
    {
        foreach ((string key, DefInjectionPackage.DefInjection defInjection) in injection.injections)
        {
            string[] segments = key.Split('.');

            if (segments.Length > 2)
            {
                // We'll skip injections that inject deeper into the XML def tree.

                continue;
            }

            (string defName, string defField) = (segments[0], segments[1]);

            if (!_defTranslations.TryGetValue(defName, out DefInjectedTranslation? container))
                _defTranslations[defName] = container = new DefInjectedTranslation(string.Empty, string.Empty);

            DefTranslationKey translationKey = DefTranslationKeyExtensions.TryParse(defField, out DefTranslationKey value, ignoreCase: true) ? value : DefTranslationKey.Unknown;

            _defTranslations[defName] = translationKey switch
            {
                DefTranslationKey.Label => container with
                {
                    Label = defInjection.injection,
                },
                DefTranslationKey.Description => container with
                {
                    Description = defInjection.injection,
                },
                var _ => _defTranslations[defName],
            };
        }
    }

    private sealed record DefInjectedTranslation(string Label, string Description);
}
