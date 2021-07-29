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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class SetFavoriteColor : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage message)
        {
            string code = CommandFilter.Parse(message.Message).Skip(1).FirstOrDefault();

            if (code.NullOrEmpty())
            {
                return;
            }

            if (!Data.ColorIndex.TryGetValue(code!.ToLowerInvariant(), out Color color)
                && !ColorUtility.TryParseHtmlString(code, out color))
            {
                MessageHelper.ReplyToUser(message.Username, "TKUtils.NotAColor".LocalizeKeyed(code));
                return;
            }

            if (!PurchaseHelper.TryGetPawn(message.Username, out Pawn pawn))
            {
                MessageHelper.ReplyToUser(message.Username, "TKUtils.NoPawn".Localize());
                return;
            }

            pawn.story.favoriteColor = color;
        }
    }
}
