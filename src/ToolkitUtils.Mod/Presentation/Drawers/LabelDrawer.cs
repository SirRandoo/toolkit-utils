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
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Drawers;

/// <summary>A collection of specialized classes for drawing text.</summary>
[PublicAPI]
public static class LabelDrawer
{
    /// <summary>Draws text on screen.</summary>
    /// <param name="region">The region to draw the text in.</param>
    /// <param name="text">The text to draw on screen.</param>
    /// <param name="anchor">The text alignment of the text within the region of the screen.</param>
    /// <param name="font">The font size of the text.</param>
    public static void DrawLabel(Rect region, string text, TextAnchor anchor = TextAnchor.MiddleLeft, GameFont font = GameFont.Small)
    {
        DrawLabel(region, text, Color.white, anchor, font);
    }

    /// <summary>Draws text on screen.</summary>
    /// <param name="region">The region to draw the text in.</param>
    /// <param name="text">The text to draw on screen.</param>
    /// <param name="textColor">The color of the text being drawn on screen.</param>
    /// <param name="anchor">The text alignment of the text within the region of the screen.</param>
    /// <param name="font">The font size of the text.</param>
    public static void DrawLabel(Rect region, string text, Color textColor, TextAnchor anchor = TextAnchor.MiddleLeft, GameFont font = GameFont.Small)
    {
        TextAnchor previousAnchor = Text.Anchor;
        GameFont previousFont = Text.Font;
        Color previousColor = GUI.color;

        Text.Anchor = anchor;
        Text.Font = font;

        GUI.color = textColor;
        Widgets.Label(region, text);
        GUI.color = previousColor;

        Text.Anchor = previousAnchor;
        Text.Font = previousFont;
    }

    /// <summary>Draws text on screen.</summary>
    /// <param name="listing">The <see cref="Listing" /> to use for laying out content.</param>
    /// <param name="text">The text to draw on screen.</param>
    /// <param name="textColor">The color of the text being drawn on screen.</param>
    /// <param name="anchor">The text alignment of the text within the region of the screen.</param>
    /// <param name="font">The font size of the text.</param>
    public static void DrawLabel(this Listing listing, string text, Color textColor, TextAnchor anchor = TextAnchor.MiddleLeft, GameFont font = GameFont.Small)
    {
        Rect region = listing.GetRect(UiConstants.LineHeight);

        DrawLabel(region, text, textColor, anchor, font);
    }

    /// <summary>Draws text on screen.</summary>
    /// <param name="listing">The <see cref="Listing" /> to use for laying out content.</param>
    /// <param name="text">The text to draw on screen.</param>
    /// <param name="anchor">The text alignment of the text within the region of the screen.</param>
    /// <param name="font">The font size of the text.</param>
    public static void DrawLabel(this Listing listing, string text, TextAnchor anchor = TextAnchor.MiddleLeft, GameFont font = GameFont.Small)
    {
        Rect region = listing.GetRect(UiConstants.LineHeight);

        DrawLabel(region, text, anchor, font);
    }
}
