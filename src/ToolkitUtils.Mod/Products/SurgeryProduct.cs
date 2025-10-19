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
public sealed record SurgeryProduct(
    RecipeDef Def,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("price")] int Price,
    [property: JsonProperty("purchasable")] bool Purchasable,
    [property: JsonProperty("karmic_weight")] float Weight,
    [property: JsonProperty("karma")] Karma Karma,
    [property: JsonProperty("category")] WorkTypeDef Category,
    [property: JsonProperty("research_prerequisites")] IReadOnlyList<ResearchProjectDef> ResearchPrerequisites,
    [property: JsonProperty("metadata")] SurgeryProductMetadata Metadata
) : IProduct<SurgeryProductMetadata>
{
    [JsonProperty("id")] public string Id => Def.defName;
}

[JsonObject(MemberSerialization.OptIn)]
public sealed record SurgeryProductMetadata(
    [property: JsonProperty("mod")] Mod Mod,
    [property: JsonProperty("has_custom_name")] bool HasCustomName,
    [property: JsonProperty("local_cooldown")] TimeSpan LocalCooldown,
    [property: JsonProperty("global_cooldown")] TimeSpan GlobalCooldown
) : IProductMetadata;
