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

[JsonObject(MemberSerialization.OptIn)]
public sealed record TraitProduct(
    TraitDef Def,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("price_to_add")] int PriceToAdd,
    [property: JsonProperty("price_to_remove")] int PriceToRemove,
    [property: JsonProperty("degree")] byte Degree,
    [property: JsonProperty("description")] string Description,
    [property: JsonProperty("can_add")] bool CanAdd,
    [property: JsonProperty("can_remove")] bool CanRemove,
    [property: JsonProperty("karmic_weight")] float Weight,
    [property: JsonProperty("karma_for_removing")] Karma KarmaForRemoving,
    [property: JsonProperty("karma_for_adding")] Karma KarmaForAdding,
    [property: JsonProperty("conflicting_traits")] IReadOnlyList<Trait> ConflictingTraits,
    [property: JsonProperty("research_prerequisites")] IReadOnlyList<ResearchProjectDef> ResearchPrerequisites,
    [property: JsonProperty("bypasses_trait_limit")] bool BypassesTraitLimit,
    [property: JsonProperty("metadata")] TraitProductMetadata Metadata
) : IProduct<TraitProductMetadata>
{
    /// <inheritdoc />
    [JsonProperty("id")] public string Id => Def.defName;
}

[JsonObject(MemberSerialization.OptIn)]
public sealed record TraitProductMetadata(
    [property: JsonProperty("stats")] IReadOnlyList<string> Stats,
    [property: JsonProperty("mod")] Mod Mod,
    [property: JsonProperty("has_custom_name")] bool HasCustomName,
    [property: JsonProperty("local_cooldown")] TimeSpan LocalCooldown,
    [property: JsonProperty("global_cooldown")] TimeSpan GlobalCooldown,
    [property: JsonProperty("properties")] TraitProperties Properties
) : IProductMetadata;

/// <summary>Represents a set of properties that define specific aspects or characteristics of a trait.</summary>
/// <remarks>
///     This enumeration is decorated with the <see cref="FlagsAttribute" />, allowing a bitwise combination of its
///     values to represent multiple trait attributes simultaneously. It is primarily used to categorize and manage traits
///     within a system, offering a structured way to associate properties such as gender identity and sexuality.
/// </remarks>
[Flags]
public enum TraitProperties
{
    /// <summary>Represents the absence of any specific trait property.</summary>
    /// <remarks>
    ///     The <see cref="None" /> member indicates that no defined properties are associated or relevant. This is
    ///     typically used as a default value within the <see cref="TraitProperties" /> enumeration.
    /// </remarks>
    None = 0,

    /// <summary>Represents a trait property related to an individual's sexual orientation.</summary>
    /// <remarks>
    ///     The <see cref="SexualOrientation" /> member is used to characterize traits associated with a person's patterns
    ///     of emotional, romantic, or sexual attraction. This allows for nuanced modeling of individuality and interpersonal
    ///     dynamics within the system.
    /// </remarks>
    SexualOrientation = 1,

    /// <summary>
    ///     Represents a trait property corresponding to an individual's self-perception and identification with a
    ///     particular gender or lack thereof.
    /// </summary>
    /// <remarks>
    ///     The <see cref="GenderIdentity" /> member is used to define traits that are specifically associated with gender
    ///     identity, enabling nuanced representation. This enumeration value may be combined with other trait properties
    ///     within the <see cref="TraitProperties" /> enum to create detailed and complex profiles.
    /// </remarks>
    GenderIdentity = 2,
}
