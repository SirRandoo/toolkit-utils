// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using Steamworks;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Drawers;

public static class GroupDrawer
{
    /// <summary>Draws a header for a group within a listing.</summary>
    /// <param name="listing">The listing in which the header will be drawn.</param>
    /// <param name="name">The name of the group to display as a header.</param>
    /// <param name="gap">Indicates whether a gap should be added before the header.</param>
    public static void DrawGroupHeader(this Listing listing, string name, bool gap = true)
    {
        if (gap) listing.Gap(UiConstants.LineHeight);

        LabelDrawer.DrawLabel(listing.GetRect(Text.LineHeight), name, TextAnchor.LowerLeft, GameFont.Tiny);
        listing.GapLine(6f);
    }

    /// <summary>Draws a header for a mod group within a listing, including mod details and actions.</summary>
    /// <param name="listing">The listing in which the mod group header will be drawn.</param>
    /// <param name="modName">The name of the mod displayed as the header title.</param>
    /// <param name="modId">The unique mod identifier used for workshop interactions.</param>
    /// <param name="gap">Determines whether to add spacing before the header.</param>
    public static void DrawModGroupHeader(this Listing listing, string modName, ulong modId, bool gap = true)
    {
        if (gap) listing.Gap(UiConstants.LineHeight);

        Rect lineRect = listing.GetRect(Text.LineHeight);
        LabelDrawer.DrawLabel(lineRect, modName, TextAnchor.LowerLeft, GameFont.Tiny);

        string modRequirementString = string.Format(UxLocale.ModDependsOn, modName);

        Text.Font = GameFont.Tiny;
        float width = Text.CalcSize(modRequirementString).x;
        var modRequirementRect = new Rect(lineRect.x + lineRect.width - width, lineRect.y, width, Text.LineHeight);
        Text.Font = GameFont.Small;

        LabelDrawer.DrawLabel(lineRect, modRequirementString, UxColors.RedishPink, TextAnchor.LowerRight, GameFont.Tiny);

        Widgets.DrawHighlightIfMouseover(modRequirementRect);

        if (Widgets.ButtonInvisible(modRequirementRect)) SteamUtility.OpenWorkshopPage(new PublishedFileId_t(modId));

        listing.GapLine(6f);
    }
}
