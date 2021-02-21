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
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class ReviveAll : IncidentHelper
    {
        private List<Pawn> pawns;

        public override bool IsPossible()
        {
            List<Pawn> list = Find.ColonistBar.GetColonistsInOrder()
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
            if (!pawns.Any(HealHelper.TryResurrect))
            {
                return;
            }

            Viewer.Charge(storeIncident);

            Find.LetterStack.ReceiveLetter(
                "TKUtils.MassRevivalLetter.Title".Localize(),
                "TKUtils.MassRevivalLetter.Description".Localize(),
                LetterDefOf.PositiveEvent
            );
        }
    }
}
