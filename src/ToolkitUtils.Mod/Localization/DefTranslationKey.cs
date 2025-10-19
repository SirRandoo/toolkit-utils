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
using NetEscapades.EnumGenerators;

namespace ToolkitUtils.Mod.Localization;

/// <summary>Defines keys used for localization in the application.</summary>
/// <remarks>
///     The <c>DefTranslationKey</c> enumeration provides a set of predefined keys that are used to reference
///     localized strings within the application. These keys are instrumental in enabling consistent and maintainable
///     localization by serving as identifiers for different elements that require translation, such as labels and
///     descriptions.
/// </remarks>
[EnumExtensions]
public enum DefTranslationKey
{
    /// <summary>Represents an unknown key for localization purposes.</summary>
    /// <remarks>
    ///     The <c>Unknown</c> member serves as a default case within the <see cref="DefTranslationKey" /> enumeration,
    ///     mainly used to handle unexpected or unrecognized keys. It ensures robust processing of localization data when
    ///     encountering keys that do not match predefined values.
    /// </remarks>
    Unknown,

    /// <summary>Represents the key for accessing localized label strings within the application.</summary>
    /// <remarks>
    ///     The Label key is used within the localization system to identify and retrieve the appropriate translations for
    ///     labels associated with various user interface elements. These labels are critical for ensuring that text visible to
    ///     users is presented in their selected language, enhancing overall user experience and accessibility.
    /// </remarks>
    Label,

    /// <summary>Represents a description key for localization purposes.</summary>
    /// <remarks>
    ///     The <c>Description</c> member is used to identify the description field within a localization process. It
    ///     serves as a key in the <see cref="DefTranslationKey" /> enumeration, allowing for the retrieval and processing of
    ///     descriptive text associated with localization data.
    /// </remarks>
    Description,
}
