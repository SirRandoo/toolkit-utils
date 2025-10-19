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

/// <summary>
///     Represents a product that is associated with a specific pawn kind and xenotype in the RimWorld universe. This
///     class encapsulates details such as the pawn's type, custom name, pricing, description, and associated metadata.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public sealed record PawnProduct(
    [property: JsonProperty("def")] PawnKindDef PawnKindDef,
    [property: JsonProperty("sub_def")] XenotypeDef XenotypeDef,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("price")] int Price,
    [property: JsonProperty("description")] string Description,
    [property: JsonProperty("purchasable")] bool Purchasable,
    [property: JsonProperty("karmic_weight")] float Weight,
    [property: JsonProperty("karma")] Karma Karma,
    [property: JsonProperty("research_prerequisites")] IReadOnlyList<ResearchProjectDef> ResearchPrerequisites,
    [property: JsonProperty("metadata")] PawnProductMetadata Metadata
) : IProduct<PawnProductMetadata>
{
    /// <summary>
    ///     Represents a derived identifier for the product, generated based on the associated Xenotype definition's
    ///     unique name.
    /// </summary>
    public string SubId => XenotypeDef.defName;

    /// <inheritdoc />
    public string Id => PawnKindDef.defName;
}

/// <summary>
///     Provides additional details and attributes associated with a pawn product, including name customization,
///     statistical data, mod source, and cooldown periods.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public sealed record PawnProductMetadata(
    [property: JsonProperty("has_custom_name")] bool HasCustomName,
    [property: JsonProperty("stats")] IReadOnlyList<string> Stats,
    [property: JsonProperty("mod")] Mod Mod,
    [property: JsonProperty("local_cooldown")] TimeSpan LocalCooldown,
    [property: JsonProperty("global_cooldown")] TimeSpan GlobalCooldown
) : IProductMetadata;
