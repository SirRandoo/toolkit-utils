using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class HealRandom : IncidentHelper
    {
        private Pawn target;
        private Hediff toHeal;
        private BodyPartRecord toRestore;

        public override bool IsPossible()
        {
            List<Pawn> list = Find.ColonistBar.GetColonistsInOrder().Where(p => !p.Dead).ToList();

            if (!list.Any())
            {
                return false;
            }

            List<Pair<Pawn, object>> container = list
               .Select(p => new Pair<Pawn, object>(p, HealHelper.GetPawnHealable(p)))
               .Where(r => r.Second != null)
               .ToList();

            if (!container.Any())
            {
                return false;
            }

            Pair<Pawn, object> random = container.RandomElementWithFallback();

            if (random.First == null || random.Second == null)
            {
                return false;
            }

            target = random.First;

            switch (random.Second)
            {
                case Hediff hediff:
                    toHeal = hediff;
                    break;
                case BodyPartRecord record:
                    toRestore = record;
                    break;
            }

            return target != null && (toHeal != null || toRestore != null);
        }

        public override void TryExecute()
        {
            if (toHeal != null)
            {
                HealHelper.Cure(toHeal);

                Notify__Success(toHeal.LabelCap);
            }

            if (toRestore == null)
            {
                return;
            }

            target.health.RestorePart(toRestore);

            Notify__Success(toRestore.Label);
        }

        private void Notify__Success(string affected)
        {
            var description = "";

            if (toHeal != null)
            {
                description = "TKUtils.HealLetter.RecoveredDescription";
            }

            if (toRestore != null)
            {
                description = "TKUtils.HealLetter.RestoredDescription";
            }

            if (description.NullOrEmpty())
            {
                return;
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.HealLetter.Title".Localize(),
                description.Localize(target.LabelCap, affected),
                LetterDefOf.PositiveEvent,
                affected
            );
        }
    }
}
