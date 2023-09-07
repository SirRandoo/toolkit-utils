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
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.CommandSettings;

public class PawnRelations : ICommandSettings
{
    private string _minimumBuffer = TkSettings.OpinionMinimum.ToString();

    public void Draw(Rect region)
    {
        var listing = new Listing_Standard();

        listing.Begin(region);

        if (!TkSettings.MinimalRelations)
        {
            (Rect labelRect, Rect fieldRect) = listing.Split();
            UiHelper.Label(labelRect, "TKUtils.PawnRelations.OpinionThreshold.Label".TranslateSimple());
            Widgets.TextFieldNumeric(fieldRect, ref TkSettings.OpinionMinimum, ref _minimumBuffer);
            listing.DrawDescription("TKUtils.PawnRelations.OpinionThreshold.Description".TranslateSimple());
        }

        listing.CheckboxLabeled("TKUtils.PawnRelations.MinimalRelations.Label".TranslateSimple(), ref TkSettings.MinimalRelations);
        listing.DrawDescription("TKUtils.PawnRelations.MinimalRelations.Description".TranslateSimple());

        listing.End();
    }

    public void Save()
    {
        TkUtils.Instance.WriteSettings();
    }
}