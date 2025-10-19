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
using JetBrains.Annotations;
using ToolkitUtils.Mod.Presentation.Extensions;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Drawers;

/// <summary>A set of utility methods for drawing boxes on screen.</summary>
[PublicAPI]
public static class BoxDrawer
{
    /// <summary>Draws a box on screen.</summary>
    /// <param name="region">The region of the screen to draw the box in.</param>
    /// <param name="thickness">The number of pixels the box's outline is.</param>
    /// <param name="color">The optional color of the box.</param>
    /// <remarks>
    ///     This is a slightly modified version of <see cref="Widgets.DrawBox(Rect, int, Texture2D)" /> in that it takes
    ///     an optional color as a parameter, but more importantly that the left side of the box doesn't have a rounding error
    ///     in its thickness. The original version from <see cref="Widgets" /> can occasionally draw a thicker border on the
    ///     left at some sizes and resolutions since it uses <see cref="Mathf.Ceil" />.
    /// </remarks>
    public static void DrawBox(Rect region, int thickness = 1, Color? color = null)
    {
        var drawRegion = new Rect();
        Color previousColor = GUI.color;

        float thicknessForUiScale = Prefs.UIScale * thickness;
        float finalThickness = Mathf.Floor(thickness - (thicknessForUiScale - Mathf.Ceil(thicknessForUiScale)) / Prefs.UIScale);

        if (color != null) GUI.color = color.Value;

        // Draw left side
        drawRegion.SetX(region.x);
        drawRegion.SetY(region.y);
        drawRegion.SetWidth(finalThickness);
        drawRegion.SetHeight(region.height);
        GUI.DrawTexture(drawRegion, BaseContent.WhiteTex);

        // Draw right side
        drawRegion.SetX(region.x + region.width - 1);
        GUI.DrawTexture(drawRegion, BaseContent.WhiteTex);

        // Draw top side
        drawRegion.SetX(region.x);
        drawRegion.SetHeight(finalThickness);
        drawRegion.SetWidth(region.width);
        GUI.DrawTexture(drawRegion, BaseContent.WhiteTex);

        // Draw bottom side
        drawRegion.SetY(region.y + region.height - 1);
        GUI.DrawTexture(drawRegion, BaseContent.WhiteTex);

        if (color != null) GUI.color = previousColor;
    }
}
