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
using System;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.UX.Extensions.RimWorld;
using UnityEngine;

namespace ToolkitUtils.Mod.Presentation.Extensions;

/// <summary>A collection of extension methods for modifying <see cref="Rect" />s without an allocation.</summary>
/// <remarks>
///     These extension methods mutate an existing <see cref="Rect" />, and thus aren't suitable for operations that
///     require immutable modifications.
/// </remarks>
[PublicAPI]
public static class RectExtensions
{
    /// <summary>
    ///     Shifts a region to the direction specified with an optional padding between the origin region and the
    ///     destination region.
    /// </summary>
    /// <param name="region">The original region being shifted in a given direction.</param>
    /// <param name="direction">The direction a region is being shifted to.</param>
    /// <param name="padding">An optional amount of additional shifting the region will go through.</param>
    /// <exception cref="ArgumentOutOfRangeException">An unsupported direction was specified.</exception>
    public static ref Rect Shift(this ref Rect region, Direction8Way direction, float padding = 2f)
    {
        switch (direction)
        {
            case Direction8Way.North: region.y -= region.height + padding; break;
            case Direction8Way.NorthEast:
                region.x += region.width + padding;
                region.y -= region.height + padding;

                break;
            case Direction8Way.East: region.x += region.width + padding; break;
            case Direction8Way.SouthEast:
                region.x += region.width + padding;
                region.y += region.height + padding;

                break;
            case Direction8Way.South: region.y += region.height + padding; break;
            case Direction8Way.SouthWest:
                region.y += region.height + padding;
                region.x -= region.width + padding;

                break;
            case Direction8Way.West: region.x -= region.width + padding; break;
            case Direction8Way.NorthWest:
                region.x -= region.width + padding;
                region.y -= region.height + padding;

                break;
            case Direction8Way.Invalid: break;
            default: throw new ArgumentOutOfRangeException(nameof(direction), direction, $"""The direction "{direction.ToStringFast()}" isn't supported for shift operations.""");
        }

        return ref region;
    }

    /// <summary>Contracts the region by the specified margin.</summary>
    /// <param name="region">The original region being contracted by the given margin.</param>
    /// <param name="margin">The margin to contract the region by.</param>
    public static ref Rect ContractedBy(this ref Rect region, float margin)
    {
        region.x += margin;
        region.y += margin;
        region.height -= margin + margin;
        region.width -= margin + margin;

        return ref region;
    }

    /// <summary>Expands the region by the specified margin.</summary>
    /// <param name="region">The original region being contracted by the given margin.</param>
    /// <param name="margin">The margin to contract the region by.</param>
    public static ref Rect ExpandedBy(this ref Rect region, float margin)
    {
        region.x -= margin;
        region.y -= margin;
        region.height += margin + margin;
        region.width += margin + margin;

        return ref region;
    }

    /// <summary>Sets the region's X and Y to zero.</summary>
    /// <param name="region">The region to zero the position for.</param>
    public static ref Rect AtZero(this ref Rect region)
    {
        region.x = 0f;
        region.y = 0f;

        return ref region;
    }

    /// <summary>Sets the X position for a given region.</summary>
    /// <param name="region">The region whose X position is being set.</param>
    /// <param name="x">The new X position of the given region.</param>
    public static ref Rect SetX(this ref Rect region, float x)
    {
        region.x = x;

        return ref region;
    }

    /// <summary>Sets the Y position for a given region.</summary>
    /// <param name="region">The region whose Y position is being set.</param>
    /// <param name="y">The new Y position of the given region.</param>
    public static ref Rect SetY(this ref Rect region, float y)
    {
        region.y = y;

        return ref region;
    }

    /// <summary>Sets the width position for a given region.</summary>
    /// <param name="region">The region whose width is being set.</param>
    /// <param name="width">The new width of the given region.</param>
    public static ref Rect SetWidth(this ref Rect region, float width)
    {
        region.width = width;

        return ref region;
    }

    /// <summary>Sets the height for a given region.</summary>
    /// <param name="region">The region whose height is being set.</param>
    /// <param name="height">The new height of the given region.</param>
    public static ref Rect SetHeight(this ref Rect region, float height)
    {
        region.height = height;

        return ref region;
    }
}
