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
using JetBrains.Annotations;
using RimWorld;
using TwitchToolkit.IncidentHelpers.Special;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class HealHelper
    {
        public static float HandCoverageAbsWithChildren =>
            ThingDefOf.Human.race.body.GetPartsWithDef(BodyPartDefOf.Hand).First().coverageAbsWithChildren;

        private static bool CanEverKill([NotNull] Hediff hediff)
        {
            return hediff.def.stages?.Any(s => s.lifeThreatening) ?? hediff.def.lethalSeverity >= 0f;
        }

        public static void Cure([NotNull] Hediff hediff)
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
                    LogHelper.Warn("HealHelper iterated too many times.");
                    break;
                }

                Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediff.def);

                if (firstHediffOfDef == null)
                {
                    break;
                }

                pawn.health.RemoveHediff(firstHediffOfDef);
            }
        }

        [CanBeNull]
        public static Hediff_Addiction FindAddiction([NotNull] Pawn pawn)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            foreach (Hediff h in hediffs)
            {
                if (h is Hediff_Addiction {Visible: true} a && a.def.everCurableByItem)
                {
                    return a;
                }
            }

            return null;
        }

        [CanBeNull]
        public static BodyPartRecord FindBiggestMissingBodyPart([NotNull] Pawn pawn, float minCoverage = 0f)
        {
            BodyPartRecord record = null;

            foreach (Hediff_MissingPart missing in pawn.health.hediffSet.GetMissingPartsCommonAncestors())
            {
                if (!(missing.Part.coverageAbsWithChildren < minCoverage)
                    && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(missing.Part)
                    && (record == null || missing.Part.coverageAbsWithChildren > record.coverageAbsWithChildren))
                {
                    record = missing.Part;
                }
            }

            return record;
        }

        [CanBeNull]
        public static Hediff FindImmunizableHediffWhichCanKill([NotNull] Pawn pawn)
        {
            Hediff hediff = null;
            float num = -1f;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            foreach (Hediff h in hediffs)
            {
                if (!h.Visible
                    || !h.def.everCurableByItem
                    || h.TryGetComp<HediffComp_Immunizable>() == null
                    || h.FullyImmune()
                    || !CanEverKill(h))
                {
                    continue;
                }

                float severity = h.Severity;

                if (hediff != null && !(severity > num))
                {
                    continue;
                }

                hediff = h;
                num = severity;
            }

            return hediff;
        }

        [CanBeNull]
        public static Hediff_Injury FindInjury(
            [NotNull] Pawn pawn,
            [CanBeNull] IReadOnlyCollection<BodyPartRecord> allowedBodyParts = null
        )
        {
            Hediff_Injury injury = null;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            foreach (Hediff h in hediffs)
            {
                if (h is Hediff_Injury {Visible: true} h2
                    && h2.def.everCurableByItem
                    && (allowedBodyParts == null || allowedBodyParts.Contains(h2.Part))
                    && (injury == null || h2.Severity > injury.Severity))
                {
                    injury = h2;
                }
            }

            return injury;
        }

        [CanBeNull]
        public static Hediff FindLifeThreateningHediff([NotNull] Pawn pawn)
        {
            Hediff hediff = null;
            float num = -1f;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            foreach (Hediff h in hediffs)
            {
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

                if (hediff != null && !(coverage > num))
                {
                    continue;
                }

                hediff = h;
                num = coverage;
            }

            return hediff;
        }

        [CanBeNull]
        public static Hediff FindMostBleedingHediff([NotNull] Pawn pawn)
        {
            var num = 0f;
            Hediff hediff = null;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            foreach (Hediff h in hediffs)
            {
                if (!h.Visible || !h.def.everCurableByItem)
                {
                    continue;
                }

                float bleedRate = h.BleedRate;

                if (!(bleedRate > 0f) || !(bleedRate > num) && hediff != null)
                {
                    continue;
                }

                num = bleedRate;
                hediff = h;
            }

            return hediff;
        }

        [CanBeNull]
        public static Hediff FindNonInjuryMiscBadHediff([NotNull] Pawn pawn, bool onlyIfCanKill)
        {
            Hediff hediff = null;
            var num = 1f;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            foreach (Hediff h in hediffs)
            {
                if (!h.Visible
                    || !h.def.isBad
                    || !h.def.everCurableByItem
                    || h is Hediff_Injury
                    || h is Hediff_MissingPart
                    || h is Hediff_Addiction
                    || h is Hediff_AddedPart
                    || onlyIfCanKill && !CanEverKill(h)
                    || (!h.def.GetModExtension<HealExtension>()?.ShouldHeal ?? false))
                {
                    continue;
                }

                if (h.def == HediffDefOf.BloodLoss || h.def == HediffDefOf.Malnutrition)
                {
                    continue;
                }

                float coverage = h.Part?.coverageAbsWithChildren ?? 999f;

                if (hediff != null && !(coverage > num))
                {
                    continue;
                }

                hediff = h;
                num = coverage;
            }

            return hediff;
        }

        [CanBeNull]
        public static Hediff_Injury FindPermanentInjury(
            [NotNull] Pawn pawn,
            [CanBeNull] IReadOnlyCollection<BodyPartRecord> allowedBodyParts = null
        )
        {
            Hediff_Injury injury = null;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            foreach (Hediff h in hediffs)
            {
                if (h is Hediff_Injury {Visible: true} h2
                    && h2.IsPermanent()
                    && h2.def.everCurableByItem
                    && (allowedBodyParts == null || allowedBodyParts.Contains(h2.Part))
                    && (injury == null || h2.Severity > injury.Severity)
                    && (h.def.GetModExtension<HealExtension>()?.ShouldHeal ?? true))
                {
                    injury = h2;
                }
            }

            return injury;
        }

        public static float GetAverageHealthOfPart(Pawn pawn, [NotNull] BodyPartRecord part)
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

        [CanBeNull]
        public static object GetPawnHealable([NotNull] Pawn pawn)
        {
            Hediff hediff = FindLifeThreateningHediff(pawn);
            if (hediff != null)
            {
                return hediff;
            }

            if (HealthUtility.TicksUntilDeathDueToBloodLoss(pawn) < 2500)
            {
                Hediff hediff2 = FindMostBleedingHediff(pawn);
                if (hediff2 != null)
                {
                    return hediff2;
                }
            }

            if (pawn.health.hediffSet.GetBrain() != null)
            {
                Hediff_Injury injury = FindPermanentInjury(
                    pawn,
                    Gen.YieldSingle(pawn.health.hediffSet.GetBrain()) as IReadOnlyCollection<BodyPartRecord>
                );
                if (injury != null)
                {
                    return injury;
                }
            }

            BodyPartRecord bodyPartRecord = FindBiggestMissingBodyPart(pawn, HandCoverageAbsWithChildren);
            if (bodyPartRecord != null)
            {
                return bodyPartRecord;
            }

            Hediff_Injury injury2 = FindPermanentInjury(
                pawn,
                pawn.health.hediffSet.GetNotMissingParts().Where(p => p.def == BodyPartDefOf.Eye) as
                    IReadOnlyCollection<BodyPartRecord>
            );
            if (injury2 != null)
            {
                return injury2;
            }

            Hediff hediff3 = FindImmunizableHediffWhichCanKill(pawn);
            if (hediff3 != null)
            {
                return hediff3;
            }

            Hediff hediff4 = FindNonInjuryMiscBadHediff(pawn, true);
            if (hediff4 != null)
            {
                return hediff4;
            }

            Hediff hediff5 = FindNonInjuryMiscBadHediff(pawn, false);
            if (hediff5 != null)
            {
                return hediff5;
            }

            if (pawn.health.hediffSet.GetBrain() != null)
            {
                Hediff_Injury injury3 = FindInjury(
                    pawn,
                    Gen.YieldSingle(pawn.health.hediffSet.GetBrain()) as IReadOnlyCollection<BodyPartRecord>
                );
                if (injury3 != null)
                {
                    return injury3;
                }
            }

            BodyPartRecord bodyPartRecord2 = FindBiggestMissingBodyPart(pawn);
            if (bodyPartRecord2 != null)
            {
                return bodyPartRecord2;
            }

            Hediff_Addiction addiction = FindAddiction(pawn);
            if (addiction != null)
            {
                return addiction;
            }

            Hediff_Injury injury4 = FindPermanentInjury(pawn);

            return injury4 ?? FindInjury(pawn);
        }

        public static void Resurrect(this Pawn pawn)
        {
            try
            {
                ResurrectionUtility.ResurrectWithSideEffects(pawn);
            }
            catch (NullReferenceException)
            {
                LogHelper.Warn("Failed to revive with side effects -- falling back to a regular revive");
                ResurrectionUtility.Resurrect(pawn);
            }

            PawnTracker.pawnsToRevive.Remove(pawn);
        }

        public static bool TryResurrect([NotNull] this Pawn pawn)
        {
            try
            {
                Pawn val;

                if (pawn.SpawnedParentOrMe != pawn.Corpse
                    && (val = pawn.SpawnedParentOrMe as Pawn) != null
                    && !val.carryTracker.TryDropCarriedThing(val.Position, (ThingPlaceMode) 1, out Thing _))
                {
                    LogHelper.Warn($"Could not drop {pawn} at {val.Position.ToString()} from {val.LabelShort}");
                    return false;
                }

                pawn.ClearAllReservations();
                pawn.Resurrect();
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error($"Could not revive {pawn.LabelShort}", e);
                return false;
            }
        }
    }
}
