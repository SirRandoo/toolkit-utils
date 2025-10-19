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

/// <summary>Contains the settings for commands within the mod.</summary>
[PublicAPI]
public sealed record CommandSettings(
    [property: JsonProperty("command_handler_type")] CommandHandlerType CommandHandlerType,
    [property: JsonProperty("command_prefix")] string CommandPrefix = "!",
    [property: JsonProperty("buy_prefix")] string BuyPrefix = "$",
    [property: JsonProperty("use_emojis")] bool UseEmojis = true,
    [property: JsonProperty("lookup_limit")] int LookupLimit = 10,
    [property: JsonProperty("version")] byte Version = 1
);
