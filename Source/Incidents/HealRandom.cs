using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class HealRandom : IncidentHelperVariables
    {
        private Pawn target;
        private Hediff toHeal;
        private BodyPartRecord toRestore;

        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            List<Pawn> list = Find.ColonistBar.GetColonistsInOrder().Where(p => !p.Dead).ToList();

            if (!list.Any())
            {
                return false;
            }

            if (IncidentSettings.HealRandom.FairFights)
            {
                list = list.Where(
                        p => p.mindState.lastAttackTargetTick > 0
                             && Find.TickManager.TicksGame < p.mindState.lastAttackTargetTick + 1800
                    )
                   .ToList();
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
            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(storeIncident.cost);
            }

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

            string descriptionTranslated = description.Localize(target.LabelShort.CapitalizeFirst(), affected);
            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, descriptionTranslated);
                ;
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.HealLetter.Title".Localize(),
                descriptionTranslated,
                LetterDefOf.PositiveEvent,
                target
            );
        }
    }
}
