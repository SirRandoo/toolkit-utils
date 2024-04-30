// MIT License
//
// Copyright (c) 2024 SirRandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using JetBrains.Annotations;
using SirRandoo.ToolkitUtils;
using Steamworks;
using UnityEngine;
using Verse;

namespace ToolkitUtils.UX;

public static class UiHelper
{
    private static readonly Color ActiveTabColor = new(0.46f, 0.49f, 0.5f);
    private static readonly Color InactiveTabColor = new(0.21f, 0.23f, 0.24f);
    private static readonly Color TableHeaderColor = new(0.62f, 0.65f, 0.66f);


    /// <summary>
    ///     Draws a sort indicator
    /// </summary>
    /// <param name="parentRegion"></param>
    /// <param name="order"></param>
    public static void SortIndicator(Rect parentRegion, SortOrder order)
    {
        Rect region = LayoutHelper.IconRect(
            parentRegion.x + parentRegion.width - parentRegion.height + 3f,
            parentRegion.y + 8f,
            parentRegion.height - 9f,
            parentRegion.height - 16f, 0f
        );

        switch (order)
        {
            case SortOrder.Ascending:
                GUI.DrawTexture(region, Textures.SortingAscend);

                return;
            case SortOrder.Descending:
                GUI.DrawTexture(region, Textures.SortingDescend);

                return;
        }
    }


    /// <summary>
    ///     Draws a background suitable for a tab.
    /// </summary>
    /// <param name="region">The region to draw the background in</param>
    /// <param name="vertical">
    ///     Whether or not to draw the background
    ///     vertically
    /// </param>
    /// <param name="active">
    ///     Whether or not the associated tab is the active
    ///     tab
    /// </param>
    public static void DrawTabBackground(Rect region, bool vertical = false, bool active = false)
    {
        if (vertical)
        {
            region.y += region.width;
            GUIUtility.RotateAroundPivot(-90f, region.position);
        }

        GUI.color = active ? ActiveTabColor : InactiveTabColor;
        Widgets.DrawHighlight(region);
        GUI.color = Color.white;

        if (!active && Mouse.IsOver(region))
        {
            Widgets.DrawLightHighlight(region);
        }

        if (vertical)
        {
            GUI.matrix = Matrix4x4.identity;
        }
    }

    /// <summary>
    ///     Draws a table header at the given region.
    /// </summary>
    /// <param name="region">The region to draw the header in</param>
    /// <param name="name">The name of the header</param>
    /// <param name="order">The sort order of the header's data</param>
    /// <param name="anchor">The text anchor of the header's name</param>
    /// <param name="fontScale">The font scale of the header's name</param>
    /// <param name="vertical">Whether or not to draw the header vertically</param>
    /// <param name="marginX">
    ///     The amount of space to contract from <see cref="region" />'s
    ///     horizontal axis before drawing <see cref="name" />
    /// </param>
    /// <param name="marginY">
    ///     The amount of space to contract from <see cref="region" />'s
    ///     vertical axis before drawing <see cref="name" />
    /// </param>
    /// <returns>Whether or not the header was clicked</returns>
    public static bool TableHeader(
        Rect region,
        string name,
        SortOrder order = SortOrder.None,
        TextAnchor anchor = TextAnchor.MiddleLeft,
        GameFont fontScale = GameFont.Small,
        bool vertical = false,
        float marginX = 5f,
        float marginY = 0f
    )
    {
        Text.Anchor = anchor;
        Text.Font = fontScale;

        if (vertical)
        {
            region.y += region.width;
            GUIUtility.RotateAroundPivot(-90f, region.position);
        }

        GUI.color = TableHeaderColor;
        Widgets.DrawHighlight(region);
        GUI.color = Color.white;

        if (Mouse.IsOver(region))
        {
            GUI.color = Color.grey;
            Widgets.DrawLightHighlight(region);
            GUI.color = Color.white;
        }

        Rect textRegion = region.ContractedBy(marginX, marginY);
        Widgets.Label(textRegion, name);
        bool pressed = Widgets.ButtonInvisible(region);

        switch (order)
        {
            case SortOrder.Descending:
            case SortOrder.Ascending:
                SortIndicator(textRegion, order);

                break;
        }

        if (vertical)
        {
            GUI.matrix = Matrix4x4.identity;
        }

        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;

        return pressed;
    }

    /// <summary>
    ///     Draws a table header at the given region.
    /// </summary>
    /// <param name="region">The region to draw the header in</param>
    /// <param name="icon">The icon of the header</param>
    /// <param name="order">The sort order of the header's data</param>
    /// <param name="vertical">Whether or not to draw the header vertically</param>
    /// <param name="margin">
    ///     The amount of space to contract from <see cref="region" /> before
    ///     drawing <see cref="icon" />
    /// </param>
    /// <returns>Whether or not the header was clicked</returns>
    public static bool TableHeader(Rect region, Texture2D icon, SortOrder order = SortOrder.None, bool vertical = false, float margin = 5f)
    {
        if (vertical)
        {
            region.y += region.width;
            GUIUtility.RotateAroundPivot(-90f, region.position);
        }

        GUI.color = TableHeaderColor;
        Widgets.DrawHighlight(region);
        GUI.color = Color.white;

        if (Mouse.IsOver(region))
        {
            GUI.color = Color.grey;
            Widgets.DrawLightHighlight(region);
            GUI.color = Color.white;
        }

        Rect innerRegion = region.ContractedBy(margin);

        Rect iconRect = LayoutHelper.IconRect(innerRegion.x, innerRegion.y, innerRegion.height, innerRegion.height, 4f);

        GUI.DrawTexture(iconRect, icon);
        bool pressed = Widgets.ButtonInvisible(region);

        switch (order)
        {
            case SortOrder.Descending:
            case SortOrder.Ascending:
                SortIndicator(innerRegion, order);

                break;
        }

        if (vertical)
        {
            GUI.matrix = Matrix4x4.identity;
        }

        return pressed;
    }

    /// <summary>
    ///     Draws a grouping header for the given content.
    /// </summary>
    /// <param name="listing">The <see cref="Listing" /> object use for layout</param>
    /// <param name="name">The name of the group header</param>
    /// <param name="gapPrefix">Whether or not to prepend a gap before the group header</param>
    public static void GroupHeader(this Listing listing, string name, bool gapPrefix = true)
    {
        if (gapPrefix)
        {
            listing.Gap(Mathf.CeilToInt(Text.LineHeight * 1.25f));
        }

        LabelDrawer.Draw(listing.GetRect(Text.LineHeight), name, TextAnchor.LowerLeft, GameFont.Tiny);
        listing.GapLine(6f);
    }

    /// <summary>
    ///     Draws a button suitable for tabbed content.
    /// </summary>
    /// <param name="region">The region to draw the tab button in</param>
    /// <param name="name">The name of the tab</param>
    /// <param name="anchor">The text anchor of the tab</param>
    /// <param name="fontScale">The font scale of the tab</param>
    /// <param name="vertical">Whether or not to draw the tab vertically</param>
    /// <param name="active">Whether or not the tab is the currently open tab</param>
    /// <returns>Whether or not the tab was clicked</returns>
    public static bool TabButton(
        Rect region,
        string name,
        TextAnchor anchor = TextAnchor.MiddleLeft,
        GameFont fontScale = GameFont.Small,
        bool vertical = false,
        bool active = false
    )
    {
        Text.Anchor = anchor;
        Text.Font = fontScale;

        if (vertical)
        {
            region.y += region.width;
            GUIUtility.RotateAroundPivot(-90f, region.position);
        }

        GUI.color = active ? ActiveTabColor : InactiveTabColor;
        Widgets.DrawHighlight(region);
        GUI.color = Color.white;

        if (!active && Mouse.IsOver(region))
        {
            Widgets.DrawLightHighlight(region);
        }

        Widgets.Label(region, name);
        bool pressed = Widgets.ButtonInvisible(region);

        if (vertical)
        {
            GUI.matrix = Matrix4x4.identity;
        }

        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;

        return pressed;
    }

    /// <summary>
    ///     Draws the specified <see cref="ThingDef" /> in form suitable for
    ///     users.
    /// </summary>
    /// <param name="region">The region to draw the thing at</param>
    /// <param name="def">The <see cref="ThingDef" /> to draw</param>
    /// <param name="labelOverride">An override for the thing's label</param>
    /// <param name="infoCard">
    ///     Whether or not to show the info card for the
    ///     thing when it's clicked
    /// </param>
    public static void DrawThing(Rect region, ThingDef def, string? labelOverride = null, bool infoCard = true)
    {
        var iconRect = new Rect(region.x + 2f, region.y + 2f, region.height - 4f, region.height - 4f);
        var labelRect = new Rect(iconRect.x + region.height, region.y, region.width - region.height, region.height);

        Widgets.ThingIcon(iconRect, def);
        LabelDrawer.Draw(labelRect, labelOverride ?? def.label?.CapitalizeFirst() ?? def.defName);

        if (Current.Game == null || !infoCard)
        {
            return;
        }

        if (Widgets.ButtonInvisible(region))
        {
            Find.WindowStack.Add(new Dialog_InfoCard(def));
        }

        Widgets.DrawHighlightIfMouseover(region);
    }

    /// <summary>
    ///     Draws a grouping header for the given mod-specific content.
    /// </summary>
    /// <param name="listing">The <see cref="Listing" /> object use for layout</param>
    /// <param name="modName">The human readable name of the mod</param>
    /// <param name="modId">The workshop id of the mod</param>
    /// <param name="gapPrefix">
    ///     Whether or not to prepend a gap before the
    ///     group header
    /// </param>
    public static void ModGroupHeader(this Listing listing, string modName, ulong modId, bool gapPrefix = true)
    {
        if (gapPrefix)
        {
            listing.Gap(Mathf.CeilToInt(Text.LineHeight * 1.25f));
        }

        Rect lineRect = listing.GetRect(Text.LineHeight);
        LabelDrawer.Draw(lineRect, modName, TextAnchor.LowerLeft, GameFont.Tiny);

        string modRequirementString = "ContentRequiresMod".Translate(modName);
        GUI.color = DescriptionDrawer.ExperimentalTextColor;

        Text.Font = GameFont.Tiny;
        float width = Text.CalcSize(modRequirementString).x;
        var modRequirementRect = new Rect(lineRect.x + lineRect.width - width, lineRect.y, width, Text.LineHeight);
        Text.Font = GameFont.Small;

        LabelDrawer.Draw(lineRect, modRequirementString, TextAnchor.LowerRight, GameFont.Tiny);
        GUI.color = Color.white;

        Widgets.DrawHighlightIfMouseover(modRequirementRect);

        if (Widgets.ButtonInvisible(modRequirementRect))
        {
            SteamUtility.OpenWorkshopPage(new PublishedFileId_t(modId));
        }

        listing.GapLine(6f);
    }
}
