using System.Linq;
using JetBrains.Annotations;
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
            pawn = Find.ColonistBar.GetColonistsInOrder()
               .Where(p => p.Dead && p.SpawnedOrAnyParentSpawned && !PawnTracker.pawnsToRevive.Contains(p))
               .RandomElementWithFallback();

            if (pawn == null)
            {
                return false;
            }

            PawnTracker.pawnsToRevive.Add(pawn);
            return true;
        }

        public override void TryExecute()
        {
            pawn.TryResurrect();
        }
    }
}
