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

/// <summary>A utility class for rendering descriptions for UI elements, like settings.</summary>
[PublicAPI]
public static class DescriptionDrawer
{
    public static readonly Color DescriptionTextColor = new(r: 0.72f, g: 0.72f, b: 0.72f);

    /// <summary>Renders a block of text within a defined rectangular region.</summary>
    /// <param name="region">The rectangular region where the text block will be drawn.</param>
    /// <param name="content">The text content to render within the specified region.</param>
    /// <param name="color">The color to apply to the rendered text.</param>
    public static void DrawTextBlock(Rect region, string content, Color color)
    {
        LabelDrawer.DrawLabel(region, content, color, TextAnchor.UpperLeft, GameFont.Tiny);
    }

    /// <summary>Calculates the height of a single line of text within a specified width.</summary>
    /// <param name="width">The width within which the text will be contained.</param>
    /// <param name="content">The text content to calculate the height for.</param>
    /// <returns>Returns the calculated height of the text content.</returns>
    public static float GetLineHeight(float width, string content)
    {
        GameFont previous = Text.Font;

        Text.Font = GameFont.Tiny;
        float height = Text.CalcHeight(content, width);
        Text.Font = previous;

        return height;
    }

    /// <summary>Calculates the height of the line required to display an experimental notice within the specified width.</summary>
    /// <param name="width">The width of the area to calculate the line height for.</param>
    /// <returns>The height of the line needed to render the experimental notice text.</returns>
    public static float GetExperimentalNoticeLineHeight(float width)
    {
        GameFont previous = Text.Font;

        Text.Font = GameFont.Tiny;
        float height = Text.CalcHeight(UxLocale.ExperimentalNotice, width);
        Text.Font = previous;

        return height;
    }

    /// <summary>Calculates the dimensions of a text block based on the content, width, and a splitting percentage.</summary>
    /// <param name="content">The text content to calculate the size for.</param>
    /// <param name="width">The total width available for the text block.</param>
    /// <param name="splitPercent">The percentage of the width allocated to the text block.</param>
    /// <returns>Returns a Vector2 representing the width and height of the calculated text block.</returns>
    public static Vector2 GetTextBlockSize(string content, float width, float splitPercent)
    {
        float finalWidth = width * splitPercent;

        return new Vector2(finalWidth, GetLineHeight(finalWidth, content));
    }

    /// <summary>Renders a description within a specified rectangular region.</summary>
    /// <param name="region">The rectangular area where the description will be drawn.</param>
    /// <param name="content">The description content to render.</param>
    public static void DrawDescription(Rect region, string content)
    {
        DrawTextBlock(region, content, DescriptionTextColor);
    }

    /// <summary>Renders a description within a specified rectangular region using the default description text color.</summary>
    /// <param name="listing">The listing to use for laying out.</param>
    /// <param name="content">The content of the description to be rendered.</param>
    public static void DrawDescription(this Listing listing, string content)
    {
        DrawDescription(GetTextBlockSize(listing, content), content);
    }

    /// <summary>Displays a notice indicating that the content is experimental.</summary>
    /// <param name="region">The rectangular region where the notice will be drawn.</param>
    /// <param name="content">
    ///     Optional custom message to display as the experimental notice. Uses a default message if not
    ///     provided.
    /// </param>
    public static void DrawExperimentalNotice(Rect region, string? content = null)
    {
        if (string.IsNullOrEmpty(content)) content = UxLocale.ExperimentalNotice;

        DrawTextBlock(region, content!, UxColors.RedishPink);
    }

    /// <summary>Renders a notice indicating experimental functionality within a specified rectangular region.</summary>
    /// <param name="listing">The listing to use for laying out.</param>
    /// <param name="content">
    ///     Optional custom text to display for the experimental notice. Defaults to a standard message if
    ///     not provided.
    /// </param>
    public static void DrawExperimentalNotice(this Listing listing, string content)
    {
        DrawExperimentalNotice(GetTextBlockSize(listing, content), content);
    }

    private static Rect GetTextBlockSize(Listing listing, string content)
    {
        Vector2 size = GetTextBlockSize(content, listing.ColumnWidth, splitPercent: 0.8f);
        Rect region = listing.GetRect(size.y);

        region.SetWidth(size.x);

        return region;
    }
}
