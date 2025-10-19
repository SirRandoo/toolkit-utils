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
using Newtonsoft.Json;

namespace ToolkitUtils.Mod.Domain.Settings;

/// <summary>
///     Defines configuration settings related to the visibility of gear-related data such as equipped weapons, armor,
///     apparel, and temperature range. This interface establishes a contract for managing these settings in a customizable
///     manner.
/// </summary>
[PublicAPI]
public sealed record GearSettings(
    [property: JsonProperty("should_show_weapon")] bool ShouldShowWeapon = true,
    [property: JsonProperty("should_show_armor")] bool ShouldShowArmor = true,
    [property: JsonProperty("should_show_apparel")] bool ShouldShowApparel = true,
    [property: JsonProperty("should_show_temperature_range")] bool ShouldShowTemperatureRange = true,
    [property: JsonProperty("version")] byte Version = 1
);
