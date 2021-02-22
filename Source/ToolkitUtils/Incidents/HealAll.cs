﻿using System.Collections.Generic;
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
    public class HealAll : IncidentHelperVariables
    {
        private readonly List<Hediff> healQueue = new List<Hediff>();
        private readonly List<Pair<Pawn, BodyPartRecord>> restoreQueue = new List<Pair<Pawn, BodyPartRecord>>();

        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            foreach (Pawn pawn in Find.ColonistBar.GetColonistsInOrder().Where(p => !p.Dead))
            {
                if (IncidentSettings.HealAll.FairFights
                    && pawn.mindState.lastAttackTargetTick > 0
                    && Find.TickManager.TicksGame < pawn.mindState.lastAttackTargetTick + 1800)
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

            Viewer.Charge(storeIncident);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.MassHealLetter.Description".Localize());

            Find.LetterStack.ReceiveLetter(
                "TKUtils.MassHealLetter.Title".Localize(),
                "TKUtils.MassHealLetter.Description".Localize(),
                LetterDefOf.PositiveEvent
            );
        }
    }
}