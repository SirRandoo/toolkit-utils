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
        private List<Hediff> healQueue = new List<Hediff>();
        private List<Pair<Pawn, BodyPartRecord>> restoreQueue = new List<Pair<Pawn, BodyPartRecord>>();

        public override bool IsPossible()
        {
            foreach(var pawn in Find.ColonistBar.Entries.Select(e => e.pawn))
            {
                if(pawn.health.Dead)
                {
                    continue;
                }

                var result = GetPawnHealable(pawn);

                if(result is Hediff)
                {
                    healQueue.Add((Hediff) result);
                }
                else if(result is BodyPartRecord)
                {
                    restoreQueue.Add(new Pair<Pawn, BodyPartRecord>(pawn, (BodyPartRecord) result));
                }
            }

            return healQueue.Any(i => i != null) || restoreQueue.Any(i => i != null);
        }

        public override void TryExecute()
        {
            foreach(var hediff in healQueue)
            {
                HealHelper.Cure(hediff);
            }

            foreach(var part in restoreQueue)
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

        private object GetPawnHealable(Pawn pawn)
        {
            var hediff = HealHelper.FindLifeThreateningHediff(pawn);
            if(hediff != null)
            {
                return hediff;
            }

            if(HealthUtility.TicksUntilDeathDueToBloodLoss(pawn) < 2500)
            {
                var hediff2 = HealHelper.FindMostBleedingHediff(pawn);
                if(hediff2 != null)
                {
                    return hediff2;
                }
            }

            if(pawn.health.hediffSet.GetBrain() != null)
            {
                var injury = HealHelper.FindPermanentInjury(pawn, Gen.YieldSingle(pawn.health.hediffSet.GetBrain()));
                if(injury != null)
                {
                    return injury;
                }
            }

            var bodyPartRecord = HealHelper.FindBiggestMissingBodyPart(pawn, HealHelper.HandCoverageAbsWithChildren);
            if(bodyPartRecord != null)
            {
                return bodyPartRecord;
            }

            var injury2 = HealHelper.FindPermanentInjury(pawn, pawn.health.hediffSet.GetNotMissingParts().Where(p => p.def == BodyPartDefOf.Eye));
            if(injury2 != null)
            {
                return injury2;
            }

            var hediff3 = HealHelper.FindImmunizableHediffWhichCanKill(pawn);
            if(hediff3 != null)
            {
                return hediff3;
            }

            var hediff4 = HealHelper.FindNonInjuryMiscBadHediff(pawn, onlyIfCanKill: true);
            if(hediff4 != null)
            {
                return hediff4;
            }

            var hediff5 = HealHelper.FindNonInjuryMiscBadHediff(pawn, onlyIfCanKill: false);
            if(hediff5 != null)
            {
                return hediff5;
            }

            if(pawn.health.hediffSet.GetBrain() != null)
            {
                var injury3 = HealHelper.FindInjury(pawn, Gen.YieldSingle(pawn.health.hediffSet.GetBrain()));
                if(injury3 != null)
                {
                    return injury3;
                }
            }

            var bodyPartRecord2 = HealHelper.FindBiggestMissingBodyPart(pawn);
            if(bodyPartRecord2 != null)
            {
                return bodyPartRecord2;
            }

            var addiction = HealHelper.FindAddiction(pawn);
            if(addiction != null)
            {
                return addiction;
            }

            var injury4 = HealHelper.FindPermanentInjury(pawn);
            if(injury4 != null)
            {
                return injury4;
            }

            var injury5 = HealHelper.FindInjury(pawn);
            if(injury5 != null)
            {
                return injury5;
            }

            return null;
        }
    }
}
