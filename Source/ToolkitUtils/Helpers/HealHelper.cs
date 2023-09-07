// ToolkitUtils
// Copyright (C) 2021  SirRandoo
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TwitchToolkit.IncidentHelpers.Special;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers;

public static class HealHelper
{
    private static float HandCoverageAbsWithChildren => ThingDefOf.Human.race.body.GetPartsWithDef(BodyPartDefOf.Hand).First().coverageAbsWithChildren;

    private static bool CanEverKill(Hediff hediff)
    {
        return hediff.def.stages?.Any(s => s.lifeThreatening) ?? hediff.def.lethalSeverity >= 0f;
    }

    public static void Cure(Hediff hediff)
    {
        Pawn pawn = hediff.pawn;
        pawn.health.RemoveHediff(hediff);

        if (!hediff.def.cureAllAtOnceIfCuredByItem)
        {
            return;
        }

        var num = 0;

        while (true)
        {
            num++;

            if (num > 10000)
            {
                TkUtils.Logger.Warn("HealHelper iterated too many times.");

                break;
            }

            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediff.def);

            if (firstHediffOfDef is null)
            {
                break;
            }

            pawn.health.RemoveHediff(firstHediffOfDef);
        }
    }

    private static Hediff_Addiction? FindAddiction(Pawn pawn)
    {
        List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

        for (var index = 0; index < hediffs.Count; index++)
        {
            Hediff hediff = hediffs[index];

            if (!CompatRegistry.IsHealable(hediff))
            {
                continue;
            }

            if (hediff is Hediff_Addiction { Visible: true } a && a.def.everCurableByItem)
            {
                return a;
            }
        }

        return null;
    }

    private static BodyPartRecord? FindBiggestMissingBodyPart(Pawn pawn, float minCoverage = 0f)
    {
        BodyPartRecord? record = null;

        List<Hediff_MissingPart>? list = pawn.health.hediffSet.GetMissingPartsCommonAncestors();

        for (var index = 0; index < list.Count; index++)
        {
            Hediff_MissingPart missing = list[index];

            if (!CompatRegistry.IsHealable(missing))
            {
                continue;
            }

            if (missing.Part.coverageAbsWithChildren >= minCoverage && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(missing.Part)
                && (record is null || missing.Part.coverageAbsWithChildren > record.coverageAbsWithChildren))
            {
                record = missing.Part;
            }
        }

        return record;
    }

    private static Hediff? FindImmunizableHediffWhichCanKill(Pawn pawn)
    {
        Hediff? hediff = null;
        float num = -1f;
        List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

        for (var index = 0; index < hediffs.Count; index++)
        {
            Hediff h = hediffs[index];

            if (!CompatRegistry.IsHealable(h))
            {
                continue;
            }

            if (!h.Visible || !h.def.everCurableByItem || h.TryGetComp<HediffComp_Immunizable>() == null || h.FullyImmune() || !CanEverKill(h))
            {
                continue;
            }

            float severity = h.Severity;

            if (hediff is not null && severity <= num)
            {
                continue;
            }

            hediff = h;
            num = severity;
        }

        return hediff;
    }

    private static Hediff_Injury? FindInjury(Pawn pawn, IReadOnlyCollection<BodyPartRecord>? allowedBodyParts = null)
    {
        Hediff_Injury? injury = null;
        List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

        for (var index = 0; index < hediffs.Count; index++)
        {
            Hediff h = hediffs[index];

            if (!CompatRegistry.IsHealable(h))
            {
                continue;
            }

            if (h is Hediff_Injury { Visible: true } h2 && h2.def.everCurableByItem && (allowedBodyParts is null || allowedBodyParts.Contains(h2.Part))
                && (injury is null || h2.Severity > injury.Severity))
            {
                injury = h2;
            }
        }

        return injury;
    }

    private static Hediff? FindLifeThreateningHediff(Pawn pawn)
    {
        Hediff? hediff = null;
        float num = -1f;
        List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

        for (var index = 0; index < hediffs.Count; index++)
        {
            Hediff h = hediffs[index];

            if (!CompatRegistry.IsHealable(h))
            {
                continue;
            }

            if (!h.Visible || !h.def.everCurableByItem || h.FullyImmune())
            {
                continue;
            }

            bool lifeThreatening = h.CurStage?.lifeThreatening ?? false;
            bool lethal = h.def.lethalSeverity >= 0f && h.Severity / h.def.lethalSeverity >= 0.8f;

            if (!lifeThreatening && !lethal)
            {
                continue;
            }

            float coverage = h.Part?.coverageAbsWithChildren ?? 999f;

            if (hediff is not null && coverage <= num)
            {
                continue;
            }

            hediff = h;
            num = coverage;
        }

        return hediff;
    }

    private static Hediff? FindMostBleedingHediff(Pawn pawn)
    {
        var num = 0f;
        Hediff? hediff = null;
        List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

        for (var index = 0; index < hediffs.Count; index++)
        {
            Hediff h = hediffs[index];

            if (!CompatRegistry.IsHealable(h))
            {
                continue;
            }

            if (!h.Visible || !h.def.everCurableByItem)
            {
                continue;
            }

            float bleedRate = h.BleedRate;

            if (bleedRate <= 0f || (bleedRate <= num && hediff is not null))
            {
                continue;
            }

            num = bleedRate;
            hediff = h;
        }

        return hediff;
    }

    private static Hediff? FindNonInjuryMiscBadHediff(Pawn pawn, bool onlyIfCanKill)
    {
        Hediff? hediff = null;
        var num = 1f;
        List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

        for (var index = 0; index < hediffs.Count; index++)
        {
            Hediff h = hediffs[index];

            if (!CompatRegistry.IsHealable(h))
            {
                continue;
            }

            if (!h.Visible || !h.def.isBad || !h.def.everCurableByItem || h is Hediff_Injury || h is Hediff_MissingPart || h is Hediff_Addiction || h is Hediff_AddedPart
                || (onlyIfCanKill && !CanEverKill(h)))
            {
                continue;
            }

            if (h.def == HediffDefOf.BloodLoss || h.def == HediffDefOf.Malnutrition)
            {
                continue;
            }

            float coverage = h.Part?.coverageAbsWithChildren ?? 999f;

            if (hediff is not null && coverage <= num)
            {
                continue;
            }

            hediff = h;
            num = coverage;
        }

        return hediff;
    }

    public static Hediff_Injury? FindPermanentInjury(Pawn pawn, IReadOnlyCollection<BodyPartRecord>? allowedBodyParts = null)
    {
        Hediff_Injury? injury = null;
        List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

        for (var index = 0; index < hediffs.Count; index++)
        {
            Hediff h = hediffs[index];

            if (!CompatRegistry.IsHealable(h))
            {
                continue;
            }

            if (h is Hediff_Injury { Visible: true } h2 && h2.IsPermanent() && h2.def.everCurableByItem && (allowedBodyParts is null || allowedBodyParts.Contains(h2.Part))
                && (injury is null || h2.Severity > injury.Severity))
            {
                injury = h2;
            }
        }

        return injury;
    }

    public static float GetAverageHealthOfPart(Pawn pawn, BodyPartRecord part)
    {
        var container = new List<float>();

        if (part.GetDirectChildParts()?.Count() > 0)
        {
            container.AddRange(part.GetDirectChildParts().Select(p => GetAverageHealthOfPart(pawn, p)));
        }
        else
        {
            container.Add(pawn.health.hediffSet.GetPartHealth(part) / part.def.GetMaxHealth(pawn));
        }

        return container.Average();
    }

    public static object? GetPawnHealable(Pawn pawn)
    {
        Hediff? hediff = FindLifeThreateningHediff(pawn);

        if (hediff is not null)
        {
            return hediff;
        }

        if (HealthUtility.TicksUntilDeathDueToBloodLoss(pawn) < 2500)
        {
            Hediff? hediff2 = FindMostBleedingHediff(pawn);

            if (hediff2 is not null)
            {
                return hediff2;
            }
        }

        if (pawn.health.hediffSet.GetBrain() is not null)
        {
            Hediff_Injury? injury = FindPermanentInjury(pawn, Gen.YieldSingle(pawn.health.hediffSet.GetBrain()) as IReadOnlyCollection<BodyPartRecord>);

            if (injury is not null)
            {
                return injury;
            }
        }

        BodyPartRecord? bodyPartRecord = FindBiggestMissingBodyPart(pawn, HandCoverageAbsWithChildren);

        if (bodyPartRecord is not null)
        {
            return bodyPartRecord;
        }

        Hediff_Injury? injury2 = FindPermanentInjury(
            pawn,
            pawn.health.hediffSet.GetNotMissingParts().Where(p => p.def == BodyPartDefOf.Eye) as IReadOnlyCollection<BodyPartRecord>
        );

        if (injury2 is not null)
        {
            return injury2;
        }

        Hediff? hediff3 = FindImmunizableHediffWhichCanKill(pawn);

        if (hediff3 is not null)
        {
            return hediff3;
        }

        Hediff? hediff4 = FindNonInjuryMiscBadHediff(pawn, true);

        if (hediff4 is not null)
        {
            return hediff4;
        }

        Hediff? hediff5 = FindNonInjuryMiscBadHediff(pawn, false);

        if (hediff5 is not null)
        {
            return hediff5;
        }

        if (pawn.health.hediffSet.GetBrain() is not null)
        {
            Hediff_Injury? injury3 = FindInjury(pawn, Gen.YieldSingle(pawn.health.hediffSet.GetBrain()) as IReadOnlyCollection<BodyPartRecord>);

            if (injury3 is not null)
            {
                return injury3;
            }
        }

        BodyPartRecord? bodyPartRecord2 = FindBiggestMissingBodyPart(pawn);

        if (bodyPartRecord2 is not null)
        {
            return bodyPartRecord2;
        }

        Hediff_Addiction? addiction = FindAddiction(pawn);

        if (addiction is not null)
        {
            return addiction;
        }

        Hediff_Injury? injury4 = FindPermanentInjury(pawn);

        return injury4 ?? FindInjury(pawn);
    }

    private static void Resurrect(this Pawn pawn)
    {
        try
        {
            ResurrectionUtility.ResurrectWithSideEffects(pawn);
        }
        catch (NullReferenceException)
        {
            TkUtils.Logger.Warn("Failed to revive with side effects -- falling back to a regular revive");
            ResurrectionUtility.Resurrect(pawn);
        }

        PawnTracker.pawnsToRevive.Remove(pawn);
    }

    public static bool TryResurrect(this Pawn pawn)
    {
        try
        {
            Pawn? val;

            if (pawn.SpawnedParentOrMe != pawn.Corpse && (val = pawn.SpawnedParentOrMe as Pawn) is not null
                && !val.carryTracker.TryDropCarriedThing(val.Position, (ThingPlaceMode)1, out Thing _))
            {
                TkUtils.Logger.Warn($"Could not drop {pawn} at {val.Position.ToString()} from {val.LabelShort}");

                return false;
            }

            pawn.ClearAllReservations();
            pawn.Resurrect();

            return true;
        }
        catch (Exception e)
        {
            TkUtils.Logger.Error($"Could not revive {pawn.LabelShort}", e);

            return false;
        }
    }
}
