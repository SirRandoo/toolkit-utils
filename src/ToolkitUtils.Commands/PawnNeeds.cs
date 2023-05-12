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
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnNeeds : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".TranslateSimple());

                return;
            }

            if (pawn!.needs?.AllNeeds == null)
            {
                twitchMessage.Reply("TKUtils.PawnNeeds.None".TranslateSimple().WithHeader("TabNeeds".TranslateSimple()));

                return;
            }

            twitchMessage.Reply(
                pawn.needs.AllNeeds.Select(n => ResponseHelper.JoinPair(n.LabelCap, n.CurLevelPercentage.ToStringPercent())).SectionJoin().WithHeader("TabNeeds".TranslateSimple())
            );
        }
    }
}
