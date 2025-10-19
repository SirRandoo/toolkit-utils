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

namespace ToolkitUtils.Mod.Presentation;

/// <summary>Contains the various translation strings used by the library.</summary>
[PublicAPI]
[StaticConstructorOnStartup]
public static class UxLocale
{
    /// <summary>Contains the translation string for an "invalid url" error.</summary>
    [Translation("SirRandoo.UX.InvalidUrl")]
    public static readonly string InvalidUrl = null!;

    /// <summary>Contains the translation string for an "experimental content" notice.</summary>
    [Translation("SirRandoo.UX.ExperimentalNotice")]
    public static readonly string ExperimentalNotice = null!;

    /// <summary>Contains the translation string for an "invalid url" error, but colored redish-pinkish.</summary>
    [Translation(key: "SirRandoo.UX.InvalidUrl", UxColors.RedishPinkHex)]
    public static readonly string InvalidUrlColored = null!;

    /// <summary>Contains the translation string for an "experimental content" notice, but colored redish-pinkish.</summary>
    [Translation(key: "SirRandoo.UX.ExperimentalNotice", UxColors.RedishPinkHex)]
    public static readonly string ExperimentalNoticeColored = null!;

    [Translation("ModDependsOn")] public static readonly string ModDependsOn = null!;

    [Translation(key: "ModDependsOn", UxColors.RedishPinkHex)]
    public static readonly string ModDependsOnColored = null!;

    static UxLocale()
    {
        TranslationManager.Register(typeof(UxLocale));
    }
}
