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

/// <summary>Represents a contract for configuring data operation settings.</summary>
/// <remarks>
///     This interface is designed to allow configuration of various aspects of data management, including
///     serialization formatting, threading behaviors, and version control. Implementers of this interface can specify
///     settings to customize how data is handled in an application.
/// </remarks>
[PublicAPI]
public sealed record DataSettings(
    [property: JsonProperty("should_minify")] bool ShouldMinify = true,
    [property: JsonProperty("use_background_thread")] bool UseBackgroundThread = true,
    [property: JsonProperty("version")] byte Version = 1
);
