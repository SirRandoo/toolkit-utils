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

/// <summary>Defines the properties for miscellaneous settings customization.</summary>
/// <remarks>
///     This interface provides configuration options for enabling or disabling various Easter eggs and specifying
///     version information in an application.
/// </remarks>
[PublicAPI]
public sealed record MiscSettings(
    [property: JsonProperty("use_viewer_easter_eggs")] bool UseViewerEasterEggs = true,
    [property: JsonProperty("use_developer_easter_eggs")] bool UseDeveloperEasterEggs = true,
    [property: JsonProperty("use_gateway_easter_eggs")] bool UseGatewayEasterEggs = true,
    [property: JsonProperty("version")] byte Version = 1
);
