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
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using System;

namespace ToolkitUtils.Api;

/// <summary>A static class dedicated to translating text into other languages supported by the mod, and RimWorld.</summary>
public static class Localizer
{
    /// <summary>Marks text as untranslated.</summary>
    /// <param name="text">The text that isn't translated.</param>
    /// <remarks>
    ///     This method is intended to be used by developers to mark strings as untranslated. This method alone does
    ///     nothing if a developer's workflow doesn't track obsolete methods.
    /// </remarks>
    [Obsolete("Callers should translate the string passed prior to release.")]
    public static string MarkNotTranslated(this string text) => text;
}
