using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public class HealAll : IncidentHelperVariables
    {
        private readonly List<Hediff> healQueue = new List<Hediff>();
        private readonly List<Pair<Pawn, BodyPartRecord>> restoreQueue = new List<Pair<Pawn, BodyPartRecord>>();

        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            foreach (Pawn pawn in Find.ColonistBar.GetColonistsInOrder())
            {
                if (pawn.health.Dead)
                {
                    continue;
                }

                if (IncidentSettings.HealAll.FairFights
                    && (Find.TickManager.TicksGame < pawn.mindState.lastAttackTargetTick + 1800
                        || pawn.mindState.lastAttackTargetTick <= 0))
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

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(storeIncident.cost);
            }

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.MassHealLetter.Description".Localize());
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.MassHealLetter.Title".Localize(),
                "TKUtils.MassHealLetter.Description".Localize(),
                LetterDefOf.PositiveEvent
            );
        }
    }
}
