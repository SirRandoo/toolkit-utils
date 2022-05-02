// ToolkitUtils
// Copyright (C) 2022  SirRandoo
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

using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class Divorce : ConsensualCommand
    {
        /// <inheritdoc/>
        protected override void ProcessAcceptInternal(string asker, string askee)
        {
            if (!PurchaseHelper.TryGetPawn(asker, out Pawn askerPawn))
            {
                MessageHelper.ReplyToUser(asker, "TKUtils.NoPawn".LocalizeKeyed(asker));

                return;
            }

            if (!PurchaseHelper.TryGetPawn(askee, out Pawn askeePawn))
            {
                MessageHelper.ReplyToUser(asker, "TKUtils.PawnNotFound".LocalizeKeyed(askee));

                return;
            }

            TkUtils.Context.Post(c => PerformDivorce(askerPawn, askeePawn), null);
        }

        private static void PerformDivorce(Pawn askerPawn, Pawn askeePawn)
        {
            SpouseRelationUtility.DoDivorce(askerPawn, askeePawn);
        }
    }
}
