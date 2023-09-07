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

namespace SirRandoo.ToolkitUtils.Ideology.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Dye : CommandBase
{
    private string _invoker;
    private Pawn _pawn;

    public override void RunCommand(ITwitchMessage twitchMessage)
    {
        _invoker = twitchMessage.Username;

        if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out _pawn))
        {
            twitchMessage.Reply("TKUtils.NoPawn".Localize());

            return;
        }

        string? hexcode = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrDefault();
        List<KeyValuePair<string, string>> apparelPairs = CommandParser.ParseKeyed(twitchMessage.Message);

        if (!apparelPairs.NullOrEmpty())
        {
            CommandRouter.MainThreadCommands.Enqueue(() => DyeApparel(apparelPairs));

            return;
        }

        if (string.IsNullOrEmpty(hexcode))
        {
            CommandRouter.MainThreadCommands.Enqueue(() => DyeAll(null));

            return;
        }

        string s = hexcode!.ToToolkit();

        if (!Data.ColorIndex.TryGetValue(s, out Color color) && !ColorUtility.TryParseHtmlString(s, out color))
        {
            twitchMessage.Reply("TKUtils.NotAColor".LocalizeKeyed(s));

            return;
        }

        CommandRouter.MainThreadCommands.Enqueue(() => DyeAll(new Color(color.r, color.g, color.b, 1f)));
    }

    private void DyeApparel(IEnumerable<KeyValuePair<string, string>> pairs)
    {
        List<Apparel> apparel = _pawn.apparel.WornApparel;

        foreach ((string? nameOrDef, string? colorCode) in pairs)
        {
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
                    MessageHelper.ReplyToUser(_invoker, "TKUtils.NotAColor".LocalizeKeyed(colorCode));

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

        MessageHelper.ReplyToUser(_invoker, "TKUtils.Dye.Complete".Localize());
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

        MessageHelper.ReplyToUser(_invoker, "TKUtils.Dye.Complete".Localize());
    }
}
