using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchToolkit.IncidentHelpers.Special;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class ReviveAllHelper : IncidentHelper
    {
        private List<Pawn> pawns;

        public override bool IsPossible()
        {
            var list = Find.ColonistBar
                .GetColonistsInOrder()
                .Where(p => p.Dead && p.SpawnedOrAnyParentSpawned && !PawnTracker.pawnsToRevive.Contains(p))
                .ToList();

            if (!list.Any())
            {
                return false;
            }

            pawns = list;
            foreach (var p in list)
            {
                PawnTracker.pawnsToRevive.Add(p);
            }

            return true;
        }

        public override void TryExecute()
        {
            var wasAnyRevived = false;
            foreach (var pawn in pawns)
            {
                try
                {
                    Pawn val;
                    if (pawn.SpawnedParentOrMe != pawn.Corpse
                        && (val = pawn.SpawnedParentOrMe as Pawn) != null
                        && !val.carryTracker.TryDropCarriedThing(val.Position, (ThingPlaceMode) 1, out var _))
                    {
                        TkLogger.Warn(
                            $"Submit this bug to ToolkitUtils' issue tracker: Could not drop {pawn} at {val.Position.ToString()} from {val}"
                        );
                    }
                    else
                    {
                        pawn.ClearAllReservations();

                        if (Androids.IsAndroid(pawn))
                        {
                            ResurrectionUtility.Resurrect(pawn);
                        }
                        else
                        {
                            ResurrectionUtility.ResurrectWithSideEffects(pawn);
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
                "TKUtils.Letters.MassRevival.Title".Translate(),
                "TKUtils.Letters.MassRevival.Description".Translate(),
                LetterDefOf.PositiveEvent
            );
        }
    }
}
