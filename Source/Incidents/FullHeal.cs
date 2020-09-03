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
                return false;
            }

            return true;
        }

        public override void TryExecute()
        {
            var healed = 0;
            var iterations = 0;
            while (true)
            {
                if (!Viewer.CanAfford(storeIncident.cost))
                {
                    break;
                }

                object healable = HealHelper.GetPawnHealable(pawn);

                if (healable == null)
                {
                    break;
                }

                healed = Heal(healable, healed);
                iterations += 1;

                if (iterations < 500)
                {
                    continue;
                }

                TkLogger.Warn("Exceeded the maximum number of iterations during full heal.");
                break;
            }

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(
                    Viewer.username,
                    healed > 1
                        ? "TKUtils.FullHeal.CompletePlural".Localize(healed.ToString("N0"))
                        : "TKUtils.FullHeal.Complete".Localize()
                );
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.FullHealLetter.Title".Localize(),
                "TKUtils.FullHealLetter.Description".Localize(Viewer.username),
                LetterDefOf.PositiveEvent,
                pawn
            );
        }

        private int Heal(object injury, int healed)
        {
            switch (injury)
            {
                case Hediff hediff:
                    HealHelper.Cure(hediff);
                    healed += 1;

                    if (!ToolkitSettings.UnlimitedCoins)
                    {
                        Viewer.TakeViewerCoins(storeIncident.cost);
                    }

                    Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost * healed);
                    break;
                case BodyPartRecord record:
                    pawn.health.RestorePart(record);
                    healed += 1;

                    if (!ToolkitSettings.UnlimitedCoins)
                    {
                        Viewer.TakeViewerCoins(storeIncident.cost);
                    }

                    Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost * healed);
                    break;
            }

            return healed;
        }
    }
}
