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
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ToolkitUtils.Mod.Products;
using Verse;

namespace ToolkitUtils.Mod.Domain.Products;

[JsonObject(MemberSerialization.OptIn)]
public sealed record WeatherProduct(
    WeatherDef Def,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("price")] int Price,
    [property: JsonProperty("enabled")] bool Enabled,
    [property: JsonProperty("karma")] Karma Karma,
    [property: JsonProperty("karmic_weight")] float Weight,
    [property: JsonProperty("research_prerequisites")] IReadOnlyList<ResearchProjectDef> ResearchPrerequisites,
    [property: JsonProperty("metadata")] WeatherProductMetadata Metadata
) : IProduct<WeatherProductMetadata>
{
    /// <inheritdoc />
    [property: JsonProperty("id")] public string Id { get; } = Def.defName;
}

[JsonObject(MemberSerialization.OptIn)]
public sealed record WeatherProductMetadata(
    [property: JsonProperty("mod")] Mod Mod,
    [property: JsonProperty("is_bad")] bool IsBad,
    [property: JsonProperty("duration")] IntegerRange Duration,
    [property: JsonProperty("has_custom_name")] bool HasCustomName,
    [property: JsonProperty("minimum_monolith_level")] byte MinimumMonolithLevel,
    [property: JsonProperty("local_cooldown")] TimeSpan LocalCooldown,
    [property: JsonProperty("global_cooldown")] TimeSpan GlobalCooldown
) : IProductMetadata;
