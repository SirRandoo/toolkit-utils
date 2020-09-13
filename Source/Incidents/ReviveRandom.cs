using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit.IncidentHelpers.Special;
using TwitchToolkit.Store;
using Verse;

#pragma warning disable 618

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class ReviveRandom : IncidentHelper
    {
        private Pawn pawn;

        public override bool IsPossible()
        {
            List<Pawn> list = Find.ColonistBar.GetColonistsInOrder()
               .Where(p => p.Dead && p.SpawnedOrAnyParentSpawned && !PawnTracker.pawnsToRevive.Contains(p))
               .ToList();

            if (!list.Any())
            {
                return false;
            }

            pawn = list.RandomElementWithFallback();

            if (pawn == null)
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
                    TkLogger.Warn(
                        $"Submit this bug to ToolkitUtils issue tracker: Could not drop {pawn} at {val.Position.ToString()} from {val}"
                    );
                    return;
                }

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
                    "TKUtils.RevivalLetter.Description".Localize(pawn.Name.ToStringShort),
                    LetterDefOf.PositiveEvent,
                    new LookTargets(pawn)
                );
            }
            catch (Exception ex)
            {
                TkLogger.Error("Could not execute reviveanypawn", ex);
            }
        }
    }
}
