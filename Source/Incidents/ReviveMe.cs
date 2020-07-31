﻿using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.Special;
using TwitchToolkit.Store;
using Verse;

#pragma warning disable 618

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class ReviveMe : IncidentHelperVariables
    {
        private Pawn pawn;
        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            Viewer = viewer;

            Pawn viewerPawn = CommandBase.GetOrFindPawn(viewer.username);

            if (viewerPawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            if (PawnTracker.pawnsToRevive.Contains(viewerPawn))
            {
                return false;
            }

            pawn = viewerPawn;
            PawnTracker.pawnsToRevive.Add(viewerPawn);
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
                    TkLogger.Warn(
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
                    TkLogger.Warn("Failed to revive with side effects!");
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
                TkLogger.Error("Could not execute reviveme", ex);
            }
        }
    }
}