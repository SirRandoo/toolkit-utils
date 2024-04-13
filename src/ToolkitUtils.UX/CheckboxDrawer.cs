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

using UnityEngine;
using Verse;

namespace ToolkitUtils.UX;

public static class CheckboxDrawer
{
    /// <summary>
    ///     Draws a checkbox.
    /// </summary>
    /// <param name="region">The region to draw the checkbox in</param>
    /// <param name="state">The current state of the checkbox</param>
    /// <returns>Whether or not the checkbox was clicked</returns>
    public static bool DrawCheckbox(Rect region, ref bool state)
    {
        bool proxy = state;
        Widgets.Checkbox(region.position, ref proxy, Mathf.Min(region.width, region.height), paintable: true);

        bool changed = proxy != state;
        state = proxy;

        return changed;
    }

    /// <summary>
    ///     Draws a checkbox.
    /// </summary>
    /// <param name="label">A string to use as the label for the checkbox.</param>
    /// <param name="region">The region to draw the checkbox in</param>
    /// <param name="state">The current state of the checkbox</param>
    /// <param name="tooltip">The optional tooltip for the checkbox.</param>
    /// <returns>Whether or not the checkbox was clicked</returns>
    public static bool DrawCheckbox(Rect region, string label, ref bool state, string? tooltip = null)
    {
        Rect checkboxRegion = LayoutHelper.IconRect(region.x, region.y, region.height, region.height);
        var labelRegion = new Rect(region.x + region.height, region.y, region.width - region.height, region.height);

        LabelDrawer.Draw(labelRegion, label);
        TooltipHandler.TipRegion(region, tooltip);

        return DrawCheckbox(checkboxRegion, ref state);
    }
}
