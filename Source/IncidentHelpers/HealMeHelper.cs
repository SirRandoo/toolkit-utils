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

            var result = HealHelper.GetPawnHealable(pawn);

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
                    response = "TKUtils.Responses.HealMe.Healed";
                }

                if (toRestore != null)
                {
                    response = "TKUtils.Responses.HealMe.Restored";
                }

                if (!response.NullOrEmpty())
                {
                    MessageHelper.ReplyToUser(Viewer.username, response.Translate(target));
                }
            }

            var description = "";

            if (toHeal != null)
            {
                description = "TKUtils.Letters.Heal.Recovery.Description";
            }

            if (toRestore != null)
            {
                description = "TKUtils.Letters.Heal.Restored.Description";
            }

            if (!description.NullOrEmpty())
            {
                Current.Game.letterStack.ReceiveLetter(
                    "TKUtils.Letters.Heal.Title".Translate(),
                    description.Translate(Viewer.username, target),
                    LetterDefOf.PositiveEvent,
                    pawn
                );
            }
        }
    }
}
