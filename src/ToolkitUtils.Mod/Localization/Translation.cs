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
using Verse;

namespace ToolkitUtils.Mod.Localization;

/// <summary>Represents a translation key used to retrieve localized translations from a translation service.</summary>
/// <remarks>
///     Instances of this struct encapsulate a key value, which can be implicitly converted to a string representation
///     through a translation service. The key is intended to be used to fetch translations dynamically at runtime through
///     the registered ITranslationService.
/// </remarks>
/// <example>
///     A <see cref="Translation" /> can be implicitly converted to a string by resolving the corresponding
///     translation using the registered <see cref="TranslationService" />. Additionally, strings can be implicitly
///     converted into <see cref="Translation" /> instances.
/// </example>
public readonly record struct Translation(string Value)
{
    /// <summary>Represents an empty or uninitialized <see cref="Translation" /> value.</summary>
    /// <remarks>
    ///     This is a predefined instance of the <see cref="Translation" /> struct, initialized with an empty string. It
    ///     can be used as a default value or placeholder for translation keys that do not represent meaningful or valid data.
    /// </remarks>
    public static readonly Translation None = new(string.Empty);

    /// <inheritdoc />
    public override string ToString() => Value;

    public static implicit operator string(Translation translation) => translation.Value;
    public static implicit operator Translation(string value) => new(value);
    public static implicit operator TaggedString(Translation translation) => new(translation.Value);
    public static implicit operator Translation(TaggedString value) => new(value.RawText);
}
