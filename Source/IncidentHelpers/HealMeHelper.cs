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
        private Hediff toHeal;
        private BodyPartRecord toRestore;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            Viewer = viewer;

            pawn = CommandBase.GetPawn(viewer.username);

            if (pawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.NoPawn".Translate());
                return false;
            }

            var hediff = HealHelper.FindLifeThreateningHediff(pawn);
            if (hediff != null)
            {
                toHeal = hediff;
                return true;
            }

            if (HealthUtility.TicksUntilDeathDueToBloodLoss(pawn) < 2500)
            {
                var hediff2 = HealHelper.FindMostBleedingHediff(pawn);
                if (hediff2 != null)
                {
                    toHeal = hediff2;
                    return true;
                }
            }

            if (pawn.health.hediffSet.GetBrain() != null)
            {
                var hediffInjury = HealHelper.FindPermanentInjury(
                    pawn,
                    Gen.YieldSingle(pawn.health.hediffSet.GetBrain())
                );
                if (hediffInjury != null)
                {
                    toHeal = hediffInjury;
                    return true;
                }
            }

            var bodyPartRecord = HealHelper.FindBiggestMissingBodyPart(pawn, HealHelper.HandCoverageAbsWithChildren);
            if (bodyPartRecord != null)
            {
                toRestore = bodyPartRecord;
                return true;
            }

            var hediffInjury2 = HealHelper.FindPermanentInjury(
                pawn,
                pawn.health.hediffSet.GetNotMissingParts()
                    .Where(p => p.def == BodyPartDefOf.Eye)
            );

            if (hediffInjury2 != null)
            {
                toHeal = hediffInjury2;
                return true;
            }

            var hediff3 = HealHelper.FindImmunizableHediffWhichCanKill(pawn);
            if (hediff3 != null)
            {
                toHeal = hediff3;
                return true;
            }

            var hediff4 = HealHelper.FindNonInjuryMiscBadHediff(pawn, true);
            if (hediff4 != null)
            {
                toHeal = hediff4;
                return true;
            }

            var hediff5 = HealHelper.FindNonInjuryMiscBadHediff(pawn, false);
            if (hediff5 != null)
            {
                toHeal = hediff5;
                return true;
            }

            if (pawn.health.hediffSet.GetBrain() != null)
            {
                var hediffInjury3 = HealHelper.FindInjury(pawn, Gen.YieldSingle(pawn.health.hediffSet.GetBrain()));
                if (hediffInjury3 != null)
                {
                    toHeal = hediffInjury3;
                    return true;
                }
            }

            var bodyPartRecord2 = HealHelper.FindBiggestMissingBodyPart(pawn);
            if (bodyPartRecord2 != null)
            {
                toRestore = bodyPartRecord2;
                return true;
            }

            var hediffAddiction = HealHelper.FindAddiction(pawn);
            if (hediffAddiction != null)
            {
                toHeal = hediffAddiction;
                return true;
            }

            var hediffInjury4 = HealHelper.FindPermanentInjury(pawn);
            if (hediffInjury4 != null)
            {
                toHeal = hediffInjury4;
                return true;
            }

            var hediffInjury5 = HealHelper.FindInjury(pawn);
            if (hediffInjury5 != null)
            {
                toHeal = hediffInjury5;
            }

            return toHeal != null || toRestore != null;
        }

        public override void TryExecute()
        {
            if (toHeal != null)
            {
                HealHelper.Cure(toHeal);

                Viewer.TakeViewerCoins(storeIncident.cost);
                Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

                NotifySuccess(toHeal.LabelCap);
            }

            if (toRestore == null)
            {
                return;
            }

            pawn.health.RestorePart(toRestore);

            Viewer.TakeViewerCoins(storeIncident.cost);
            Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

            NotifySuccess(toRestore.LabelCap);
        }

        private void NotifySuccess(string target)
        {
            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Responses.HealMe.Healed".Translate(target));
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.Letters.Heal.Title".Translate(),
                "TKUtils.Letters.Heal.Description".Translate(Viewer.username, target),
                LetterDefOf.PositiveEvent,
                new LookTargets(CommandBase.GetPawn(Viewer.username))
            );
        }
    }
}
