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

using System;
using System.Linq;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.CommandSettings;

public class PawnLeave : ICommandSettings
{
    private readonly FloatMenu _leaveMethods;
    private string _currentLeaveMethodText;

    public PawnLeave()
    {
        _leaveMethods = new FloatMenu(
            Enum.GetNames(typeof(LeaveMethod))
               .Select(
                    n => new FloatMenuOption(
                        $"TKUtils.Abandon.Method.{n}".TranslateSimple(),
                        () =>
                        {
                            TkSettings.LeaveMethod = n;
                            _currentLeaveMethodText = $"TKUtils.Abandon.Method.{TkSettings.LeaveMethod}".TranslateSimple();
                        }
                    )
                )
               .ToList()
        );

        _currentLeaveMethodText = $"TKUtils.Abandon.Method.{TkSettings.LeaveMethod}".TranslateSimple();
    }

    public void Draw(Rect region)
    {
        var listing = new Listing_Standard();

        listing.Begin(region);

        (Rect labelRect, Rect fieldRect) = listing.GetRect(Text.LineHeight * 1.5f).Split();
        UiHelper.Label(labelRect, "TKUtils.Abandon.Method.Label".TranslateSimple());
        listing.DrawDescription("TKUtils.Abandon.Method.Description".TranslateSimple());

        if (Widgets.ButtonText(fieldRect, _currentLeaveMethodText))
        {
            Find.WindowStack.Add(_leaveMethods);
        }

        if (!TkSettings.LeaveMethod.EqualsIgnoreCase(nameof(LeaveMethod.Thanos)))
        {
            listing.CheckboxLabeled("TKUtils.Abandon.Gear.Label".TranslateSimple(), ref TkSettings.DropInventory);
            listing.DrawDescription("TKUtils.Abandon.Gear.Description".TranslateSimple());
        }

        listing.End();
    }

    public void Save()
    {
        TkUtils.Instance.WriteSettings();
    }
}