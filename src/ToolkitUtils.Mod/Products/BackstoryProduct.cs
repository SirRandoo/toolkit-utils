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
using Verse;

namespace ToolkitUtils.Mod.Products;

public sealed record Cooldown;

/// <summary>Represents a purchasable backstory.</summary>
/// <param name="Def">The <see cref="BackstoryDef" /> being put up for sale.</param>
/// <param name="Name">
///     The name of the backstory being put up for sale. Users are permitted to change the name of products,
///     so this property may differ from the value of <see cref="BackstoryDef.label" />.
/// </param>
/// <param name="Price">The number of coins viewers will spend in order to purchase the backstory.</param>
/// <param name="Enabled">Whether viewers are permitted to purchase the backstory.</param>
/// <param name="Karma">The karmic alignment of the backstory product.</param>
/// <param name="Weight">A multiplier that scales how much karmic debt the viewer accrues by purchasing the backstory.</param>
/// <param name="ResearchPrerequisites">
///     A collection of research projects that must be researched before the backstory can
///     be purchased by viewers.
/// </param>
/// <param name="Metadata">The associated metadata of the backstory.</param>
[JsonObject(MemberSerialization.OptIn)]
public sealed record BackstoryProduct(
    BackstoryDef Def,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("price")] int Price,
    [property: JsonProperty("enabled")] bool Enabled,
    [property: JsonProperty("karma")] Karma Karma,
    [property: JsonProperty("karmic_weight")] float Weight,
    [property: JsonProperty("research_prerequisites")] IReadOnlyList<ResearchProjectDef> ResearchPrerequisites,
    [property: JsonProperty("metadata")] BackstoryProductMetadata Metadata
) : IProduct<BackstoryProductMetadata>
{
    private readonly IProductMetadata _metadata = Metadata;

    /// <summary>The unique id of the backstory.</summary>
    [JsonProperty("id")] public string Id => Def.defName;
}

[JsonObject(MemberSerialization.OptIn)]
public sealed record BackstoryProductMetadata(
    [property: JsonProperty("has_custom_name")] bool HasCustomName,
    [property: JsonProperty("mod")] Mod Mod,
    [property: JsonProperty("slot")] BackstorySlot Slot,
    [property: JsonProperty("disabled_work_tags")] WorkTags DisabledWorkTags,
    [property: JsonProperty("required_work_tags")] WorkTags RequiredWorkTags,
    [property: JsonProperty("skill_gains")] IReadOnlyList<SkillGain> SkillGains,
    [property: JsonProperty("forced_traits")] IReadOnlyList<Trait> ForcedTraits,
    [property: JsonProperty("disallowed_traits")] IReadOnlyList<Trait> DisallowedTraits,
    [property: JsonProperty("local_cooldown")] TimeSpan LocalCooldown,
    [property: JsonProperty("global_cooldown")] TimeSpan GlobalCooldown
) : IProductMetadata;
