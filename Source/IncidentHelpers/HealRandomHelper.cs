using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class HealRandomHelper : IncidentHelper
    {
        private Pawn target;
        private Hediff toHeal;
        private BodyPartRecord toRestore;

        public override bool IsPossible()
        {
            var list = Find.ColonistBar
                .GetColonistsInOrder()
                .Where(p => !p.Dead)
                .ToList();

            if (!list.Any())
            {
                return false;
            }

            var container = list
                .Select(p => new Pair<Pawn, object>(p, HealHelper.GetPawnHealable(p)))
                .Where(r => r.Second != null)
                .ToList();

            if (!container.Any())
            {
                return false;
            }

            var random = container.RandomElementWithFallback();

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

                if (!ToolkitSettings.UnlimitedCoins)
                {
                    Viewer.TakeViewerCoins(storeIncident.cost);
                }

                Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

                Notify__Success(toHeal.LabelCap);
            }

            if (toRestore == null)
            {
                return;
            }

            target.health.RestorePart(toRestore);

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(storeIncident.cost);
            }

            Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

            Notify__Success(toRestore.Label);
        }

        private void Notify__Success(string affected)
        {
            var description = "";

            if (toHeal != null)
            {
                description = "TKUtils.Letters.Heal.Recovery.Description";
            }

            if (toRestore != null)
            {
                description = "TKUtils.Letters.Heal.Restored.Description";
            }

            if (description.NullOrEmpty())
            {
                return;
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.Letters.Heal.Title".Translate(),
                description.Translate(target.LabelCap, affected),
                LetterDefOf.PositiveEvent,
                affected
            );
        }
    }
}
