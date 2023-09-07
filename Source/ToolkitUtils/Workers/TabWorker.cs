// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers;

/// <summary>
///     A class for handling tabular content.
/// </summary>
public class TabWorker
{
    private readonly List<TabItem> _tabItems = new List<TabItem>();

    /// <summary>
    ///     The currently selected tab.
    /// </summary>
    public TabItem SelectedTab { get; set; }

    /// <summary>
    ///     Creates a new empty instance of a <see cref="TabWorker"/>.
    /// </summary>
    public static TabWorker CreateInstance() => new TabWorker();

    /// <summary>
    ///     Creates a new instance of a <see cref="TabWorker"/> with the
    ///     given tabs.
    /// </summary>
    /// <param name="tabs">The tabs to display</param>
    public static TabWorker CreateInstance(params TabItem[] tabs)
    {
        var worker = new TabWorker();
        worker._tabItems.AddRange(tabs);

        return worker;
    }

    /// <summary>
    ///     Adds a tab to the worker's internal list.
    /// </summary>
    /// <param name="tab"></param>
    public void AddTab(TabItem tab)
    {
        _tabItems.Add(tab);
        SelectedTab ??= tab;
    }

    /// <summary>
    ///     Removes a tab from the worker's internal list.
    /// </summary>
    /// <param name="label"></param>
    public void RemoveTab(string label)
    {
        TabItem tab = GetTab(label);

        if (tab != null)
        {
            _tabItems.Remove(tab);
        }
    }

    /// <summary>
    ///     Returns the tab that matches the label given.
    /// </summary>
    /// <param name="label">The tab label to get</param>
    [CanBeNull]
    public TabItem GetTab(string label)
    {
        return _tabItems.FirstOrDefault(t => t.Label.Equals(label, StringComparison.InvariantCulture));
    }

    /// <summary>
    ///     Draws the tab header according to the current tabs within the
    ///     worker.
    /// </summary>
    /// <param name="region">The region to draw the header in</param>
    /// <param name="vertical">Whether the header should be drawn vertically</param>
    /// <param name="paneled">
    ///     Whether the header should be drawn paneled,
    ///     like old Windows software
    /// </param>
    public void Draw(Rect region, bool vertical = false, bool paneled = false)
    {
        float offset = 0;

        foreach (TabItem tab in _tabItems)
        {
            var tabRegion = new Rect(region.x, region.y, tab.Width + 25f, region.height);

            if (vertical)
            {
                tabRegion.y += offset;
            }
            else
            {
                tabRegion.x += offset;
            }

            if (UiHelper.TabButton(tabRegion, tab.Label, TextAnchor.MiddleCenter, vertical: vertical, active: SelectedTab == tab)
                && (tab.Clicked == null || tab.Clicked()))
            {
                SelectedTab = tab;
            }

            offset += tabRegion.width;

            if (!paneled)
            {
                continue;
            }

            GUI.color = Color.grey;
            Widgets.DrawLineHorizontal(tabRegion.x, tabRegion.y, tabRegion.width);
            GUI.color = Color.black;
            Widgets.DrawLineVertical(tabRegion.x, tabRegion.y, tabRegion.height);
            GUI.color = Color.white;
        }
    }
}