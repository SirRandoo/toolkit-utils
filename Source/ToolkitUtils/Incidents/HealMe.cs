using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class HealMe : IncidentHelperVariables
    {
        private Pawn pawn;
        private Hediff toHeal;
        private BodyPartRecord toRestore;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            if (IncidentSettings.HealMe.FairFights
                && pawn.mindState.lastAttackTargetTick > 0
                && Find.TickManager.TicksGame < pawn.mindState.lastAttackTargetTick + 1800)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InCombat".Localize());
                return false;
            }

            object result = HealHelper.GetPawnHealable(pawn);

            switch (result)
            {
                case Hediff hediff:
                    toHeal = hediff;
                    break;
                case BodyPartRecord record:
                    toRestore = record;
                    break;
            }

            return toHeal != null || toRestore != null;
        }

        public override void TryExecute()
        {
            if (toHeal != null)
            {
                HealHelper.Cure(toHeal);

                if (!ToolkitSettings.UnlimitedCoins)
                {
                    Viewer.TakeViewerCoins(storeIncident.cost);
                }

                Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

                Notify__Success(toHeal.LabelCap);
            }

            if (toRestore == null)
            {
                return;
            }

            pawn.health.RestorePart(toRestore);

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(storeIncident.cost);
            }

            Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

            Notify__Success(toRestore.Label);
        }

        private void Notify__Success(string target)
        {
            if (ToolkitSettings.PurchaseConfirmations)
            {
                var response = "";

                if (toHeal != null)
                {
                    response = "TKUtils.HealMe.Recovered";
                }

                if (toRestore != null)
                {
                    response = "TKUtils.HealMe.Restored";
                }

                if (!response.NullOrEmpty())
                {
                    MessageHelper.ReplyToUser(Viewer.username, response.Localize(target));
                }
            }

            var description = "";

            if (toHeal != null)
            {
                description = "TKUtils.HealLetter.RecoveredDescription";
            }

            if (toRestore != null)
            {
                description = "TKUtils.HealLetter.RestoredDescription";
            }

            if (!description.NullOrEmpty())
            {
                Current.Game.letterStack.ReceiveLetter(
                    "TKUtils.HealLetter.Title".Localize(),
                    description.Localize(Viewer.username, target),
                    LetterDefOf.PositiveEvent,
                    pawn
                );
            }
        }
    }
}
