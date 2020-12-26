using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.Special;
using TwitchToolkit.Store;
using Verse;

#pragma warning disable 618

namespace SirRandoo.ToolkitUtils.Incidents
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class ReviveMe : IncidentHelperVariables
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

            if (PawnTracker.pawnsToRevive.Contains(pawn))
            {
                return false;
            }

            PawnTracker.pawnsToRevive.Add(pawn);
            return true;
        }

        public override void TryExecute()
        {
            try
            {
                Pawn val;
                if (pawn.SpawnedParentOrMe != pawn.Corpse
                    && (val = pawn.SpawnedParentOrMe as Pawn) != null
                    && !val.carryTracker.TryDropCarriedThing(val.Position, (ThingPlaceMode) 1, out Thing _))
                {
                    LogHelper.Warn(
                        $"Submit this bug to ToolkitUtils issue tracker: Could not drop {pawn} at {val.Position.ToString()} from {val}"
                    );
                    return;
                }

                if (!ToolkitSettings.UnlimitedCoins)
                {
                    Viewer.TakeViewerCoins(storeIncident.cost);
                }

                Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

                pawn.ClearAllReservations();

                try
                {
                    ResurrectionUtility.ResurrectWithSideEffects(pawn);
                }
                catch (NullReferenceException)
                {
                    LogHelper.Warn("Failed to revive with side effects!");
                    ResurrectionUtility.Resurrect(pawn);
                }

                PawnTracker.pawnsToRevive.Remove(pawn);
                Find.LetterStack.ReceiveLetter(
                    "TKUtils.RevivalLetter.Title".Localize(),
                    "TKUtils.RevivalLetter.Description".Localize(Viewer.username.CapitalizeFirst()),
                    LetterDefOf.PositiveEvent,
                    new LookTargets(pawn)
                );
            }
            catch (Exception ex)
            {
                LogHelper.Error("Could not execute reviveme", ex);
            }
        }
    }
}
