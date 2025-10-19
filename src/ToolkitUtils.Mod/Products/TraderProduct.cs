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
using RimWorld;
using ToolkitUtils.Mod.Products;
using Verse;

namespace ToolkitUtils.Mod.Domain.Products;

public sealed record TraderProduct(
    TraderKindDef Def,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("price")] int Price,
    [property: JsonProperty("category")] string Category,
    [property: JsonProperty("enabled")] bool Enabled,
    [property: JsonProperty("karma")] Karma Karma,
    [property: JsonProperty("karmic_weight")] float Weight,
    [property: JsonProperty("research_prerequisites")] IReadOnlyList<ResearchProjectDef> ResearchPrerequisites,
    [property: JsonProperty("metadata")] TraderProductMetadata Metadata
) : IProduct<TraderProductMetadata>
{
    /// <inheritdoc />
    [JsonProperty("id")] public string Id => Def.defName;
}

public sealed record TraderProductMetadata(
    [property: JsonProperty("mod")] Mod Mod,
    [property: JsonProperty("has_custom_name")] bool HasCustomName,
    [property: JsonProperty("is_orbital")] bool IsOrbital,
    [property: JsonProperty("requires_royal_permit")] bool RequiresRoyalPermit,
    [property: JsonProperty("royal_title_required")] RoyalTitleDef RoyalTitleRequired,
    [property: JsonProperty("local_cooldown")] TimeSpan LocalCooldown,
    [property: JsonProperty("global_cooldown")] TimeSpan GlobalCooldown
) : IProductMetadata;
