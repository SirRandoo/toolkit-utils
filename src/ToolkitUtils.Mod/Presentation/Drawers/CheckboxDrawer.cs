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
using ToolkitUtils.Mod.Presentation.Helpers;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Drawers;

/// <summary>A collection of specialized methods for drawing checkboxes.</summary>
[PublicAPI]
public static class CheckboxDrawer
{
    /// <summary>Draws a checkbox.</summary>
    /// <param name="region">The region to draw the checkbox in</param>
    /// <param name="state">The current state of the checkbox</param>
    /// <returns>Whether the checkbox was clicked</returns>
    public static bool DrawCheckbox(Rect region, ref bool state)
    {
        GUI.color = state ? Color.green : Color.red;
        GUI.DrawTexture(LayoutHelper.IconRect(region.x, region.y, region.width, region.height), state ? Icons.Check.Value : Icons.X.Value, ScaleMode.ScaleToFit);
        GUI.color = Color.white;

        if (Widgets.ButtonInvisible(region))
        {
            state = !state;

            return true;
        }

        return false;
    }

    /// <summary>Draws a checkbox with a label.</summary>
    /// <param name="region">The area within which the checkbox should be drawn.</param>
    /// <param name="label">The text label displayed alongside the checkbox.</param>
    /// <param name="state">The current state of the checkbox, passed by reference.</param>
    /// <param name="anchor">The alignment of the label text relative to the checkbox.</param>
    /// <returns>True if the checkbox state was changed, otherwise false.</returns>
    public static bool DrawCheckbox(Rect region, string label, ref bool state, TextAnchor anchor = TextAnchor.MiddleLeft)
    {
        var copy = new Rect(region);
        copy.SetWidth(region.width - region.height - 2f);

        LabelDrawer.DrawLabel(copy, label, anchor);

        copy.SetX(copy.x + copy.width + 2f);
        copy.SetWidth(copy.height);

        bool proxy = state;

        DrawCheckbox(copy, ref proxy);

        bool changed = proxy != state;
        state = proxy;

        return changed;
    }
}
