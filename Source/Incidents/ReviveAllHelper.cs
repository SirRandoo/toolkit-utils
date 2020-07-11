using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit.IncidentHelpers.Special;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class ReviveAllHelper : IncidentHelper
    {
        private List<Pawn> pawns;

        public override bool IsPossible()
        {
            List<Pawn> list = Find.ColonistBar
                .GetColonistsInOrder()
                .Where(p => p.Dead && p.SpawnedOrAnyParentSpawned && !PawnTracker.pawnsToRevive.Contains(p))
                .ToList();

            if (!list.Any())
            {
                return false;
            }

            pawns = list;
            foreach (Pawn p in list)
            {
                PawnTracker.pawnsToRevive.Add(p);
            }

            return true;
        }

        public override void TryExecute()
        {
            var wasAnyRevived = false;
            foreach (Pawn pawn in pawns)
            {
                try
                {
                    Pawn val;
                    if (pawn.SpawnedParentOrMe != pawn.Corpse
                        && (val = pawn.SpawnedParentOrMe as Pawn) != null
                        && !val.carryTracker.TryDropCarriedThing(val.Position, (ThingPlaceMode) 1, out Thing _))
                    {
                        TkLogger.Warn(
                            $"Submit this bug to ToolkitUtils' issue tracker: Could not drop {pawn} at {val.Position.ToString()} from {val}"
                        );
                    }
                    else
                    {
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
                        wasAnyRevived = true;
                    }
                }
                catch (Exception ex)
                {
                    TkLogger.Error($"Could not revive {pawn.LabelCap}", ex);
                }
            }

            if (!wasAnyRevived)
            {
                return;
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.MassRevivalLetter.Title".Localize(),
                "TKUtils.MassRevivalLetter.Description".Localize(),
                LetterDefOf.PositiveEvent
            );
        }
    }
}
