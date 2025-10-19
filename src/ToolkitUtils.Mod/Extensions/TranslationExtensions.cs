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
using System.Collections.Generic;
using FormatWith;
using JetBrains.Annotations;
using ToolkitUtils.Mod.Localization;
using Verse;

namespace ToolkitUtils.Mod.Extensions;

/// <summary>Provides extension methods to facilitate interaction with translation services.</summary>
[PublicAPI]
public static class TranslationExtensions
{
    /// <summary>Retrieves a translation for the specified key or constructs a default translation if none is found.</summary>
    /// <param name="key">The key used to identify the translation.</param>
    /// <returns>A <see cref="Translation" /> object corresponding to the provided key.</returns>
    /// <remarks>This method should be used in contexts that can't be injected a <see cref="TranslationService" /> instance.</remarks>
    public static Translation FromKey(string key) => TranslationService.Instance.GetTranslation(key);

    /// <summary>Capitalizes the first character of the translation value.</summary>
    /// <param name="translation">The translation whose value will be modified.</param>
    /// <returns>A new <see cref="Translation" /> object with the first character capitalized.</returns>
    public static Translation CapitalizeFirst(this Translation translation) => new(translation.Value.CapitalizeFirst());

    /// <summary>Formats the translation value with the specified arguments.</summary>
    /// <param name="translation">The translation object whose value will be formatted.</param>
    /// <param name="args">An array of objects to format the translation value with.</param>
    /// <returns>A new <see cref="Translation" /> object containing the formatted translation value.</returns>
    public static Translation Format(this Translation translation, params object[] args) => new(string.Format(translation.Value, args));

    /// <summary>
    ///     Formats the translation string by replacing placeholders with corresponding values from the provided
    ///     dictionary.
    /// </summary>
    /// <param name="translation">The translation whose value will be formatted.</param>
    /// <param name="args">A dictionary containing keys and respective values for placeholders in the translation string.</param>
    /// <returns>A new <see cref="Translation" /> object containing the formatted string.</returns>
    public static Translation Format(this Translation translation, Dictionary<string, object> args) => new(translation.Value.FormatWith(args, MissingKeyBehaviour.Ignore));
}
