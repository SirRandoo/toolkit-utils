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
using JetBrains.Annotations;
using Newtonsoft.Json;
using TwitchToolkit.Incidents;
using Verse;

namespace ToolkitUtils.Mod.Products;

[JsonObject(MemberSerialization.OptIn)]
public sealed record PriceRange([property: JsonProperty("minimum")] int Minimum, [property: JsonProperty("maximum")] int Maximum);

[JsonObject(MemberSerialization.OptIn)]
public sealed record EventProduct(
    StoreIncident Def,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("price")] int Price,
    [property: JsonProperty("wager")] PriceRange Wager,
    [property: JsonProperty("enabled")] bool Enabled,
    [property: JsonProperty("karma")] Karma Karma,
    [property: JsonProperty("event_cap")] byte EventCap,
    [property: JsonProperty("karmic_weight")] float Weight,
    [property: JsonProperty("research_prerequisites")] IReadOnlyList<ResearchProjectDef> ResearchPrerequisites,
    [property: JsonProperty("metadata")] EventProductMetadata Metadata
) : IProduct<EventProductMetadata>
{
    /// <inheritdoc />
    [JsonProperty("id")] public string Id => Def.defName;

    public bool ShouldSerializeWager() => Def is StoreIncidentVariables { maxWager: > 0, };
}

[Flags]
[PublicAPI]
public enum EventProperties
{
    None = 0, Variables = 1, Configurable = Variables | 2,
}

[JsonObject(MemberSerialization.OptIn)]
public sealed record EventProductMetadata(
    [property: JsonProperty("mod")] Mod Mod,
    [property: JsonProperty("has_custom_name")] bool HasCustomName,
    [property: JsonProperty("event_type")] EventTypes EventType,
    [property: JsonProperty("properties")] EventProperties Properties,
    [property: JsonProperty("local_cooldown")] TimeSpan LocalCooldown,
    [property: JsonProperty("global_cooldown")] TimeSpan GlobalCooldown
) : IProductMetadata;
