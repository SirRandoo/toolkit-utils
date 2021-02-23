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

using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models.Tables;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class ItemTableWorker : TableWorker<ItemTableItem>
    {
        private Rect categoryHeaderRect = Rect.zero;
        private string categoryHeaderText;
        private Rect categoryHeaderTextRect = Rect.zero;
        private Rect expandedHeaderInnerRect = Rect.zero;
        private Rect expandedHeaderRect = Rect.zero;
        private Rect nameHeaderRect = Rect.zero;
        private string nameHeaderText;
        private Rect nameHeaderTextRect = Rect.zero;
        private Rect priceHeaderRect = Rect.zero;
        private string priceHeaderText;
        private Rect priceHeaderTextRect = Rect.zero;
        private Vector2 scrollPos = Vector2.zero;
        private SortKey sortKey = SortKey.Name;
        private SortOrder sortOrder = SortOrder.Descending;
        private Rect stateHeaderInnerRect = Rect.zero;

        private Rect stateHeaderRect = Rect.zero;

        private StateKey stateKey = StateKey.Enable;

        public override void DrawHeaders(Rect canvas)
        {
            if (SettingsHelper.DrawTableHeader(
                stateHeaderRect,
                stateHeaderInnerRect,
                stateKey == StateKey.Enable ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                NotifyGlobalStateChanged(stateKey);
                stateKey = stateKey == StateKey.Enable ? StateKey.Disable : StateKey.Enable;
            }

            if (SettingsHelper.DrawTableHeader(expandedHeaderRect, expandedHeaderInnerRect, Textures.Gear))
            {
                NotifyGlobalSettingsCollapse();
            }

            DrawSortableHeaders();
            DrawSortableHeaderIcon();
        }

        private void DrawSortableHeaderIcon()
        {
            switch (sortKey)
            {
                case SortKey.Name:
                    SettingsHelper.DrawSortIndicator(nameHeaderRect, sortOrder);
                    return;
                case SortKey.Price:
                    SettingsHelper.DrawSortIndicator(priceHeaderRect, sortOrder);
                    return;
                case SortKey.Category:
                    SettingsHelper.DrawSortIndicator(categoryHeaderRect, sortOrder);
                    return;
                default:
                    return;
            }
        }

        private void DrawSortableHeaders()
        {
            var anyClicked = false;
            SortKey previousKey = sortKey;
            if (SettingsHelper.DrawTableHeader(nameHeaderRect, nameHeaderTextRect, nameHeaderText))
            {
                sortKey = SortKey.Name;
                anyClicked = true;
            }

            if (SettingsHelper.DrawTableHeader(priceHeaderRect, priceHeaderTextRect, priceHeaderText))
            {
                sortKey = SortKey.Price;
                anyClicked = true;
            }

            if (SettingsHelper.DrawTableHeader(categoryHeaderRect, categoryHeaderTextRect, categoryHeaderText))
            {
                sortKey = SortKey.Category;
                anyClicked = true;
            }

            if (sortKey != previousKey)
            {
                sortOrder = SortOrder.Descending;
                NotifySortRequested();
            }
            else if (anyClicked)
            {
                InvertSortOrder();
                NotifySortRequested();
            }
        }

        private void InvertSortOrder()
        {
            sortOrder = sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }

        private void NotifyGlobalSettingsCollapse()
        {
            foreach (ItemTableItem item in Data)
            {
                item.SettingsVisible = false;
            }
        }

        private void NotifyGlobalStateChanged(StateKey newState)
        {
            foreach (ItemTableItem item in Data)
            {
                item.Data.IsEnabled = newState == StateKey.Enable;
                item.Data.Update();
            }
        }

        public override void DrawTableContents(Rect canvas)
        {
            float expectedLines = Data.Where(i => !i.IsHidden).Sum(i => i.SettingsVisible ? 5f : 1f);
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, RowLineHeight * expectedLines + (expectedLines - 1));

            var index = 0;
            var expanded = 0;
            var alternate = false;
            GUI.BeginGroup(canvas);
            scrollPos = GUI.BeginScrollView(canvas, scrollPos, viewPort);

            foreach (ItemTableItem item in Data.Where(i => !i.IsHidden))
            {
                var lineRect = new Rect(
                    0f,
                    index * RowLineHeight + index + RowLineHeight * 4f * expanded,
                    canvas.width - 16f,
                    item.SettingsVisible ? RowLineHeight * 5f : RowLineHeight
                );

                if (!lineRect.IsRegionVisible(canvas, scrollPos))
                {
                    index++;
                    continue;
                }

                GUI.BeginGroup(lineRect);
                Rect rect = lineRect.AtZero();

                if (alternate)
                {
                    Widgets.DrawLightHighlight(rect);
                }

                DrawItem(rect, item);
                GUI.EndGroup();

                alternate = !alternate;
                index++;

                if (item.SettingsVisible)
                {
                    expanded++;
                }
            }

            GUI.EndScrollView();
            GUI.EndGroup();
        }

        private void DrawItem(Rect canvas, ItemTableItem item)
        {
            var checkboxRect = new Rect(stateHeaderRect.x, canvas.y, stateHeaderRect.width, RowLineHeight);
            var nameRect = new Rect(nameHeaderRect.x, canvas.y, nameHeaderRect.width, RowLineHeight);
            var priceRect = new Rect(priceHeaderRect.x, canvas.y, priceHeaderRect.width, RowLineHeight);
            var categoryRect = new Rect(categoryHeaderRect.x, canvas.y, categoryHeaderRect.width, RowLineHeight);
            var settingRect = new Rect(
                expandedHeaderRect.x,
                canvas.y + Mathf.FloorToInt(Mathf.Abs(expandedHeaderRect.width - RowLineHeight) / 2f),
                expandedHeaderRect.width,
                expandedHeaderRect.width
            );

            if (SettingsHelper.DrawCheckbox(checkboxRect, ref item.Data.IsEnabled))
            {
                item.Data.Update();
            }

            SettingsHelper.DrawColoredLabel(
                nameRect,
                item.Data.Name,
                item.Data.Thing == null ? Color.yellow : Color.white
            );
            SettingsHelper.DrawPriceField(priceRect, ref item.Data.Item.price);
            SettingsHelper.DrawLabel(categoryRect, item.Data.Category);

            if (Widgets.ButtonImage(settingRect, Textures.Gear))
            {
                item.SettingsVisible = !item.SettingsVisible;
            }
        }

        public override void Prepare()
        {
            LoadTranslations();

            Data ??= new List<ItemTableItem>();
            Data.AddRange(ToolkitUtils.Data.Items.OrderBy(i => i.Name).Select(i => new ItemTableItem {Data = i}));
        }

        private void LoadTranslations()
        {
            nameHeaderText = "TKUtils.Headers.Name".Localize();
            priceHeaderText = "TKUtils.Headers.Price".Localize();
            categoryHeaderText = "TKUtils.Headers.Category".Localize();
        }

        public override void NotifySortRequested()
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    NotifyAscendingSortRequested();
                    return;
                case SortOrder.Descending:
                    NotifyDescendingSortRequested();
                    return;
                default:
                    return;
            }
        }

        private void NotifyDescendingSortRequested()
        {
            switch (sortKey)
            {
                case SortKey.Price:
                    Data = Data.OrderByDescending(i => i.Data.Price).ThenByDescending(i => i.Data.Name).ToList();
                    return;
                case SortKey.Category:
                    Data = Data.OrderByDescending(i => i.Data.Category).ThenByDescending(i => i.Data.Name).ToList();
                    return;
                default:
                    Data = Data.OrderByDescending(i => i.Data.Name).ToList();
                    return;
            }
        }

        private void NotifyAscendingSortRequested()
        {
            switch (sortKey)
            {
                case SortKey.Price:
                    Data = Data.OrderBy(i => i.Data.Price).ThenBy(i => i.Data.Name).ToList();
                    return;
                case SortKey.Category:
                    Data = Data.OrderBy(i => i.Data.Category).ThenBy(i => i.Data.Name).ToList();
                    return;
                default:
                    Data = Data.OrderBy(i => i.Data.Name).ToList();
                    return;
            }
        }

        public override void NotifySearchRequested(string query)
        {
            FilterDataBySearch(query);
        }

        public override void NotifyResolutionChanged(Rect canvas)
        {
            float distributedWidth = Mathf.FloorToInt((canvas.width - 16f - LineHeight * 2f) * 0.333f);
            stateHeaderRect = new Rect(0f, 0f, LineHeight, LineHeight);
            stateHeaderInnerRect = stateHeaderRect.ContractedBy(2f);
            nameHeaderRect = new Rect(LineHeight + 1f, 0f, distributedWidth, LineHeight);
            nameHeaderTextRect = new Rect(
                nameHeaderRect.x + 4f,
                nameHeaderRect.y,
                nameHeaderRect.width - 8f,
                nameHeaderRect.height
            );
            priceHeaderRect = nameHeaderRect.ShiftRight(1f);
            priceHeaderTextRect = new Rect(
                priceHeaderRect.x + 4f,
                priceHeaderRect.y,
                priceHeaderRect.width - 8f,
                priceHeaderRect.height
            );
            categoryHeaderRect = priceHeaderRect.ShiftRight(1f);
            categoryHeaderTextRect = new Rect(
                categoryHeaderRect.x + 4f,
                categoryHeaderRect.y,
                categoryHeaderRect.width - 8f,
                categoryHeaderRect.height
            );
            expandedHeaderRect = new Rect(categoryHeaderRect.x + categoryHeaderRect.width, 0f, LineHeight, LineHeight);
            expandedHeaderInnerRect = expandedHeaderRect.ContractedBy(2f);
        }

        protected override void FilterDataBySearch(string query)
        {
            foreach (ItemTableItem item in Data)
            {
                item.IsHidden = !query.NullOrEmpty() && !item.Data.Name.ToLower().Contains(query.ToLower());
            }
        }

        private enum StateKey { Enable, Disable }

        private enum SortKey { Name, Price, Category }
    }
}
