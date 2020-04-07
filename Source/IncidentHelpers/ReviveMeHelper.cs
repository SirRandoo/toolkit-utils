using System;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.Special;
using TwitchToolkit.Store;
using Verse;

#pragma warning disable 618

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class ReviveMeHelper : IncidentHelperVariables
    {
        private Pawn pawn;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            Viewer = viewer;

            var viewerPawn = CommandBase.GetOrFindPawn(viewer.username);

            if (viewerPawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.NoPawn".Translate());
                return false;
            }

            if (PawnTracker.pawnsToRevive.Contains(viewerPawn))
            {
                return false;
            }

            pawn = viewerPawn;
            return true;
        }

        public override void TryExecute()
        {
            try
            {
                Pawn val;
                if (pawn.SpawnedParentOrMe != pawn.Corpse
                    && (val = pawn.SpawnedParentOrMe as Pawn) != null
                    && !val.carryTracker.TryDropCarriedThing(val.Position, (ThingPlaceMode) 1, out var _))
                {
                    Logger.Warn(
                        $"Submit this bug to ToolkitUtils issue tracker: Could not drop {pawn} at {val.Position.ToString()} from {val}"
                    );
                }
                else
                {
                    Viewer.TakeViewerCoins(storeIncident.cost);
                    Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

                    pawn.ClearAllReservations();
                    ResurrectionUtility.ResurrectWithSideEffects(pawn);
                    PawnTracker.pawnsToRevive.Remove(pawn);
                    Find.LetterStack.ReceiveLetter(
                        "TKUtils.Letters.Revival.Title".Translate(),
                        "TKUtils.Letters.Revival.Description".Translate(pawn.Name),
                        LetterDefOf.PositiveEvent,
                        new LookTargets(pawn)
                    );
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not execute reviveme", ex);
            }
        }
    }
}
