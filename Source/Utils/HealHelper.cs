using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class HealHelper
    {
        public static float HandCoverageAbsWithChildren =>
            ThingDefOf.Human.race.body.GetPartsWithDef(BodyPartDefOf.Hand).First().coverageAbsWithChildren;

        private static bool CanEverKill(Hediff hediff)
        {
            return hediff.def.stages?.Any(s => s.lifeThreatening) ?? hediff.def.lethalSeverity >= 0f;
        }

        public static void Cure(Hediff hediff)
        {
            var pawn = hediff.pawn;
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
                    Logger.Warn("HealHelper iterated too many times.");
                    break;
                }

                var firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediff.def);

                if (firstHediffOfDef == null)
                {
                    break;
                }

                pawn.health.RemoveHediff(firstHediffOfDef);
            }
        }

        public static Hediff_Addiction FindAddiction(Pawn pawn)
        {
            var hediffs = pawn.health.hediffSet.hediffs;

            foreach (var h in hediffs)
            {
                if (h is Hediff_Addiction a && a.Visible && a.def.everCurableByItem)
                {
                    return a;
                }
            }

            return null;
        }

        public static BodyPartRecord FindBiggestMissingBodyPart(Pawn pawn, float minCoverage = 0f)
        {
            BodyPartRecord record = null;

            foreach (var missing in pawn.health.hediffSet.GetMissingPartsCommonAncestors())
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

        public static Hediff FindImmunizableHediffWhichCanKill(Pawn pawn)
        {
            Hediff hediff = null;
            var num = -1f;
            var hediffs = pawn.health.hediffSet.hediffs;

            foreach (var h in hediffs)
            {
                if (!h.Visible
                    || !h.def.everCurableByItem
                    || h.TryGetComp<HediffComp_Immunizable>() == null
                    || h.FullyImmune()
                    || !CanEverKill(h))
                {
                    continue;
                }

                var severity = h.Severity;

                if (hediff != null && !(severity > num))
                {
                    continue;
                }

                hediff = h;
                num = severity;
            }

            return hediff;
        }

        public static Hediff_Injury FindInjury(Pawn pawn, IEnumerable<BodyPartRecord> allowedBodyParts = null)
        {
            Hediff_Injury injury = null;
            var hediffs = pawn.health.hediffSet.hediffs;

            foreach (var h in hediffs)
            {
                if (h is Hediff_Injury h2
                    && h2.Visible
                    && h2.def.everCurableByItem
                    && (allowedBodyParts == null || allowedBodyParts.Contains(h2.Part))
                    && (injury == null || h2.Severity > injury.Severity))
                {
                    injury = h2;
                }
            }

            return injury;
        }

        public static Hediff FindLifeThreateningHediff(Pawn pawn)
        {
            Hediff hediff = null;
            var num = -1f;
            var hediffs = pawn.health.hediffSet.hediffs;

            foreach (var h in hediffs)
            {
                if (!h.Visible || !h.def.everCurableByItem || h.FullyImmune())
                {
                    continue;
                }

                var lifeThreatening = h.CurStage?.lifeThreatening ?? false;
                var lethal = h.def.lethalSeverity >= 0f && h.Severity / h.def.lethalSeverity >= 0.8f;

                if (!lifeThreatening && !lethal)
                {
                    continue;
                }

                var coverage = h.Part?.coverageAbsWithChildren ?? 999f;

                if (hediff != null && !(coverage > num))
                {
                    continue;
                }

                hediff = h;
                num = coverage;
            }

            return hediff;
        }

        public static Hediff FindMostBleedingHediff(Pawn pawn)
        {
            var num = 0f;
            Hediff hediff = null;
            var hediffs = pawn.health.hediffSet.hediffs;

            foreach (var h in hediffs)
            {
                if (!h.Visible || !h.def.everCurableByItem)
                {
                    continue;
                }

                var bleedRate = h.BleedRate;

                if (!(bleedRate > 0f) || (!(bleedRate > num) && hediff != null))
                {
                    continue;
                }

                num = bleedRate;
                hediff = h;
            }

            return hediff;
        }

        public static Hediff FindNonInjuryMiscBadHediff(Pawn pawn, bool onlyIfCanKill)
        {
            Hediff hediff = null;
            var num = 1f;
            var hediffs = pawn.health.hediffSet.hediffs;

            foreach (var h in hediffs)
            {
                if (h.Visible
                    && h.def.isBad
                    && h.def.everCurableByItem
                    && !(h is Hediff_Injury)
                    && !(h is Hediff_MissingPart)
                    && !(h is Hediff_Addiction)
                    && !(h is Hediff_AddedPart)
                    && (!onlyIfCanKill || CanEverKill(h)))
                {
                    var coverage = h.Part?.coverageAbsWithChildren ?? 999f;

                    if (hediff != null && !(coverage > num))
                    {
                        continue;
                    }

                    hediff = h;
                    num = coverage;
                }
            }

            return hediff;
        }

        public static Hediff_Injury FindPermanentInjury(Pawn pawn, IEnumerable<BodyPartRecord> allowedBodyParts = null)
        {
            Hediff_Injury injury = null;
            var hediffs = pawn.health.hediffSet.hediffs;

            foreach (var h in hediffs)
            {
                if (h is Hediff_Injury h2
                    && h2.Visible
                    && h2.IsPermanent()
                    && h2.def.everCurableByItem
                    && (allowedBodyParts == null || allowedBodyParts.Contains(h2.Part))
                    && (injury == null || h2.Severity > injury.Severity))
                {
                    injury = h2;
                }
            }

            return injury;
        }
    }
}
