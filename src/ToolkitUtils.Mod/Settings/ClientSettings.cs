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
using UnityEngine;

namespace ToolkitUtils.Mod.Domain.Settings;

/// <summary>Represents client-specific settings for an application.</summary>
[PublicAPI]
public sealed record ClientSettings(
    [property: JsonProperty("voting_window_position")] Vector2 VotingWindowPosition,
    [property: JsonProperty("connection_settings")] ConnectionSettings ConnectionSettings,
    [property: JsonProperty("use_larger_voting_window")] bool UseLargerVotingWindow = false,
    [property: JsonProperty("is_first_installation")] bool IsFirstInstallation = true,
    [property: JsonProperty("use_gateway_puff")] bool UseGatewayPuff = true,
    [property: JsonProperty("should_dye_hair")] bool ShouldDyeHair = true,
    [property: JsonProperty("version")] byte Version = 1
);
