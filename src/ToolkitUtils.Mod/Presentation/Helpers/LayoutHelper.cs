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
using RimWorld;
using ToolkitUtils.Mod.Presentation.Extensions;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Helpers;

[PublicAPI]
public static class LayoutHelper
{
    /// <summary>Shifts a <see cref="Rect" /> in the specified direction.</summary>
    /// <param name="region">The region to shift</param>
    /// <param name="direction">The direction to shift the region to</param>
    /// <param name="padding">The amount of padding to add to the shifted region</param>
    /// <returns>The shifted region</returns>
    public static Rect Shift(this Rect region, Direction8Way direction, float padding = 5f)
    {
        switch (direction)
        {
            case Direction8Way.North:     return new Rect(region.x, region.y - region.height - padding, region.width, region.height);
            case Direction8Way.NorthEast: return new Rect(region.x + region.width + padding, region.y - region.height - padding, region.width, region.height);
            case Direction8Way.East:      return new Rect(region.x + region.width + padding, region.y, region.width, region.height);
            case Direction8Way.SouthEast: return new Rect(region.x + region.width + padding, region.y + region.height + padding, region.width, region.height);
            case Direction8Way.South:     return new Rect(region.x, region.y + region.height + padding, region.width, region.height);
            case Direction8Way.SouthWest: return new Rect(region.x - region.width - padding, region.y + region.height + padding, region.width, region.height);
            case Direction8Way.West:      return new Rect(region.x - region.width - padding, region.y, region.width, region.height);
            case Direction8Way.NorthWest: return new Rect(region.x - region.width - padding, region.y - region.height - padding, region.width, region.height);
            case Direction8Way.Invalid:
            default:
                return region;
        }
    }

    /// <summary>Determines whether a given region is visible in a scroll view.</summary>
    /// <param name="region">The region in question</param>
    /// <param name="scrollRect">The visible region of the scroll view</param>
    /// <param name="scrollPos">The current scroll position of the scroll bar</param>
    /// <returns>Whether the region is visible</returns>
    /// <remarks>
    ///     The "visible" region, as mentioned in <see cref="scrollRect" />'s documentation, refers to a
    ///     <see cref="Rect" /> that defines the area on screen where content would be visible in a scroll view. This is
    ///     typically the first parameter of <see cref=" GUI.BeginScrollView(Rect, Vector2, Rect)" />.
    /// </remarks>
    public static bool IsVisible(this Rect region, Rect scrollRect, Vector2 scrollPos) =>
        (region.y >= scrollPos.y || region.y + region.height - 1f >= scrollPos.y) && region.y <= scrollPos.y + scrollRect.height;

    /// <summary>Splits a <see cref="Rect" /> in two.</summary>
    /// <param name="region">The rect to split</param>
    /// <param name="percent">A percent indicating how big the left side of the split should be relative to the region's width</param>
    /// <returns>A tuple containing the left and right sides of the current line rect</returns>
    public static (Rect leftRegion, Rect rightRegion) Split(this Rect region, float percent = 0.8f)
    {
        var left = new Rect(region.x, region.y, Mathf.FloorToInt(region.width * percent - 2f), region.height);

        return (left, new Rect(left.x + left.width + 2f, left.y, region.width - left.width - 2f, left.height));
    }

    /// <summary>Splits a <see cref="Rect" /> in two.</summary>
    /// <param name="listing">The <see cref="Listing" /> object to use</param>
    /// <param name="percent">A percent indicating how big the left side of the split should be relative to the region's width</param>
    /// <returns>A tuple containing the left and right sides of the current line rect</returns>
    public static (Rect leftRegion, Rect rightRegion) Split(this Listing listing, float percent = 0.8f)
    {
        Rect region = listing.GetRect(UiConstants.LineHeight);
        var leftRegion = new Rect(region.x, region.y, region.width * percent, region.height);
        region.SetWidth(region.width - leftRegion.width);
        region.SetX(leftRegion.x + leftRegion.width);

        return (leftRegion, region);
    }

    /// <summary>Splits the given region into two <see cref="Rect" />s</summary>
    /// <param name="x">The X position of the region</param>
    /// <param name="y">The Y position of the region</param>
    /// <param name="width">The width of the region</param>
    /// <param name="height">The height of the region</param>
    /// <param name="percent">A percent indicating how big the left side of the split should be relative to the region's width</param>
    /// <returns>A tuple containing the left and right sides of the current line rect.</returns>
    public static (Rect leftRegion, Rect rightRegion) Split(float x, float y, float width, float height, float percent = 0.8f)
    {
        var left = new Rect(x, y, Mathf.FloorToInt(width * percent), height);
        var right = new Rect(x + left.width, y, width - left.width, height);

        return (left, right);
    }

    /// <summary>Trims a <see cref="Rect" /> by the specified amount in the specified direction.</summary>
    /// <param name="region">The region to trim</param>
    /// <param name="direction">The direction to trim</param>
    /// <param name="amount">The amount to trim</param>
    /// <returns>The trimmed rect</returns>
    public static Rect Trim(this Rect region, Direction8Way direction, float amount)
    {
        switch (direction)
        {
            case Direction8Way.North:     return new Rect(region.x, region.y + amount, region.width, region.height - amount);
            case Direction8Way.NorthEast: return new Rect(region.x, region.y + amount, region.width - amount, region.height - amount);
            case Direction8Way.East:      return new Rect(region.x, region.y, region.width - amount, region.height);
            case Direction8Way.SouthEast: return new Rect(region.x, region.y, region.width - amount, region.height - amount);
            case Direction8Way.South:     return new Rect(region.x, region.y, region.width, region.height - amount);
            case Direction8Way.SouthWest: return new Rect(region.x + amount, region.y, region.width - amount, region.height - amount);
            case Direction8Way.West:      return new Rect(region.x + amount, region.y, region.width - amount, region.height);
            case Direction8Way.NorthWest: return new Rect(region.x + amount, region.y + amount, region.width - amount, region.height - amount);
            case Direction8Way.Invalid:
            default:
                return region;
        }
    }

    /// <summary>Shrinks and centers a deconstructed <see cref="Rect" /> for use in drawing icons.</summary>
    /// <param name="x">The X position of a region</param>
    /// <param name="y">The Y position of a region</param>
    /// <param name="width">The width of the given region</param>
    /// <param name="height">The height of the given region</param>
    /// <param name="margin">The amount of margins to add to the final <see cref="Rect" /></param>
    /// <returns>A scaled, centered <see cref="Rect" /> for drawing icons</returns>
    public static Rect IconRect(float x, float y, float width, float height, float margin = 2f)
    {
        float shortest = Mathf.Min(width, height);
        float halfShortest = Mathf.FloorToInt(shortest / 2f);
        float halfWidth = Mathf.FloorToInt(width / 2f);
        float halfHeight = Mathf.FloorToInt(height / 2f);

        return new Rect(
            Mathf.Clamp(x + halfWidth - halfShortest, x, x + width) + margin,
            Mathf.Clamp(y + halfHeight - halfShortest, y, y + height) + margin,
            shortest - margin * 2f,
            shortest - margin * 2f
        );
    }

    /// <summary>Trims a <see cref="Rect" />, in place, by the margin specified.</summary>
    /// <param name="region">The reference to the region being trimmed.</param>
    /// <param name="margin">The margin to trim from the region.</param>
    public static ref Rect TrimToIconRect(ref Rect region, float margin = 2f)
    {
        region.SetX(region.x + region.width - region.height);
        region.SetWidth(region.height);

        float shortest = Mathf.Min(region.width, region.height);
        float halfShortest = Mathf.FloorToInt(shortest / 2f);
        float halfWidth = Mathf.FloorToInt(region.width / 2f);
        float halfHeight = Mathf.FloorToInt(region.height / 2f);

        region.SetX(Mathf.Clamp(region.x + halfWidth - halfShortest, region.x, region.x + region.width) + margin);
        region.SetY(Mathf.Clamp(region.y + halfHeight - halfShortest, region.y, region.y + region.height) + margin);
        region.SetWidth(shortest - margin * 2f);
        region.SetHeight(shortest - margin * 2f);

        return ref region;
    }
}
