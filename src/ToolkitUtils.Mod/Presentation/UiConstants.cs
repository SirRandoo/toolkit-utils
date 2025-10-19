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

namespace ToolkitUtils.Mod.Presentation;

/// <summary>A set of constants that are used throughout the mod's menus.</summary>
public static class UiConstants
{
    /// <summary>The line height of all content within the mod's menus.</summary>
    /// <remarks>
    ///     The mod intentionally uses a slightly higher line height than RimWorld's to create less visually dense menus
    ///     as they will often contain a lot of information that needs to be digested.
    /// </remarks>
    public const float LineHeight = 28f;

    /// <summary>The line height of all tabs within the mod's menus.</summary>
    public const float TabHeight = 35f;

    /// <summary>The halved <see cref="Text.SmallFontHeight" />.</summary>
    public const float HalvedSmallLineHeight = Text.SmallFontHeight * 0.5f;
}
