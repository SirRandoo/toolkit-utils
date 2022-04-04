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
using TwitchToolkit.IncidentHelpers.Special;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class ReviveMe : IncidentVariablesBase
    {
        private Pawn _pawn;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            if (PawnTracker.pawnsToRevive.Contains(_pawn))
            {
                return false;
            }

            PawnTracker.pawnsToRevive.Add(_pawn);

            return true;
        }

        public override void Execute()
        {
            _pawn.TryResurrect();
            Viewer.Charge(storeIncident);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.ReviveMe.Complete".Localize());

            Find.LetterStack.ReceiveLetter(
                "TKUtils.RevivalLetter.Title".Localize(),
                "TKUtils.RevivalLetter.Description".LocalizeKeyed(Viewer.username),
                LetterDefOf.PositiveEvent,
                _pawn
            );
        }
    }
}
