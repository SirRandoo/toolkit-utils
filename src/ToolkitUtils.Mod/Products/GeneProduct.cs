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
using System.Collections.Generic;
using Newtonsoft.Json;
using RimWorld;
using Verse;

namespace ToolkitUtils.Mod.Products;

public sealed record GeneProduct(
    GeneDef Def,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("category")] GeneCategoryDef Category,
    [property: JsonProperty("karmic_weight")] float Weight,
    [property: JsonProperty("karma")] Karma Karma,
    [property: JsonProperty("purchasable")] bool Purchasable,
    [property: JsonProperty("price")] int Price,
    [property: JsonProperty("research_prerequisites")] IReadOnlyList<ResearchProjectDef> ResearchPrerequisites,
    [property: JsonProperty("metadata")] GeneProductMetadata Metadata
) : IProduct<GeneProductMetadata>
{
    /// <inheritdoc />
    [JsonProperty("id")] public string Id => Def.defName;
}

public sealed record GeneProductMetadata(
    [property: JsonProperty("mod")] Mod Mod,
    [property: JsonProperty("has_custom_name")] bool HasCustomName,
    [property: JsonProperty("abilities")] IReadOnlyList<AbilityDef> Abilities,
    [property: JsonProperty("forced_traits")] IReadOnlyList<GeneticTraitData> ForcedTraits,
    [property: JsonProperty("suppressed_traits")] IReadOnlyList<GeneticTraitData> SuppressedTraits,
    [property: JsonProperty("disables_needs")] IReadOnlyList<NeedDef> DisablesNeeds,
    [property: JsonProperty("causes_need")] NeedDef? CausesNeed,
    [property: JsonProperty("disabled_work_tags")] WorkTags DisabledWorkTags,
    [property: JsonProperty("ignore_darkness")] bool IgnoreDarkness,
    [property: JsonProperty("endogene_category")] EndogeneCategory EndogeneCategory,
    [property: JsonProperty("dislikes_sunlight")] bool DislikesSunlight,
    [property: JsonProperty("active_at_age")] float ActiveAtAge,
    [property: JsonProperty("immune_to_tox_gas_exposure")] bool ImmuneToToxGasExposure,
    [property: JsonProperty("stat_offsets")] IReadOnlyList<StatModifier> StatOffsets,
    [property: JsonProperty("stat_factors")] IReadOnlyList<StatModifier> StatFactors,
    [property: JsonProperty("pain_offset")] float PainOffset,
    [property: JsonProperty("pain_factor")] float PainFactor,
    [property: JsonProperty("food_poisoning_chance_factor")] float FoodPoisoningChanceFactor,
    [property: JsonProperty("immune_to")] IReadOnlyList<HediffDef> ImmuneTo,
    [property: JsonProperty("hediff_givers_cannot_give")] IReadOnlyList<HediffDef> HediffGiversCannotGive,
    [property: JsonProperty("chemical")] ChemicalDef ChemicalDef,
    [property: JsonProperty("addiction_chance_factor")] float AdditionChanceFactor,
    [property: JsonProperty("overdose_chance_factor")] float OverdoseChanceFactor,
    [property: JsonProperty("tolerance_buildup_factor")] float ToleranceBuildupFactor,
    [property: JsonProperty("sterilize")] bool Sterilize,
    [property: JsonProperty("prevent_permanent_wounds")] bool PreventPermanentWounds,
    [property: JsonProperty("dont_mind_raw_food")] bool DontMindRawFood,
    [property: JsonProperty("social_fight_chance_factor")] float SocialFightChanceFactor,
    [property: JsonProperty("aggro_mental_break_selection_chance_factor")] float AggroMentalBreakSelectionChanceFactor,
    [property: JsonProperty("mental_break_mtb_days")] float MentalBreakMtbDays,
    [property: JsonProperty("mental_break_def")] MentalBreakDef MentalBreakDef,
    [property: JsonProperty("missing_gene_romance_chance_factor")] float MissingGeneRomanceChanceFactor,
    [property: JsonProperty("prison_break_mtb_factor")] float PrisonBreakMtbFactor,
    [property: JsonProperty("prerequisite")] GeneDef Prerequisite,
    [property: JsonProperty("market_value_factor")] float MarketValueFactor
) : IProductMetadata;
