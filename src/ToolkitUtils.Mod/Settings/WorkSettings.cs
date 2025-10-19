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
using JetBrains.Annotations;
using Newtonsoft.Json;
using Verse;

namespace ToolkitUtils.Mod.Domain.Settings;

/// <summary>
///     Represents the settings configuration for work-related functionalities. Defines properties for managing
///     entries, sorting, filtering, and tracking versioning.
/// </summary>
[PublicAPI]
public sealed record WorkSettings(
    [property: JsonProperty("work_settings_entries")] IReadOnlyList<WorkSettingsEntry> WorkSettingsEntries,
    [property: JsonProperty("should_sort_types")] bool ShouldSortTypes = true,
    [property: JsonProperty("should_filter_types")] bool ShouldFilterTypes = true,
    [property: JsonProperty("version")] byte Version = 1
);

/// <summary>
///     Defines an interface for a work settings entry, providing properties to specify the associated work type,
///     visibility, modifiability, and versioning behavior within the application's configuration system.
/// </summary>
[PublicAPI]
public sealed record WorkSettingsEntry(
    [property: JsonProperty("def")] WorkTypeDef Def,
    [property: JsonProperty("is_visible")] bool IsVisible,
    [property: JsonProperty("is_modifiable")] bool IsModifiable,
    [property: JsonProperty("version")] byte Version = 1
);
