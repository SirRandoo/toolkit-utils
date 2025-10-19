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
// with ToolkitUtils.HAR. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlienRace;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;

namespace ToolkitUtils.HAR;

[PublicAPI]
public sealed class AlienCompatibilityProvider(RouterService router) : IPawnProvider, ITraitProvider
{
    private const string ModId = "erdelf.HumanoidAlienRaces";

    /// <inheritdoc />
    public string Id { get; init; } = "sirrandoo.tku:compatibility.humanoidAlienRaces";

    /// <inheritdoc />
    public string Name { get; init; } = "Humanoid Alien Races Compatibility";

    /// <inheritdoc />
    public int Priority { get; } = 1;

    /// <inheritdoc />
    public string[] RequiredMods { get; } =
    {
        ModId,
    };

    /// <inheritdoc />
    public async Task<Result> CanCreateAsync(PawnKindDef kind, XenotypeDef? xenotype = null) => throw new NotSupportedException("");

    /// <inheritdoc />
    public async Task<Pawn> CreateAsync(PawnKindDef kind, XenotypeDef? xenotype = null) => throw new NotSupportedException();

    /// <inheritdoc />
    public async Task<Pawn> TransformBodyAsync(Pawn pawn, BodyTypeDef? bodyType = null)
    {
        if (bodyType is null && pawn.def is ThingDef_AlienRace alienRace)
        {
            AlienPartGenerator generator = alienRace.alienRace.generalSettings.alienPartGenerator;
            List<BodyTypeDef> bodyTypes = generator.bodyTypes == null ? [] : [..generator.bodyTypes,];

            if (bodyTypes.Count <= 0 || bodyTypes!.Contains(pawn.story.bodyType)) return pawn;

            switch (pawn.gender)
            {
                case Gender.Male:   bodyTypes.Remove(BodyTypeDefOf.Female); break;
                case Gender.Female: bodyTypes.Remove(BodyTypeDefOf.Male); break;
            }

            pawn.story.bodyType = bodyTypes.TryRandomElement(out BodyTypeDef newBody) ? newBody : BodyTypeDefOf.Thin;

            return pawn;
        }

        pawn.story.bodyType = bodyType;

        return pawn;
    }

    /// <inheritdoc />
    public Result IsValidPawnCandidate(Pawn pawn) => Result.Fail(); // This provider doesn't support pawn restrictions.

    /// <inheritdoc />
    public async Task<Result> CanPurchaseTraitAsync(Pawn pawn, TraitDef trait, int severity) =>
        IsTraitDisallowed(pawn, trait.defName, severity)
            ? Result.Fail(new Translation($"The trait '{trait.DataAtDegree(severity).label}' is forbidden because of the pawn's race.".MarkNotTranslated()))
            : Result.Ok();

    /// <inheritdoc />
    public async Task<Result> CanPurchaseTraitRemovalAsync(Pawn pawn, TraitDef trait, int severity) =>
        !IsTraitForced(pawn, trait.defName, severity) && !IsTraitDisallowed(pawn, trait.defName, severity)
            ? Result.Ok()
            : Result.Fail(new Translation("The trait cannot be removed.".MarkNotTranslated()));

    /// <inheritdoc />
    public async Task<Result> PurchaseTraitAsync(Pawn pawn, TraitDef trait, int severity) =>
        await router.RouteToMainAsync(AddTrait, pawn, trait, severity)
            ? Result.Ok()
            : Result.Fail(new Translation("The trait wasn't added for an unknown reason.".MarkNotTranslated()));

    /// <inheritdoc />
    public async Task<Result> PurchaseTraitRemovalAsync(Pawn pawn, TraitDef trait, int severity) =>
        await router.RouteToMainAsync(RemoveTrait, pawn, trait, severity)
            ? Result.Ok()
            : Result.Fail(new Translation("The trait wasn't removed for an unknown reason.".MarkNotTranslated()));

    private static bool IsTraitForced(Thing pawn, string defName, int degree)
    {
        if (pawn.def is not ThingDef_AlienRace alienRace) return false;

        List<AlienChanceEntry<TraitWithDegree>> entries = alienRace.alienRace.generalSettings.forcedRaceTraitEntries;

        if (entries == null) return false;

        for (var index = 0; index < entries.Count; index++)
        {
            AlienChanceEntry<TraitWithDegree> entry = entries[index];

            if (string.Equals(entry.entry.def.defName, defName, StringComparison.Ordinal) && entry.entry.degree == degree && entry.chance >= 1f) return true;
        }

        return false;
    }

    private static bool IsTraitDisallowed(Thing pawn, string defName, int degree)
    {
        if (pawn.def is not ThingDef_AlienRace alienRace) return false;

        List<AlienChanceEntry<TraitWithDegree>> entries = alienRace.alienRace.generalSettings.disallowedTraits;

        if (entries == null) return false;

        for (var index = 0; index < entries.Count; index++)
        {
            AlienChanceEntry<TraitWithDegree> entry = entries[index];

            if (string.Equals(entry.entry.def.defName, defName, StringComparison.Ordinal) && entry.entry.degree == degree && entry.chance >= 1f) return true;
        }

        return false;
    }

    private static bool AddTrait(Pawn pawn, TraitDef trait, int severity)
    {
        pawn.story.traits.allTraits.Add(new Trait(trait, severity));

        // TODO: Make skill adjustments
        // TODO: Make "incapable" adjustments.

        return true;
    }

    private static bool RemoveTrait(Pawn pawn, TraitDef trait, int severity)
    {
        // TODO: Make skill adjustments
        // TODO: Make "incapable" adjustments.

        return pawn.story.traits.allTraits.RemoveAll(t => string.Equals(t.def.defName, trait.defName, StringComparison.Ordinal) && t.CurrentData.degree == severity) > 0;
    }
}
