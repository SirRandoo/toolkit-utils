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

using System.Threading.Tasks;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class ChangeColor : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage message)
        {
            if (!PurchaseHelper.TryGetPawn(message.Username, out Pawn pawn))
            {
                message.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            CommandRelay.Tasks.Enqueue();
        }

        private static Task ChangeFavoriteColor(string username, [CanBeNull] Pawn pawn, Color color)
        {
            if (pawn == null)
            {

            }
        }
    }
}
