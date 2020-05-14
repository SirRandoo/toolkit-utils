using System.Collections.Generic;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class HealAllHelper : IncidentHelper
    {
        private readonly List<Hediff> healQueue = new List<Hediff>();
        private readonly List<Pair<Pawn, BodyPartRecord>> restoreQueue = new List<Pair<Pawn, BodyPartRecord>>();

        public override bool IsPossible()
        {
            var entries = Find.ColonistBar.Entries;

            foreach (var e in entries)
            {
                var pawn = e.pawn;

                if (pawn.health.Dead)
                {
                    continue;
                }

                var result = HealHelper.GetPawnHealable(pawn);

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
            foreach (var hediff in healQueue)
            {
                HealHelper.Cure(hediff);
            }

            foreach (var part in restoreQueue)
            {
                part.First.health.RestorePart(part.Second);
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.Letters.MassHeal.Title".Translate(),
                "TKUtils.Letters.MassHeal.Description".Translate(),
                LetterDefOf.PositiveEvent
            );
        }
    }
}
