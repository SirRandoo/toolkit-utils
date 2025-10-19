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
using Verse;

namespace ToolkitUtils.Mod.Localization;

/// <summary>
///     Provides extension methods for the <see cref="TranslationService" /> interface to enhance functionality
///     specific to game translations, such as retrieving translations based on definition names and keys.
/// </summary>
[PublicAPI]
public static class TranslationServiceExtensions
{
    /// <summary>
    ///     Retrieves the translation for a specific definition name and translation key, using the active language of the
    ///     game.
    /// </summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="defName">The name of the definition for which the translation is being retrieved.</param>
    /// <param name="key">The translation key representing the desired part of the definition, such as label or description.</param>
    /// <returns>The translated string if found; otherwise, returns null.</returns>
    public static Translation GetTranslation(this TranslationService service, string defName, DefTranslationKey key) =>
        service.GetTranslation(defName, key, new Language(LanguageDatabase.activeLanguage.FriendlyNameEnglish));
}
