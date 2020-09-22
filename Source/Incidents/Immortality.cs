using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class Immortality : IncidentHelperVariables
    {
        private Pawn pawn;

        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
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
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Immortality".Localize());
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.ImmortalityLetter.Title".Localize(),
                "TKUtils.ImmortalityLetter.Description".Localize(Viewer.username),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }
    }
}
