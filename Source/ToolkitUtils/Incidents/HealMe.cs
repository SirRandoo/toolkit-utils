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

using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class HealMe : IncidentVariablesBase
    {
        private Pawn _pawn;
        private Hediff _toHeal;
        private BodyPartRecord _toRestore;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            if (IncidentSettings.HealMe.FairFights && _pawn!.mindState.lastAttackTargetTick > 0 && Find.TickManager.TicksGame < _pawn.mindState.lastAttackTargetTick + 1800)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InCombat".Localize());

                return false;
            }

            object result = HealHelper.GetPawnHealable(_pawn!);

            switch (result)
            {
                case Hediff hediff:
                    _toHeal = hediff;

                    break;
                case BodyPartRecord record:
                    _toRestore = record;

                    break;
            }

            return _toHeal != null || _toRestore != null;
        }

        public override void Execute()
        {
            if (_toHeal != null)
            {
                HealHelper.Cure(_toHeal);
                Viewer.Charge(storeIncident);
                NotifySuccess(_toHeal.LabelCap);
            }

            if (_toRestore == null)
            {
                return;
            }

            _pawn.health.RestorePart(_toRestore);
            Viewer.Charge(storeIncident);
            NotifySuccess(_toRestore.Label);
        }

        private void NotifySuccess(string target)
        {
            if (ToolkitSettings.PurchaseConfirmations)
            {
                var response = "";

                if (_toHeal != null)
                {
                    response = "TKUtils.HealMe.Recovered";
                }

                if (_toRestore != null)
                {
                    response = "TKUtils.HealMe.Restored";
                }

                if (!response.NullOrEmpty())
                {
                    MessageHelper.ReplyToUser(Viewer.username, response.LocalizeKeyed(target));
                }
            }

            var description = "";

            if (_toHeal != null)
            {
                description = "TKUtils.HealLetter.RecoveredDescription";
            }

            if (_toRestore != null)
            {
                description = "TKUtils.HealLetter.RestoredDescription";
            }

            if (!description.NullOrEmpty())
            {
                Current.Game.letterStack.ReceiveLetter(
                    "TKUtils.HealLetter.Title".Localize(),
                    description.LocalizeKeyed(Viewer.username, target),
                    LetterDefOf.PositiveEvent,
                    _pawn
                );
            }
        }
    }
}
