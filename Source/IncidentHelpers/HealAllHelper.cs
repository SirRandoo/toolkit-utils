using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class HealAllHelper : IncidentHelper
    {
        private readonly List<Hediff> healQueue = new List<Hediff>();
        private readonly List<Pair<Pawn, BodyPartRecord>> restoreQueue = new List<Pair<Pawn, BodyPartRecord>>();

        public override bool IsPossible()
        {
            var entries = Find.ColonistBar.Entries;

            foreach (var e in entries)
            {
                var pawn = e.pawn;

                if (pawn.health.Dead)
                {
                    continue;
                }

                var result = GetPawnHealable(pawn);

                switch (result)
                {
                    case Hediff hediff:
                        healQueue.Add(hediff);
                        break;
                    case BodyPartRecord record:
                        restoreQueue.Add(new Pair<Pawn, BodyPartRecord>(pawn, record));
                        break;
                }
            }

            return healQueue.Any(i => i != null) || restoreQueue.Any(i => i.Second != null);
        }

        public override void TryExecute()
        {
            foreach (var hediff in healQueue)
            {
                HealHelper.Cure(hediff);
            }

            foreach (var part in restoreQueue)
            {
                part.First.health.RestorePart(part.Second);
            }

            Viewer.TakeViewerCoins(storeIncident.cost);
            Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

            Find.LetterStack.ReceiveLetter(
                "TKUtils.Letters.MassHeal.Title".Translate(),
                "TKUtils.Letters.MassHeal.Description".Translate(),
                LetterDefOf.PositiveEvent
            );
        }

        private static object GetPawnHealable(Pawn pawn)
        {
            var hediff = HealHelper.FindLifeThreateningHediff(pawn);
            if (hediff != null)
            {
                return hediff;
            }

            if (HealthUtility.TicksUntilDeathDueToBloodLoss(pawn) < 2500)
            {
                var hediff2 = HealHelper.FindMostBleedingHediff(pawn);
                if (hediff2 != null)
                {
                    return hediff2;
                }
            }

            if (pawn.health.hediffSet.GetBrain() != null)
            {
                var injury = HealHelper.FindPermanentInjury(pawn, Gen.YieldSingle(pawn.health.hediffSet.GetBrain()));
                if (injury != null)
                {
                    return injury;
                }
            }

            var bodyPartRecord = HealHelper.FindBiggestMissingBodyPart(pawn, HealHelper.HandCoverageAbsWithChildren);
            if (bodyPartRecord != null)
            {
                return bodyPartRecord;
            }

            var injury2 = HealHelper.FindPermanentInjury(
                pawn,
                pawn.health.hediffSet.GetNotMissingParts().Where(p => p.def == BodyPartDefOf.Eye)
            );
            if (injury2 != null)
            {
                return injury2;
            }

            var hediff3 = HealHelper.FindImmunizableHediffWhichCanKill(pawn);
            if (hediff3 != null)
            {
                return hediff3;
            }

            var hediff4 = HealHelper.FindNonInjuryMiscBadHediff(pawn, true);
            if (hediff4 != null)
            {
                return hediff4;
            }

            var hediff5 = HealHelper.FindNonInjuryMiscBadHediff(pawn, false);
            if (hediff5 != null)
            {
                return hediff5;
            }

            if (pawn.health.hediffSet.GetBrain() != null)
            {
                var injury3 = HealHelper.FindInjury(pawn, Gen.YieldSingle(pawn.health.hediffSet.GetBrain()));
                if (injury3 != null)
                {
                    return injury3;
                }
            }

            var bodyPartRecord2 = HealHelper.FindBiggestMissingBodyPart(pawn);
            if (bodyPartRecord2 != null)
            {
                return bodyPartRecord2;
            }

            var addiction = HealHelper.FindAddiction(pawn);
            if (addiction != null)
            {
                return addiction;
            }

            var injury4 = HealHelper.FindPermanentInjury(pawn);

            return injury4 ?? HealHelper.FindInjury(pawn);
        }
    }
}
