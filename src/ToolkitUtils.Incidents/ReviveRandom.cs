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

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Helpers;
using TwitchToolkit.IncidentHelpers.Special;
using TwitchToolkit.Store;
using Verse;

#pragma warning disable 618

namespace ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class ReviveRandom : IncidentHelper
    {
        private Pawn _pawn;

        public override bool IsPossible()
        {
            _pawn = Find.ColonistBar.GetColonistsInOrder()
               .Where(p => p.Dead && p.SpawnedOrAnyParentSpawned && !PawnTracker.pawnsToRevive.Contains(p))
               .RandomElementWithFallback();

            if (_pawn == null)
            {
                return false;
            }

            PawnTracker.pawnsToRevive.Add(_pawn);

            return true;
        }

        public override void TryExecute()
        {
            _pawn.TryResurrect();

            Find.LetterStack.ReceiveLetter(
                "TKUtils.RevivalLetter.Title".TranslateSimple(),
                "TKUtils.RevivalLetter.Description".Translate((_pawn.LabelShort ?? _pawn.Label).CapitalizeFirst()),
                LetterDefOf.PositiveEvent,
                _pawn
            );
        }
    }
}
