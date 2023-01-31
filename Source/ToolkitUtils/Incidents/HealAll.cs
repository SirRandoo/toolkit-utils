// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    public class HealAll : IncidentVariablesBase
    {
        private readonly List<Hediff> _healQueue = new List<Hediff>();
        private readonly List<Pair<Pawn, BodyPartRecord>> _restoreQueue = new List<Pair<Pawn, BodyPartRecord>>();

        public override bool CanHappen(string msg, Viewer viewer)
        {
            foreach (Pawn pawn in Find.ColonistBar.GetColonistsInOrder().Where(p => !p.Dead))
            {
                if (IncidentSettings.HealAll.FairFights && pawn.mindState.lastAttackTargetTick > 0
                    && Find.TickManager.TicksGame < pawn.mindState.lastAttackTargetTick + 1800)
                {
                    continue;
                }

                object result = HealHelper.GetPawnHealable(pawn);

                switch (result)
                {
                    case Hediff hediff:
                        _healQueue.Add(hediff);

                        break;
                    case BodyPartRecord record:
                        _restoreQueue.Add(new Pair<Pawn, BodyPartRecord>(pawn, record));

                        break;
                }
            }

            return _healQueue.Any(i => i != null) || _restoreQueue.Any(i => i.Second != null);
        }

        public override void Execute()
        {
            foreach (Hediff hediff in _healQueue)
            {
                HealHelper.Cure(hediff);
            }

            foreach (Pair<Pawn, BodyPartRecord> part in _restoreQueue)
            {
                part.First.health.RestorePart(part.Second);
            }

            Viewer.Charge(storeIncident);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.MassHealLetter.Description".Localize());

            Find.LetterStack.ReceiveLetter("TKUtils.MassHealLetter.Title".Localize(), "TKUtils.MassHealLetter.Description".Localize(), LetterDefOf.PositiveEvent);
        }
    }
}
