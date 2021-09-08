// ToolkitUtils.Ideology
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
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class Dye : CommandBase
    {
        private string invoker;
        private Pawn pawn;

        public override void RunCommand([NotNull] ITwitchMessage message)
        {
            invoker = message.Username;

            if (!PurchaseHelper.TryGetPawn(message.Username, out pawn))
            {
                message.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            string hexcode = CommandFilter.Parse(message.Message).Skip(1).FirstOrDefault();
            List<KeyValuePair<string, string>> apparelPairs = CommandParser.ParseKeyed(message.Message);

            if (!apparelPairs.NullOrEmpty())
            {
                DyeApparel(apparelPairs);
                message.Reply("TKUtils.Dye.Complete".Localize());
                return;
            }

            if (hexcode.NullOrEmpty()
                || !Data.ColorIndex.TryGetValue(hexcode!.ToToolkit(), out Color color)
                || !ColorUtility.TryParseHtmlString(hexcode.ToToolkit(), out color))
            {
                return;
            }

            DyeAll(color);
            message.Reply("TKUtils.Dye.Complete".Localize());
        }

        private void DyeApparel([NotNull] IEnumerable<KeyValuePair<string, string>> pairs)
        {
            List<Apparel> apparel = pawn.apparel.WornApparel;

            foreach ((string nameOrDef, string colorCode) in pairs)
            {
                string colorCodeTransformed = colorCode.ToToolkit();
                if (colorCode.NullOrEmpty()
                    || !Data.ColorIndex.TryGetValue(colorCodeTransformed, out Color color)
                    || !ColorUtility.TryParseHtmlString(colorCodeTransformed, out color))
                {
                    MessageHelper.ReplyToUser(invoker, "TKUtils.NotAColor".LocalizeKeyed(colorCode));
                    continue;
                }

                Apparel item = apparel.Find(
                    a =>
                    {
                        string toolkit = nameOrDef.ToToolkit();
                        return a.def.label.ToToolkit().EqualsIgnoreCase(toolkit)
                               || a.def.defName.EqualsIgnoreCase(toolkit);
                    }
                );

                item?.TryGetComp<CompColorable>()?.SetColor(color);
            }
        }

        private void DyeAll(Color color)
        {
            foreach (Apparel apparel in pawn.apparel.WornApparel)
            {
                apparel.TryGetComp<CompColorable>()?.SetColor(color);
            }
        }
    }
}
