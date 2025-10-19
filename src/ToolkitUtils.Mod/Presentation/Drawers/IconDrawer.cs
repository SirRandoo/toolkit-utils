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
using ToolkitUtils.Mod.Presentation.Extensions;
using ToolkitUtils.Mod.Presentation.Helpers;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Drawers;

public static class IconDrawer
{
    /// <summary>Draws the specified texture in the color given.</summary>
    /// <param name="region">The region to draw the texture in</param>
    /// <param name="icon">The texture to draw</param>
    /// <param name="color">The color to draw the texture</param>
    /// <remarks>
    ///     This method doesn't recolor the texture given to the color specified; it changes the value of
    ///     <see cref="GUI.color" /> to the color specified and lets Unity handle the recoloring.
    /// </remarks>
    public static void DrawIcon(Rect region, Texture2D icon, Color? color)
    {
        region = LayoutHelper.IconRect(region.x, region.y, region.width, region.height);

        Color old = GUI.color;

        GUI.color = color ?? Color.white;
        GUI.DrawTexture(region, icon);
        GUI.color = old;
    }

    /// <summary>An internal method for creating a <see cref="Rect" /> suitable for drawing "field icons" in.</summary>
    /// <param name="parentRegion">The region of the field the icon is being drawn over</param>
    /// <param name="offset">An optional number indicating how many slots to offset the icon</param>
    /// <returns>The <see cref="Rect" /> to draw the field icon in</returns>
    internal static Rect GetFieldIconRect(Rect parentRegion, int offset = 0) =>
        LayoutHelper.IconRect(
            parentRegion.x + parentRegion.width - parentRegion.height * (offset + 1),
            parentRegion.y,
            parentRegion.height,
            parentRegion.height,
            Mathf.CeilToInt(parentRegion.height * 0.1f)
        );

    /// <summary>Draws an icon over an input field.</summary>
    /// <param name="parentRegion">The region of the field the icon is being drawn over</param>
    /// <param name="icon">A character to be used as the icon</param>
    /// <param name="tooltip">An optional tooltip for the icon</param>
    /// <param name="offset">An optional number indicated how many slots to offset the icon</param>
    public static void DrawFieldIcon(Rect parentRegion, char icon, string? tooltip = null, int offset = 0)
    {
        Rect region = GetFieldIconRect(parentRegion, offset);
        LabelDrawer.DrawLabel(region, icon.ToString(), TextAnchor.MiddleCenter);
        TooltipHandler.TipRegion(region, tooltip);
    }

    /// <summary>Draws an icon over an input field.</summary>
    /// <param name="parentRegion">The region of the field the icon is being drawn over</param>
    /// <param name="icon">A string to be used as the icon</param>
    /// <param name="tooltip">An optional tooltip for the icon</param>
    /// <param name="offset">An optional number indicated how many slots to offset the icon</param>
    public static void DrawFieldIcon(Rect parentRegion, string icon, string? tooltip = null, int offset = 0)
    {
        Rect region = GetFieldIconRect(parentRegion, offset);
        LabelDrawer.DrawLabel(region, icon, TextAnchor.MiddleCenter);
        TooltipHandler.TipRegion(region, tooltip);
    }

    /// <summary>Draws an icon over an input field.</summary>
    /// <param name="parentRegion">The region of the field the button is being drawn over</param>
    /// <param name="icon">A texture to be drawn as the icon</param>
    /// <param name="tooltip">An optional tooltip for the icon</param>
    /// <param name="offset">An optional number indicated how many slots to offset the icon</param>
    public static void DrawFieldIcon(Rect parentRegion, Texture2D icon, string? tooltip = null, int offset = 0)
    {
        Rect region = GetFieldIconRect(parentRegion, offset);
        GUI.DrawTexture(region, icon);
        TooltipHandler.TipRegion(region, tooltip);
    }

    /// <summary>Draws an experimental icon cutout to indicate a special or restricted area within the specified region.</summary>
    /// <param name="region">
    ///     The area in which the icon cutout will be drawn. This parameter is updated to accommodate the
    ///     space used by the cutout.
    /// </param>
    /// <remarks>
    ///     The method adjusts the provided region to account for the space occupied by the experimental icon cutout. It
    ///     also handles tooltips on hover and renders the cutout with the predefined icon and color.
    /// </remarks>
    public static void DrawExperimentalIconCutout(ref Rect region)
    {
        Rect cutout = LayoutHelper.IconRect(region.x, region.y, region.height, region.height, margin: 6f);

        region.SetX(region.x + region.height);
        region.SetWidth(region.width - region.height);

        DrawIcon(cutout, Icons.TriangleExclamation.Value, UxColors.RedishPink);

        if (Mouse.IsOver(cutout)) TooltipHandler.TipRegion(cutout, UxLocale.ExperimentalNoticeColored);
    }

    /// <summary>Draws a sorting indicator within the specified region based on the sort order provided.</summary>
    /// <param name="parentRegion">The rectangle describing the area to draw the sorting indicator.</param>
    /// <param name="order">The sort order that determines the indicator's appearance.</param>
    /// <remarks>This method utilizes pre-defined sort indicator textures and positions them relative to the given region.</remarks>
    public static void DrawSortIndicator(Rect parentRegion, SortOrder order)
    {
        Rect region = LayoutHelper.IconRect(
            parentRegion.x + parentRegion.width - parentRegion.height + 3f,
            parentRegion.y + 8f,
            parentRegion.height - 9f,
            parentRegion.height - 16f,
            margin: 0f
        );

        switch (order)
        {
            case SortOrder.Ascending:
                GUI.DrawTexture(region, Icons.SortUp.Value);

                return;
            case SortOrder.Descending:
                GUI.DrawTexture(region, Icons.SortDown.Value);

                return;
            case SortOrder.None:
                GUI.DrawTexture(region, Icons.Sort.Value);

                return;
        }
    }

    /// <summary>Draws a thing's icon and label within the specified region.</summary>
    /// <param name="region">The region where the thing will be drawn.</param>
    /// <param name="def">The definition of the thing to draw.</param>
    /// <param name="labelOverride">An optional label to use instead of the thing's default label.</param>
    /// <param name="infoCard">Whether clicking on the drawn thing opens its information card.</param>
    public static void DrawThing(Rect region, ThingDef def, string? labelOverride = null, bool infoCard = true)
    {
        var iconRect = new Rect(region.x + 2f, region.y + 2f, region.height - 4f, region.height - 4f);
        var labelRect = new Rect(iconRect.x + region.height, region.y, region.width - region.height, region.height);

        Widgets.ThingIcon(iconRect, def);
        LabelDrawer.DrawLabel(labelRect, labelOverride ?? def.label?.CapitalizeFirst() ?? def.defName);

        if (Current.Game == null || !infoCard) return;

        if (Widgets.ButtonInvisible(region)) Find.WindowStack.Add(new Dialog_InfoCard(def));

        Widgets.DrawHighlightIfMouseover(region);
    }
}
