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
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class TabWorker
    {
        private readonly List<TabItem> tabItems = new List<TabItem>();

        public TabItem SelectedTab { get; set; }

        [NotNull]
        public static TabWorker CreateInstance() => new TabWorker();

        [NotNull]
        public static TabWorker CreateInstance([NotNull] params TabItem[] tabs)
        {
            var worker = new TabWorker();
            worker.tabItems.AddRange(tabs);

            return worker;
        }

        public void AddTab(TabItem tab)
        {
            tabItems.Add(tab);
            SelectedTab ??= tab;
        }

        public void RemoveTab(string label)
        {
            TabItem tab = GetTab(label);

            if (tab != null)
            {
                tabItems.Remove(tab);
            }
        }

        [CanBeNull]
        public TabItem GetTab(string label)
        {
            return tabItems.FirstOrDefault(t => t.Label.Equals(label, StringComparison.InvariantCulture));
        }

        public void Draw(Rect region, bool vertical = false, bool paneled = false)
        {
            float offset = 0;

            foreach (TabItem tab in tabItems)
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

                if (SettingsHelper.DrawTabButton(tabRegion, tab.Label, TextAnchor.MiddleCenter, vertical: vertical, selected: SelectedTab == tab))
                {
                    if (tab.Clicked == null || tab.Clicked())
                    {
                        SelectedTab = tab;
                    }
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
}
