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

/// <summary>Represents the configuration settings for passion-related behavior.</summary>
/// <remarks>
///     This interface defines properties to configure various probabilities and behaviors for simulations or systems
///     that utilize passion-driven logic or randomness.
/// </remarks>
[PublicAPI]
public sealed record PassionSettings(
    [property: JsonProperty("use_randomness")] bool UseRandomness = true,
    [property: JsonProperty("fail_chance")] float FailChance = 0.1f,
    [property: JsonProperty("hop_chance")] float HopChance = 0.1f,
    [property: JsonProperty("inverse_chance")] float InverseChance = 0.1f,
    [property: JsonProperty("version")] byte Version = 1
);
