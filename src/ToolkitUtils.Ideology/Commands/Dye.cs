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
using ToolkitCore.Utilities;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Ideology.Commands
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class Dye : CommandBase
    {
        private string _invoker;
        private Pawn _pawn;

        public override void RunCommand([NotNull] ITwitchMessage message)
        {
            _invoker = message.Username;

            if (!PurchaseHelper.TryGetPawn(message.Username, out _pawn))
            {
                message.Reply("TKUtils.NoPawn".TranslateSimple());

                return;
            }

            string hexcode = CommandFilter.Parse(message.Message).Skip(1).FirstOrDefault();
            List<KeyValuePair<string, string>> apparelPairs = CommandParser.ParseKeyed(message.Message);

            if (!apparelPairs.NullOrEmpty())
            {
                CommandRouter.MainThreadCommands.Enqueue(() => DyeApparel(apparelPairs));

                return;
            }

            if (hexcode.NullOrEmpty())
            {
                CommandRouter.MainThreadCommands.Enqueue(() => DyeAll(null));

                return;
            }

            string s = hexcode!.ToToolkit();

            if (!Data.ColorIndex.TryGetValue(s, out Color color) && !ColorUtility.TryParseHtmlString(s, out color))
            {
                message.Reply("TKUtils.NotAColor".Translate(s));

                return;
            }

            CommandRouter.MainThreadCommands.Enqueue(() => DyeAll(new Color(color.r, color.g, color.b, 1f)));
        }

        private void DyeApparel([NotNull] IEnumerable<KeyValuePair<string, string>> pairs)
        {
            List<Apparel> apparel = _pawn.apparel.WornApparel;

            foreach (KeyValuePair<string, string> pair in pairs)
            {
                string nameOrDef = pair.Key;
                string colorCode = pair.Value;

                string colorCodeTransformed = colorCode.ToToolkit();

                Color? color;

                if (colorCode.NullOrEmpty())
                {
                    color = _pawn.story.favoriteColor;
                }
                else
                {
                    if (!Data.ColorIndex.TryGetValue(colorCodeTransformed, out Color color2) && !ColorUtility.TryParseHtmlString(colorCodeTransformed, out color2))
                    {
                        MessageHelper.ReplyToUser(_invoker, "TKUtils.NotAColor".Translate(colorCode));

                        return;
                    }

                    color = new Color(color2.r, color2.g, color2.b, 1f);
                }

                if (!color.HasValue)
                {
                    continue;
                }

                Apparel item = apparel.Find(
                    a =>
                    {
                        string toolkit = nameOrDef.ToToolkit();

                        return a.def.label.ToToolkit().EqualsIgnoreCase(toolkit) || a.def.defName.EqualsIgnoreCase(toolkit);
                    }
                );

                item?.TryGetComp<CompColorable>()?.SetColor(color.Value);
            }

            MessageHelper.ReplyToUser(_invoker, "TKUtils.Dye.Complete".TranslateSimple());
        }

        private void DyeAll(Color? color)
        {
            color ??= _pawn.story.favoriteColor;

            if (!color.HasValue)
            {
                return;
            }

            foreach (Apparel apparel in _pawn.apparel.WornApparel)
            {
                apparel.TryGetComp<CompColorable>()?.SetColor(color.Value);
            }

            MessageHelper.ReplyToUser(_invoker, "TKUtils.Dye.Complete".TranslateSimple());
        }
    }
}
