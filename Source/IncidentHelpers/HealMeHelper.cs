using System.Linq;

using RimWorld;

using SirRandoo.ToolkitUtils.Utils;

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

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            Viewer = viewer;
            this.separateChannel = separateChannel;

            pawn = CommandBase.GetPawn(viewer.username);

            if(pawn == null)
            {
                CommandBase.SendCommandMessage(
                    viewer.username,
                    "TKUtils.Responses.NoPawn".Translate()
                );
                return false;
            }

            var hediff = HealHelper.FindLifeThreateningHediff(pawn);
            if(hediff != null)
            {
                toHeal = hediff;
                return true;
            }
            if(HealthUtility.TicksUntilDeathDueToBloodLoss(pawn) < 2500)
            {
                var hediff2 = HealHelper.FindMostBleedingHediff(pawn);
                if(hediff2 != null)
                {
                    toHeal = hediff2;
                    return true;
                }
            }
            if(pawn.health.hediffSet.GetBrain() != null)
            {
                var hediff_Injury = HealHelper.FindPermanentInjury(pawn, Gen.YieldSingle(pawn.health.hediffSet.GetBrain()));
                if(hediff_Injury != null)
                {
                    toHeal = hediff_Injury;
                    return true;
                }
            }
            var bodyPartRecord = HealHelper.FindBiggestMissingBodyPart(pawn, HealHelper.HandCoverageAbsWithChildren);
            if(bodyPartRecord != null)
            {
                toRestore = bodyPartRecord;
                return true;
            }
            var hediff_Injury2 = HealHelper.FindPermanentInjury(
                pawn,
                pawn.health.hediffSet.GetNotMissingParts()
                    .Where(p => p.def == BodyPartDefOf.Eye)
            );

            if(hediff_Injury2 != null)
            {
                toHeal = hediff_Injury2;
                return true;
            }
            var hediff3 = HealHelper.FindImmunizableHediffWhichCanKill(pawn);
            if(hediff3 != null)
            {
                toHeal = hediff3;
                return true;
            }
            var hediff4 = HealHelper.FindNonInjuryMiscBadHediff(pawn, onlyIfCanKill: true);
            if(hediff4 != null)
            {
                toHeal = hediff4;
                return true;
            }
            var hediff5 = HealHelper.FindNonInjuryMiscBadHediff(pawn, onlyIfCanKill: false);
            if(hediff5 != null)
            {
                toHeal = hediff5;
                return true;
            }
            if(pawn.health.hediffSet.GetBrain() != null)
            {
                var hediff_Injury3 = HealHelper.FindInjury(pawn, Gen.YieldSingle(pawn.health.hediffSet.GetBrain()));
                if(hediff_Injury3 != null)
                {
                    toHeal = hediff_Injury3;
                    return true;
                }
            }
            var bodyPartRecord2 = HealHelper.FindBiggestMissingBodyPart(pawn);
            if(bodyPartRecord2 != null)
            {
                toRestore = bodyPartRecord2;
                return true;
            }
            var hediff_Addiction = HealHelper.FindAddiction(pawn);
            if(hediff_Addiction != null)
            {
                toHeal = hediff_Addiction;
                return true;
            }
            var hediff_Injury4 = HealHelper.FindPermanentInjury(pawn);
            if(hediff_Injury4 != null)
            {
                toHeal = hediff_Injury4;
                return true;
            }
            var hediff_Injury5 = HealHelper.FindInjury(pawn);
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
                HealHelper.Cure(toHeal);

                NotifySuccess(toHeal.LabelCap);
            }

            if(toRestore != null)
            {
                pawn.health.RestorePart(toRestore);

                NotifySuccess(toRestore.LabelCap);
            }
        }

        private void NotifySuccess(string target)
        {
            if(ToolkitSettings.PurchaseConfirmations)
            {
                CommandBase.SendCommandMessage(
                    Viewer.username,
                    "TKUtils.Responses.HealMe.Healed".Translate(
                        target.Named("TARGET")
                    )
                );
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.Letters.Heal.Title".Translate(),
                "TKUtils.Letters.Heal.Description".Translate(
                    Viewer.username.Named("VIEWER"),
                    target.Named("TARGET")
                ),
                LetterDefOf.PositiveEvent,
                new LookTargets(CommandBase.GetPawn(Viewer.username))
            );
        }
    }
}
