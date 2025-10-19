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
// with ToolkitUtils.TMagic. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ToolkitUtils.Mod.Domain.Powers;
using TorannMagic;
using Verse;

namespace ToolkitUtils.TMagic.Models;

internal interface ITorannPawnPower : IPawnPower<TMAbilityDef>;

internal interface ITorannPawnPowerTier : IPawnPowerTier<TMAbilityDef>;

internal sealed record ClassPower(
    TMAbilityDef Def,
    PowerTargetType TargetType,
    PowerTargetModifier TargetModifier,
    IReadOnlyList<IFaction> Factions,
    IReadOnlyList<IPawnPowerTier<TMAbilityDef>> Tiers
) : ITorannPawnPower
{
    public string Id { get; init; } = $"{Def.modContentPack.PackageId}:{Def.defName}";
    public string Name { get; init; } = Def.LabelCap;

    private static PowerTargetType ExtractTargetType(TMAbilityDef def)
    {
        var container = PowerTargetType.None;

        if (def.MainVerb.targetParams.canTargetAnimals) container |= PowerTargetType.Animals;
        if (def.MainVerb.targetParams.canTargetBuildings) container |= PowerTargetType.Buildings;
        if (def.MainVerb.targetParams.canTargetItems) container |= PowerTargetType.Items;
        if (def.MainVerb.targetParams.canTargetPawns) container |= PowerTargetType.Pawns;
        if (def.MainVerb.targetParams.canTargetSelf) container |= PowerTargetType.Self;
        if (def.MainVerb.targetParams.canTargetBloodfeeders) container |= PowerTargetType.Bloodfeeders;
        if (def.MainVerb.targetParams.canTargetCorpses) container |= PowerTargetType.Corpses;
        if (def.MainVerb.targetParams.canTargetFires) container |= PowerTargetType.Fires;
        if (def.MainVerb.targetParams.canTargetHumans) container |= PowerTargetType.Humans;
        if (def.MainVerb.targetParams.canTargetLocations) container |= PowerTargetType.Locations;
        if (def.MainVerb.targetParams.canTargetMechs) container |= PowerTargetType.Mechanoids;
        if (def.MainVerb.targetParams.canTargetPlants) container |= PowerTargetType.Plants;

        return container;
    }

    private static PowerTargetModifier ExtractTargetTypeModifiers(TMAbilityDef def)
    {
        var container = PowerTargetModifier.None;

        if (def.MainVerb.targetParams.onlyRepairableMechs) container |= PowerTargetModifier.OnlyRepairableMechanoids;
        if (def.MainVerb.targetParams.onlyTargetAnimaTrees) container |= PowerTargetModifier.OnlyAnimaTrees;
        if (def.MainVerb.targetParams.onlyTargetColonists) container |= PowerTargetModifier.OnlyColonists;
        if (def.MainVerb.targetParams.onlyTargetColonistsOrPrisoners) container |= PowerTargetModifier.OnlyColonists | PowerTargetModifier.OnlyPrisoners;

        if (def.MainVerb.targetParams.onlyTargetColonistsOrPrisonersOrSlaves)
            container |= PowerTargetModifier.OnlyColonists | PowerTargetModifier.OnlyPrisoners | PowerTargetModifier.OnlySlaves;

        if (def.MainVerb.targetParams.onlyTargetColonistsOrPrisonersOrSlavesAllowMinorMentalBreaks)
            container |= PowerTargetModifier.OnlyColonists | PowerTargetModifier.OnlyPrisoners | PowerTargetModifier.OnlySlaves | PowerTargetModifier.AllowMinorMentalBreaks;

        if (def.MainVerb.targetParams.onlyTargetControlledPawns) container |= PowerTargetModifier.OnlyControlledPawns;
        if (def.MainVerb.targetParams.onlyTargetCorpses) container |= PowerTargetModifier.OnlyCorpses;
        if (def.MainVerb.targetParams.onlyTargetDamagedThings) container |= PowerTargetModifier.OnlyDamagedThings;
        if (def.MainVerb.targetParams.onlyTargetDoors) container |= PowerTargetModifier.OnlyDoors;
        if (def.MainVerb.targetParams.onlyTargetFactions?.Count > 0) container |= PowerTargetModifier.OnlyFactions;
        if (def.MainVerb.targetParams.onlyTargetFlammables) container |= PowerTargetModifier.OnlyFlammables;
        if (def.MainVerb.targetParams.onlyTargetIncapacitatedPawns) container |= PowerTargetModifier.OnlyIncapacitatedPawns;
        if (def.MainVerb.targetParams.onlyTargetPrisonersOfColony) container |= PowerTargetModifier.OnlyPrisonersOfColony;
        if (def.MainVerb.targetParams.onlyTargetPsychicSensitive) container |= PowerTargetModifier.OnlyPsychicSensitive;
        if (def.MainVerb.targetParams.onlyTargetSameIdeo) container |= PowerTargetModifier.OnlySameIdeology;
        if (def.MainVerb.targetParams.onlyTargetThingsAffectingRegions) container |= PowerTargetModifier.OnlyThingsAffectingRegions;
        if (def.MainVerb.targetParams.neverTargetDoors) container |= PowerTargetModifier.NeverDoors;
        if (def.MainVerb.targetParams.neverTargetHostileFaction) container |= PowerTargetModifier.NeverHostileFaction;
        if (def.MainVerb.targetParams.neverTargetIncapacitated) container |= PowerTargetModifier.NeverIncapacitated;
        if (def.MainVerb.targetParams.mustBeSelectable) container |= PowerTargetModifier.MustBeSelectable;
        if (def.MainVerb.targetParams.targetSpecificThing != null) container |= PowerTargetModifier.SpecificThing;
        if (def.MainVerb.targetParams.thingCategory is not ThingCategory.None) container |= PowerTargetModifier.ThingCategory;

        return container;
    }

    private static TimeSpan GetDuration(TMAbilityDef def)
    {
        ReadOnlySpan<char> description = def.description.AsSpan();
        int durationIndex = def.description.IndexOf(value: "duration", StringComparison.InvariantCultureIgnoreCase);

        if (durationIndex == -1) return TimeSpan.Zero;

        description = description[durationIndex..];
        int separatorIndex = description.IndexOf(':');
        int durationRangeIndex = description.IndexOf('-');

        if (durationRangeIndex == -1)
        {
            description = description[(separatorIndex + 1)..];
            description = description[..(description.IndexOf('\n') - 2)];

            return TimeSpan.FromSeconds(float.Parse(new string(description.ToArray())));
        }

        description = description[(separatorIndex + 1)..];
        description = description[..(description.IndexOf('\n') - 2)];

        return TimeSpan.FromSeconds(float.Parse(new string(description.ToArray())));
    }

    [PublicAPI]
    public sealed class Builder(TMAbilityDef def)
    {
        private readonly List<IPawnPowerTier<TMAbilityDef>> _tiers = [];

        public Builder WithTier(int tier, TMAbilityDef tieredDef)
        {
            var cost = 0f;

            if (def.chiCost > 0)
                cost = def.chiCost;
            else if (def.manaCost > 0)
                cost = def.manaCost;
            else if (def.bloodCost > 0)
                cost = def.bloodCost;
            else if (def.hediffCost > 0)
                cost = def.hediffCost;
            else if (def.needCost > 0)
                cost = def.needCost;
            else if (def.staminaCost > 0) cost = def.staminaCost;

            _tiers.Add(
                new TieredClassPower(tieredDef, tier, cost, GetDuration(tieredDef), tieredDef.MainVerb.Ranged ? tieredDef.MainVerb.range : 0f, tieredDef.MainVerb.warmupTime)
            );

            return this;
        }

        public ClassPower Build()
        {
            var factions = new List<IFaction>(); // TODO: Populate this list with the factions the ability can target, if any.

            return new ClassPower(def, ExtractTargetType(def), ExtractTargetTypeModifiers(def), factions, _tiers);
        }
    }

    private sealed record TieredClassPower(TMAbilityDef Def, int Level, float Cost, TimeSpan Duration, float Range, float CastTime) : ITorannPawnPowerTier
    {
        /// <inheritdoc />
        public string Id { get; } = $"{Def.modContentPack.PackageId}:{Def.defName}:{Level}";

        /// <inheritdoc />
        public string Name { get; init; } = Def.LabelCap;
    }
}
