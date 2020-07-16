using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class FullHeal : IncidentHelperVariables
    {
        private Pawn pawn;

        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            pawn = CommandBase.GetOrFindPawn(viewer.username);

            if (pawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            if (HealHelper.GetPawnHealable(pawn) == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NotInjured".Localize());
            }

            return true;
        }

        public override void TryExecute()
        {
            var healed = 0;
            var iterations = 0;
            while (true)
            {
                object injury = HealHelper.GetPawnHealable(pawn);

                if (injury == null)
                {
                    break;
                }

                switch (injury)
                {
                    case Hediff hediff:
                        HealHelper.Cure(hediff);
                        healed += 1;
                        break;
                    case BodyPartRecord record:
                        pawn.health.RestorePart(record);
                        healed += 1;
                        break;
                }

                iterations += 1;

                if (iterations < 500)
                {
                    continue;
                }

                TkLogger.Warn("Exceeded the maximum number of iterations during full heal.");
                return;
            }

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(storeIncident.cost * healed);
            }

            Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost * healed);

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.FullHeal.Complete".Localize(healed.ToString("N0")));
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.FullHealLetter.Title".Localize(),
                "TKUtils.FullHealLetter.Description".Localize(Viewer.username),
                LetterDefOf.PositiveEvent,
                pawn
            );
        }
    }
}
