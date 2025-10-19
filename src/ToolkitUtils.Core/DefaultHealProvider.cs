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
// with ToolkitUtils.Core. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Mod;
using Verse;

namespace ToolkitUtils.Core;

public sealed class DefaultHealProvider : IHealProvider
{
    private static readonly Logger Logger = ToolkitLogManager.GetLogger<DefaultHealProvider>();

    private static readonly HashSet<HediffDef> LifeThreateningHediffDefs = [];
    private static readonly float HandCoverageAbsWithChildren;

    static DefaultHealProvider()
    {
        foreach (HediffDef def in DefDatabase<HediffDef>.AllDefs)
        {
            if (def.stages is not { Count: > 0, }) continue;


            for (var index = 0; index < def.stages.Count; index++)
            {
                if (!def.stages[index].lifeThreatening) continue;

                LifeThreateningHediffDefs.Add(def);

                break;
            }
        }

        HandCoverageAbsWithChildren = ThingDefOf.Human.race.body.GetPartsWithDef(BodyPartDefOf.Hand).First().coverageAbsWithChildren;
    }

    /// <inheritdoc />
    public string Id { get; init; } = "sirrandoo.tku:providers.heal";

    /// <inheritdoc />
    public string Name { get; init; } = "Heal provider (Default)";

    /// <inheritdoc />
    public int Priority => 1;

    /// <inheritdoc />
    public string[] RequiredMods { get; } = [];

    /// <inheritdoc />
    public async Task<Result> CanHealAsync(Hediff hediff) => Result.Ok(); // TODO: This currently heals every hediff, this should be changed to be more selective.

    /// <inheritdoc />
    public async Task<Result> CanHealAsync(Pawn pawn, BodyPartRecord record) => Result.Ok();

    /// <inheritdoc />
    public async Task<Result> HealAsync(Hediff hediff)
    {
        Pawn pawn = hediff.pawn;
        await MainThreadExtensions.OnMainAsync(pawn.health.RemoveHediff, hediff);

        if (!hediff.def.cureAllAtOnceIfCuredByItem) return Result.Ok();

        var num = 0;

        while (num < 10000)
        {
            Hediff firstHediffOfDef = await MainThreadExtensions.OnMainAsync(pawn.health.hediffSet.GetFirstHediffOfDef, hediff.def, arg2: false);

            if (firstHediffOfDef == null) break;

            await MainThreadExtensions.OnMainAsync(pawn.health.RemoveHediff, firstHediffOfDef);

            num++;
        }

        return Result.Ok();
    }

    /// <inheritdoc />
    public async Task<Result> HealAsync(Pawn pawn, BodyPartRecord record)
    {
        await MainThreadExtensions.OnMainAsync(RestoreBodyPart, pawn, record);

        return Result.Ok();
    }

    /// <inheritdoc />
    public async Task<Result> TryResurrectAsync(Pawn pawn)
    {
        try { return await MainThreadExtensions.OnMainAsync(TryResurrectInternal, pawn); }
        catch (Exception e)
        {
            Logger.Error(e, message: "Could not resurrect {PawnName}", pawn.LabelShort);

            return Result.Fail($"Could not resurrect {pawn.LabelShort}"); // TODO: Translate this message.
        }
    }

    private static Result TryResurrectInternal(Pawn pawn)
    {
        Pawn? target;

        if (pawn.SpawnedParentOrMe != pawn.Corpse
         && (target = pawn.SpawnedParentOrMe as Pawn) is not null
         && !target.carryTracker.TryDropCarriedThing(target.Position, ThingPlaceMode.Near, out Thing _))
        {
            Logger.Warn(message: "Could not drop {Target} at {Position} from {Carrier}", pawn.LabelShort, target.Position, target.LabelShort);

            return Result.Fail($"{pawn.LabelShort} is being carried by {target.LabelShort}, and could not be dropped."); // TODO: Translate this message.
        }

        pawn.ClearAllReservations();
        ResurrectionUtility.TryResurrect(
            pawn,
            new ResurrectionParams
            {
                removeDiedThoughts = true, restoreMissingParts = false,
            }
        );

        return Result.Ok();
    }

    private static void RestoreBodyPart(Pawn pawn, BodyPartRecord record)
    {
        pawn.health.RestorePart(record);
    }

    private static bool CanEverKill(Hediff hediff) => LifeThreateningHediffDefs.Contains(hediff.def);
}
