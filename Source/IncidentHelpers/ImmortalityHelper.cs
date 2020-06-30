using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    [UsedImplicitly]
    public class ImmortalityHelper : IncidentHelperVariables
    {
        private Pawn pawn;

        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            Viewer = viewer;
            pawn = CommandBase.GetOrFindPawn(Viewer.username);

            if (pawn == null)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Responses.NoPawn".TranslateSimple());
                return false;
            }

            if (!Immortals.Active)
            {
                return false;
            }

            return !pawn.health.hediffSet.HasHediff(Immortals.ImmortalHediffDef);
        }

        public override void TryExecute()
        {
            if (!Immortals.TryGrantImmortality(pawn))
            {
                return;
            }

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(storeIncident.cost);
            }

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Responses.Immortality".TranslateSimple());
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.Letters.Immortality.Title".TranslateSimple(),
                "TKUtils.Letters.Immortality.Description".Translate(Viewer.username),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }
    }
}
