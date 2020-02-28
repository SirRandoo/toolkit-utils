using System.Collections.Generic;
using System.Linq;

using RimWorld;

using SirRandoo.ToolkitUtils.Commands;

using TwitchToolkit;
using TwitchToolkit.Store;

using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class HealMeHelper : IncidentHelperVariables
    {
        private Pawn pawn;
        private bool separateChannel;
        private Hediff toHeal;
        private BodyPartRecord toRestore;
        public override Viewer Viewer { get; set; }
        private float HandCoverageAbsWithChildren => ThingDefOf.Human.race.body.GetPartsWithDef(BodyPartDefOf.Hand).First().coverageAbsWithChildren;

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            Viewer = viewer;
            this.separateChannel = separateChannel;

            pawn = CommandBase.GetPawn(viewer.username);

            if(pawn == null)
            {
                CommandBase.SendMessage("TKUtils.Responses.NoPawn".Translate(), separateChannel);
                return false;
            }

            var hediff = FindLifeThreateningHediff(pawn);
            if(hediff != null)
            {
                toHeal = hediff;
                return true;
            }
            if(HealthUtility.TicksUntilDeathDueToBloodLoss(pawn) < 2500)
            {
                var hediff2 = FindMostBleedingHediff(pawn);
                if(hediff2 != null)
                {
                    toHeal = hediff2;
                    return true;
                }
            }
            if(pawn.health.hediffSet.GetBrain() != null)
            {
                var hediff_Injury = FindPermanentInjury(pawn, Gen.YieldSingle(pawn.health.hediffSet.GetBrain()));
                if(hediff_Injury != null)
                {
                    toHeal = hediff_Injury;
                    return true;
                }
            }
            var bodyPartRecord = FindBiggestMissingBodyPart(pawn, HandCoverageAbsWithChildren);
            if(bodyPartRecord != null)
            {
                toRestore = bodyPartRecord;
                return true;
            }
            var hediff_Injury2 = FindPermanentInjury(pawn, from x in pawn.health.hediffSet.GetNotMissingParts()
                                                           where x.def == BodyPartDefOf.Eye
                                                           select x);
            if(hediff_Injury2 != null)
            {
                toHeal = hediff_Injury2;
                return true;
            }
            var hediff3 = FindImmunizableHediffWhichCanKill(pawn);
            if(hediff3 != null)
            {
                toHeal = hediff3;
                return true;
            }
            var hediff4 = FindNonInjuryMiscBadHediff(pawn, onlyIfCanKill: true);
            if(hediff4 != null)
            {
                toHeal = hediff4;
                return true;
            }
            var hediff5 = FindNonInjuryMiscBadHediff(pawn, onlyIfCanKill: false);
            if(hediff5 != null)
            {
                toHeal = hediff5;
                return true;
            }
            if(pawn.health.hediffSet.GetBrain() != null)
            {
                var hediff_Injury3 = FindInjury(pawn, Gen.YieldSingle(pawn.health.hediffSet.GetBrain()));
                if(hediff_Injury3 != null)
                {
                    toHeal = hediff_Injury3;
                    return true;
                }
            }
            var bodyPartRecord2 = FindBiggestMissingBodyPart(pawn);
            if(bodyPartRecord2 != null)
            {
                toRestore = bodyPartRecord2;
                return true;
            }
            var hediff_Addiction = FindAddiction(pawn);
            if(hediff_Addiction != null)
            {
                toHeal = hediff_Addiction;
                return true;
            }
            var hediff_Injury4 = FindPermanentInjury(pawn);
            if(hediff_Injury4 != null)
            {
                toHeal = hediff_Injury4;
                return true;
            }
            var hediff_Injury5 = FindInjury(pawn);
            if(hediff_Injury5 != null)
            {
                toHeal = hediff_Injury5;
            }

            return toHeal != null || toRestore != null;
        }

        public override void TryExecute()
        {
            if(toHeal != null)
            {
                Cure(toHeal);
            }

            if(toRestore != null)
            {
                pawn.health.RestorePart(toRestore);
            }
        }

        private bool CanEverKill(Hediff hediff)
        {
            if(hediff.def.stages != null)
            {
                for(int i = 0; i < hediff.def.stages.Count; i++)
                {
                    if(hediff.def.stages[i].lifeThreatening)
                    {
                        return true;
                    }
                }
            }
            return hediff.def.lethalSeverity >= 0f;
        }

        private void Cure(Hediff hediff)
        {
            var pawn = hediff.pawn;
            pawn.health.RemoveHediff(hediff);
            if(hediff.def.cureAllAtOnceIfCuredByItem)
            {
                int num = 0;
                while(true)
                {
                    num++;
                    if(num > 10000)
                    {
                        Log.Error("Too many iterations.");
                        break;
                    }
                    var firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediff.def);
                    if(firstHediffOfDef == null)
                    {
                        break;
                    }
                    pawn.health.RemoveHediff(firstHediffOfDef);
                }
            }
            Messages.Message("MessageHediffCuredByItem".Translate(hediff.LabelBase.CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
        }

        private Hediff_Addiction FindAddiction(Pawn pawn)
        {
            var hediffs = pawn.health.hediffSet.hediffs;
            for(int i = 0; i < hediffs.Count; i++)
            {
                if(hediffs[i] is Hediff_Addiction hediff_Addiction && hediff_Addiction.Visible && hediff_Addiction.def.everCurableByItem)
                {
                    return hediff_Addiction;
                }
            }
            return null;
        }

        private BodyPartRecord FindBiggestMissingBodyPart(Pawn pawn, float minCoverage = 0f)
        {
            BodyPartRecord bodyPartRecord = null;
            foreach(var missingPartsCommonAncestor in pawn.health.hediffSet.GetMissingPartsCommonAncestors())
            {
                if(!(missingPartsCommonAncestor.Part.coverageAbsWithChildren < minCoverage) && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(missingPartsCommonAncestor.Part) && (bodyPartRecord == null || missingPartsCommonAncestor.Part.coverageAbsWithChildren > bodyPartRecord.coverageAbsWithChildren))
                {
                    bodyPartRecord = missingPartsCommonAncestor.Part;
                }
            }
            return bodyPartRecord;
        }

        private Hediff FindImmunizableHediffWhichCanKill(Pawn pawn)
        {
            Hediff hediff = null;
            float num = -1f;
            var hediffs = pawn.health.hediffSet.hediffs;
            for(int i = 0; i < hediffs.Count; i++)
            {
                if(hediffs[i].Visible && hediffs[i].def.everCurableByItem && hediffs[i].TryGetComp<HediffComp_Immunizable>() != null && !hediffs[i].FullyImmune() && CanEverKill(hediffs[i]))
                {
                    float severity = hediffs[i].Severity;
                    if(hediff == null || severity > num)
                    {
                        hediff = hediffs[i];
                        num = severity;
                    }
                }
            }
            return hediff;
        }

        private Hediff_Injury FindInjury(Pawn pawn, IEnumerable<BodyPartRecord> allowedBodyParts = null)
        {
            Hediff_Injury hediff_Injury = null;
            var hediffs = pawn.health.hediffSet.hediffs;
            for(int i = 0; i < hediffs.Count; i++)
            {
                if(hediffs[i] is Hediff_Injury hediff_Injury2 && hediff_Injury2.Visible && hediff_Injury2.def.everCurableByItem && (allowedBodyParts == null || allowedBodyParts.Contains(hediff_Injury2.Part)) && (hediff_Injury == null || hediff_Injury2.Severity > hediff_Injury.Severity))
                {
                    hediff_Injury = hediff_Injury2;
                }
            }
            return hediff_Injury;
        }

        private Hediff FindLifeThreateningHediff(Pawn pawn)
        {
            Hediff hediff = null;
            float num = -1f;
            var hediffs = pawn.health.hediffSet.hediffs;
            for(int i = 0; i < hediffs.Count; i++)
            {
                if(!hediffs[i].Visible || !hediffs[i].def.everCurableByItem || hediffs[i].FullyImmune())
                {
                    continue;
                }
                bool num2 = hediffs[i].CurStage?.lifeThreatening ?? false;
                bool flag = hediffs[i].def.lethalSeverity >= 0f && hediffs[i].Severity / hediffs[i].def.lethalSeverity >= 0.8f;
                if(num2 || flag)
                {
                    float num3 = (hediffs[i].Part != null) ? hediffs[i].Part.coverageAbsWithChildren : 999f;
                    if(hediff == null || num3 > num)
                    {
                        hediff = hediffs[i];
                        num = num3;
                    }
                }
            }
            return hediff;
        }

        private Hediff FindMostBleedingHediff(Pawn pawn)
        {
            float num = 0f;
            Hediff hediff = null;
            var hediffs = pawn.health.hediffSet.hediffs;
            for(int i = 0; i < hediffs.Count; i++)
            {
                if(hediffs[i].Visible && hediffs[i].def.everCurableByItem)
                {
                    float bleedRate = hediffs[i].BleedRate;
                    if(bleedRate > 0f && (bleedRate > num || hediff == null))
                    {
                        num = bleedRate;
                        hediff = hediffs[i];
                    }
                }
            }
            return hediff;
        }

        private Hediff FindNonInjuryMiscBadHediff(Pawn pawn, bool onlyIfCanKill)
        {
            Hediff hediff = null;
            float num = -1f;
            var hediffs = pawn.health.hediffSet.hediffs;
            for(int i = 0; i < hediffs.Count; i++)
            {
                if(hediffs[i].Visible && hediffs[i].def.isBad && hediffs[i].def.everCurableByItem && !(hediffs[i] is Hediff_Injury) && !(hediffs[i] is Hediff_MissingPart) && !(hediffs[i] is Hediff_Addiction) && !(hediffs[i] is Hediff_AddedPart) && (!onlyIfCanKill || CanEverKill(hediffs[i])))
                {
                    float num2 = (hediffs[i].Part != null) ? hediffs[i].Part.coverageAbsWithChildren : 999f;
                    if(hediff == null || num2 > num)
                    {
                        hediff = hediffs[i];
                        num = num2;
                    }
                }
            }
            return hediff;
        }

        private Hediff_Injury FindPermanentInjury(Pawn pawn, IEnumerable<BodyPartRecord> allowedBodyParts = null)
        {
            Hediff_Injury hediff_Injury = null;
            var hediffs = pawn.health.hediffSet.hediffs;
            for(int i = 0; i < hediffs.Count; i++)
            {
                if(hediffs[i] is Hediff_Injury hediff_Injury2 && hediff_Injury2.Visible && hediff_Injury2.IsPermanent() && hediff_Injury2.def.everCurableByItem && (allowedBodyParts == null || allowedBodyParts.Contains(hediff_Injury2.Part)) && (hediff_Injury == null || hediff_Injury2.Severity > hediff_Injury.Severity))
                {
                    hediff_Injury = hediff_Injury2;
                }
            }
            return hediff_Injury;
        }
    }
}
