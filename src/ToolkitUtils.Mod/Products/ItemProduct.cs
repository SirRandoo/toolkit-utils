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
using NetEscapades.EnumGenerators;
using Newtonsoft.Json;
using RimWorld;
using ToolkitUtils.Mod.Products;
using Verse;

namespace ToolkitUtils.Mod.Domain.Products;

[JsonObject(MemberSerialization.OptIn)]
public sealed record ItemProduct(
    ThingDef Def,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("price")] int Price,
    [property: JsonProperty("purchasable")] bool Purchasable,
    [property: JsonProperty("karma")] Karma Karma,
    [property: JsonProperty("usable")] ItemActionSettings Usable,
    [property: JsonProperty("equippable")] ItemActionSettings Equippable,
    [property: JsonProperty("wearable")] ItemActionSettings Wearable,
    [property: JsonProperty("karmic_weight")] float Weight,
    [property: JsonProperty("quantity_limit")] int QuantityLimit,
    [property: JsonProperty("can_be_material")] bool CanBeMaterial,
    [property: JsonProperty("research_prerequisites")] IReadOnlyList<ResearchProjectDef> ResearchPrerequisites,
    [property: JsonProperty("quality_settings")] IReadOnlyDictionary<QualityCategory, QualitySettings> QualitySettings,
    [property: JsonProperty("metadata")] ItemProductMetadata Metadata
) : IProduct<ItemProductMetadata>
{
    /// <inheritdoc />
    [property: JsonProperty("id")] public string Id { get; } = Def.defName;
}

[Flags]
[PublicAPI]
[EnumExtensions]
public enum ItemProperties
{
    None = 0,
    Weapon = 1,
    RangedWeapon = Weapon | 2,
    MeleeWeapon = Weapon | 4,
    Material = 8,
    Consumable = 16,
    Apparel = 32,
    Manufacturable = 64,
    Tiny = 128,
    Wildlife = 256,
}

[JsonObject(MemberSerialization.OptIn)]
public sealed record ItemActionSettings([property: JsonProperty("enabled")] bool Enabled, [property: JsonProperty("karma_override")] Karma Karma);

[JsonObject(MemberSerialization.OptIn)]
public sealed record QualitySettings([property: JsonProperty("fixed_amount")] int FixedAmount, [property: JsonProperty("multiplier")] float Multiplier);

[JsonObject(MemberSerialization.OptIn)]
public sealed record ItemProductMetadata(
    [property: JsonProperty("mod")] Mod Mod,
    [property: JsonProperty("has_custom_name")] bool HasCustomName,
    [property: JsonProperty("properties")] ItemProperties Properties,
    [property: JsonProperty("local_cooldown")] TimeSpan LocalCooldown,
    [property: JsonProperty("global_cooldown")] TimeSpan GlobalCooldown
) : IProductMetadata;
