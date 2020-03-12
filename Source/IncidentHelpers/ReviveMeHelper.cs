using System;

using RimWorld;

using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.Special;
using TwitchToolkit.Store;

using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class ReviveMeHelper : IncidentHelperVariables
    {
        private Pawn pawn;
        private bool separateChannel;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            Viewer = viewer;
            this.separateChannel = separateChannel;

            var pawn = CommandBase.GetPawnDestructive(viewer.username);

            if(pawn == null)
            {
                CommandBase.SendCommandMessage(
                    viewer.username,
                    "TKUtils.Responses.NoPawn".Translate()
                );
                return false;
            }

            if(PawnTracker.pawnsToRevive.Contains(pawn))
            {
                return false;
            }

            this.pawn = pawn;
            return true;
        }

        public override void TryExecute()
        {
            try
            {
                Pawn val;
                if(pawn.SpawnedParentOrMe != pawn.Corpse && (val = (pawn.SpawnedParentOrMe as Pawn)) != null && !val.carryTracker.TryDropCarriedThing(val.Position, (ThingPlaceMode) 1, out var val2, null))
                {
                    CommandBase.Error($"Submit this bug to ToolkitUtils issue tracker: Could not drop {pawn} at {val.Position} from {val}");
                }
                else
                {
                    pawn.ClearAllReservations(true);
                    ResurrectionUtility.ResurrectWithSideEffects(pawn);
                    PawnTracker.pawnsToRevive.Remove(pawn);
                    Find.LetterStack.ReceiveLetter("Pawn Revived", $"{pawn.Name} has been revived but is experiencing some side effects.", LetterDefOf.PositiveEvent, new LookTargets(pawn), null);
                }
            }
            catch(Exception ex)
            {
                CommandBase.Error("Submit this bug to ToolkitUtils issue tracker: " + ex.Message);
            }
        }
    }
}
