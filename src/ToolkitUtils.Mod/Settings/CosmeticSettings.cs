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

/// <summary>Defines an interface for configuring cosmetic settings.</summary>
/// <remarks>
///     This interface outlines properties to manage cosmetic-related features, including settings for hair dye,
///     transparency effects, and gateway puff options, as well as a version indicator for these settings.
/// </remarks>
[PublicAPI]
public sealed record CosmeticSettings([property: JsonProperty("allows_transparency")] bool AllowsTransparency = false, [property: JsonProperty("version")] byte Version = 1);
