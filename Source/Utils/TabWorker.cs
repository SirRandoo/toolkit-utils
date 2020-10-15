using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class TabWorker
    {
        private readonly List<TabItem> tabItems = new List<TabItem>();

        public TabItem SelectedTab { get; set; }

        public static TabWorker CreateInstance()
        {
            return new TabWorker();
        }

        public static TabWorker CreateInstance(IEnumerable<TabItem> tabs)
        {
            var worker = new TabWorker();
            worker.tabItems.AddRange(tabs);

            return worker;
        }

        public void AddTab(TabItem tab)
        {
            tabItems.Add(tab);
        }

        public void RemoveTab(string label)
        {
            TabItem tab = GetTab(label);

            if (tab != null)
            {
                tabItems.Remove(tab);
            }
        }

        public TabItem GetTab(string label)
        {
            return tabItems.FirstOrDefault(t => t.Label.Equals(label, StringComparison.InvariantCulture));
        }

        public void Draw(Rect region, bool vertical = false)
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

                if (SettingsHelper.DrawTabButton(
                    tabRegion,
                    tab.Label,
                    TextAnchor.MiddleCenter,
                    vertical: vertical,
                    selected: SelectedTab == tab
                ))
                {
                    if (tab.Clicked == null || tab.Clicked())
                    {
                        SelectedTab = tab;
                    }
                }

                offset += tabRegion.width;
            }
        }
    }
}
