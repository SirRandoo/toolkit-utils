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
    public class HealRandom : IncidentVariablesBase
    {
        private Pawn _target;
        private Hediff _toHeal;
        private BodyPartRecord _toRestore;

        public override bool CanHappen(string msg, Viewer viewer)
        {
            List<Pawn> pawns = Find.ColonistBar.GetColonistsInOrder()
               .Where(p => !p.Dead)
               .Where(
                    pawn => !IncidentSettings.HealRandom.FairFights
                        || (pawn.mindState.lastAttackTargetTick > 0 && Find.TickManager.TicksGame <= pawn.mindState.lastAttackTargetTick + 1800)
                )
               .ToList();

            if (!pawns.Select(p => new Pair<Pawn, object>(p, HealHelper.GetPawnHealable(p))).Where(r => r.Second != null).TryRandomElement(out Pair<Pawn, object> random))
            {
                return false;
            }

            if (random.First == null || random.Second == null)
            {
                return false;
            }

            _target = random.First;

            switch (random.Second)
            {
                case Hediff hediff:
                    _toHeal = hediff;

                    break;
                case BodyPartRecord record:
                    _toRestore = record;

                    break;
            }

            return _target != null && (_toHeal != null || _toRestore != null);
        }

        public override void Execute()
        {
            if (_toHeal != null)
            {
                HealHelper.Cure(_toHeal);

                NotifySuccess(_toHeal.LabelCap);
            }

            if (_toRestore == null)
            {
                return;
            }

            _target.health.RestorePart(_toRestore);

            NotifySuccess(_toRestore.Label);
        }

        private void NotifySuccess(string affected)
        {
            Viewer.Charge(storeIncident);

            var description = "";

            if (_toHeal != null)
            {
                description = "TKUtils.HealLetter.RecoveredDescription";
            }

            if (_toRestore != null)
            {
                description = "TKUtils.HealLetter.RestoredDescription";
            }

            if (description.NullOrEmpty())
            {
                return;
            }

            string descriptionTranslated = description.LocalizeKeyed(_target.LabelShort.CapitalizeFirst(), affected);
            MessageHelper.SendConfirmation(Viewer.username, descriptionTranslated);

            Current.Game.letterStack.ReceiveLetter("TKUtils.HealLetter.Title".Localize(), descriptionTranslated, LetterDefOf.PositiveEvent, _target);
        }
    }
}
