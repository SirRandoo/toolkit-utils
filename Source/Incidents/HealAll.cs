using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class HealAll : IncidentHelper
    {
        private readonly List<Hediff> healQueue = new List<Hediff>();
        private readonly List<Pair<Pawn, BodyPartRecord>> restoreQueue = new List<Pair<Pawn, BodyPartRecord>>();

        public override bool IsPossible()
        {
            foreach (Pawn pawn in Find.ColonistBar.GetColonistsInOrder())
            {
                if (pawn.health.Dead)
                {
                    continue;
                }

                object result = HealHelper.GetPawnHealable(pawn);

                switch (result)
                {
                    case Hediff hediff:
                        healQueue.Add(hediff);
                        break;
                    case BodyPartRecord record:
                        restoreQueue.Add(new Pair<Pawn, BodyPartRecord>(pawn, record));
                        break;
                }
            }

            return healQueue.Any(i => i != null) || restoreQueue.Any(i => i.Second != null);
        }

        public override void TryExecute()
        {
            foreach (Hediff hediff in healQueue)
            {
                HealHelper.Cure(hediff);
            }

            foreach (Pair<Pawn, BodyPartRecord> part in restoreQueue)
            {
                part.First.health.RestorePart(part.Second);
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.MassHealLetter.Title".Localize(),
                "TKUtils.MassHealLetter.Description".Localize(),
                LetterDefOf.PositiveEvent
            );
        }
    }
}
