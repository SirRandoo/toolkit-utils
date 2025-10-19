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
///     Represents the settings for different quality levels within the system. Provides access to individual quality
///     level multiplier settings as well as versioning information.
/// </summary>
[PublicAPI]
public sealed record QualitySettings(
    [property: JsonProperty("poor")] QualityMultiplierSettings Poor,
    [property: JsonProperty("awful")] QualityMultiplierSettings Awful,
    [property: JsonProperty("normal")] QualityMultiplierSettings Normal,
    [property: JsonProperty("good")] QualityMultiplierSettings Good,
    [property: JsonProperty("excellent")] QualityMultiplierSettings Excellent,
    [property: JsonProperty("masterwork")] QualityMultiplierSettings Masterwork,
    [property: JsonProperty("legendary")] QualityMultiplierSettings Legendary,
    [property: JsonProperty("version")] byte Version = 1
);

/// <summary>
///     Represents the settings for a quality level multiplier, including its enabling status, multiplier value, and
///     version.
/// </summary>
[PublicAPI]
public sealed record QualityMultiplierSettings(
    [property: JsonProperty("multiplier")] float Multiplier,
    [property: JsonProperty("is_enabled")] bool IsEnabled = true,
    [property: JsonProperty("version")] byte Version = 1
);
