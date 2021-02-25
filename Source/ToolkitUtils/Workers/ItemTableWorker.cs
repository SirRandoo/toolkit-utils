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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models.Tables;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class ItemTableWorker : TableWorker<ItemTableItem>
    {
        private const float ExpandedLineSpan = 3f;
        private protected Rect CategoryHeaderRect = Rect.zero;
        private string categoryHeaderText;
        private protected Rect CategoryHeaderTextRect = Rect.zero;
        private string defaultKarmaTypeText;
        private Rect expandedHeaderInnerRect = Rect.zero;
        private Rect expandedHeaderRect = Rect.zero;
        private string isStuffText;

        private string karmaTypeText;
        private protected Rect NameHeaderRect = Rect.zero;
        private string nameHeaderText;
        private protected Rect NameHeaderTextRect = Rect.zero;
        private protected Rect PriceHeaderRect = Rect.zero;
        private string priceHeaderText;
        private protected Rect PriceHeaderTextRect = Rect.zero;
        private string purchaseWeightText;
        private string quantityLimitText;
        private Vector2 scrollPos = Vector2.zero;
        private SortKey sortKey = SortKey.Name;
        private SortOrder sortOrder = SortOrder.Descending;
        private string stackLimitTooltip;
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
                stateKey = stateKey == StateKey.Enable ? StateKey.Disable : StateKey.Enable;
                NotifyGlobalStateChanged(stateKey);
            }

            if (SettingsHelper.DrawTableHeader(expandedHeaderRect, expandedHeaderInnerRect, Textures.Gear))
            {
                NotifyGlobalSettingsCollapse();
            }

            DrawSortableHeaders();
            DrawSortableHeaderIcon();
        }

        private protected void DrawSortableHeaderIcon()
        {
            switch (sortKey)
            {
                case SortKey.Name:
                    SettingsHelper.DrawSortIndicator(NameHeaderRect, sortOrder);
                    return;
                case SortKey.Price:
                    SettingsHelper.DrawSortIndicator(PriceHeaderRect, sortOrder);
                    return;
                case SortKey.Category:
                    SettingsHelper.DrawSortIndicator(CategoryHeaderRect, sortOrder);
                    return;
                default:
                    return;
            }
        }

        private protected void DrawSortableHeaders()
        {
            var anyClicked = false;
            SortKey previousKey = sortKey;
            if (SettingsHelper.DrawTableHeader(NameHeaderRect, NameHeaderTextRect, nameHeaderText))
            {
                sortKey = SortKey.Name;
                anyClicked = true;
            }

            if (SettingsHelper.DrawTableHeader(PriceHeaderRect, PriceHeaderTextRect, priceHeaderText))
            {
                sortKey = SortKey.Price;
                anyClicked = true;
            }

            if (SettingsHelper.DrawTableHeader(CategoryHeaderRect, CategoryHeaderTextRect, categoryHeaderText))
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
            foreach (ItemTableItem item in Data.Where(i => !i.IsHidden))
            {
                item.SettingsVisible = false;
            }
        }

        private void NotifyGlobalStateChanged(StateKey newState)
        {
            foreach (ItemTableItem item in Data.Where(i => !i.IsHidden))
            {
                item.Data.IsEnabled = newState == StateKey.Enable;
                item.Data.Update();
            }
        }

        public override void DrawTableContents(Rect canvas)
        {
            float expectedLines = Data.Where(i => !i.IsHidden).Sum(i => i.SettingsVisible ? ExpandedLineSpan + 1f : 1f);
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
                    index * RowLineHeight + index + RowLineHeight * ExpandedLineSpan * expanded,
                    canvas.width - 16f,
                    RowLineHeight * (item.SettingsVisible ? ExpandedLineSpan + 1f : 1f)
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

        protected virtual void DrawItem(Rect canvas, ItemTableItem item)
        {
            var checkboxRect = new Rect(stateHeaderRect.x, canvas.y, stateHeaderRect.width, RowLineHeight);
            var nameRect = new Rect(NameHeaderTextRect.x, canvas.y, NameHeaderTextRect.width, RowLineHeight);
            var priceRect = new Rect(PriceHeaderTextRect.x, canvas.y, PriceHeaderTextRect.width, RowLineHeight);
            var categoryRect = new Rect(
                CategoryHeaderTextRect.x,
                canvas.y,
                CategoryHeaderTextRect.width,
                RowLineHeight
            );
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

            DrawConfigurableItemName(nameRect, item);

            if (item.Data.Price > 0)
            {
                SettingsHelper.DrawPriceField(priceRect, ref item.Data.Item.price);
            }

            SettingsHelper.DrawLabel(categoryRect, item.Data.Category);

            if (Widgets.ButtonImage(settingRect, Textures.Gear))
            {
                item.SettingsVisible = !item.SettingsVisible;
            }

            if (!item.SettingsVisible)
            {
                return;
            }

            var expandedRect = new Rect(
                NameHeaderRect.x + 10f,
                canvas.y + RowLineHeight + 10f,
                canvas.width - checkboxRect.width - settingRect.width - 20f,
                canvas.height - RowLineHeight - 20f
            );

            GUI.BeginGroup(expandedRect);
            DrawExpandedSettings(expandedRect.AtZero(), item);
            GUI.EndGroup();
        }

        private static void DrawConfigurableItemName(Rect canvas, ItemTableItem item)
        {
            if (item.EditingName)
            {
                string text = item.Data.Name;

                if (SettingsHelper.DrawTextField(canvas, text, out string result))
                {
                    item.Data.Data.CustomName = result;
                }
            }
            else
            {
                SettingsHelper.DrawColoredLabel(
                    canvas,
                    item.Data.Name,
                    item.Data.Thing == null ? Color.yellow : Color.white
                );
            }

            GUI.color = new Color(1f, 1f, 1f, 0.7f);
            if (SettingsHelper.DrawFieldButton(canvas, item.EditingName ? Widgets.CheckboxOnTex : Textures.Edit))
            {
                item.EditingName = !item.EditingName;
            }

            GUI.color = Color.white;
        }

        public override void Prepare()
        {
            LoadTranslations();

            _data ??= new List<ItemTableItem>();
            _data.AddRange(ToolkitUtils.Data.Items.OrderBy(i => i.Name).Select(i => new ItemTableItem {Data = i}));
        }

        private void DrawExpandedSettings(Rect canvas, ItemTableItem item)
        {
            float columnWidth = Mathf.FloorToInt(canvas.width / 2f) - 26f;
            var leftColumnRect = new Rect(canvas.x, canvas.y, columnWidth, canvas.height);
            var rightColumnRect = new Rect(canvas.x + leftColumnRect.width + 52f, canvas.y, columnWidth, canvas.height);

            Widgets.DrawLineVertical(Mathf.FloorToInt(canvas.width / 2f), 0f, canvas.height - 5f);

            GUI.BeginGroup(leftColumnRect);
            DrawLeftExpandedSettingsColumn(leftColumnRect.AtZero(), item);
            GUI.EndGroup();

            GUI.BeginGroup(rightColumnRect);
            DrawRightExpandedSettingsColumn(rightColumnRect.AtZero(), item);
            GUI.EndGroup();
        }

        private void DrawLeftExpandedSettingsColumn(Rect canvas, ItemTableItem item)
        {
            (Rect karmaLabel, Rect karmaField) = new Rect(0f, 0f, canvas.width, RowLineHeight).ToForm();
            SettingsHelper.DrawLabel(karmaLabel, karmaTypeText);
            if (Widgets.ButtonText(
                karmaField,
                item.Data.Data.KarmaType == null ? defaultKarmaTypeText : item.Data.Data.KarmaType.ToString()
            ))
            {
                Find.WindowStack.Add(
                    new FloatMenu(
                        ToolkitUtils.Data.KarmaTypes.Select(
                                i => new FloatMenuOption(i.ToString(), () => item.Data.Data.KarmaType = i)
                            )
                           .ToList()
                    )
                );
            }

            var weightBuffer = item.Data.Data.Weight.ToString("N1");
            (Rect weightLabel, Rect weightField) = new Rect(0f, RowLineHeight, canvas.width, RowLineHeight).ToForm();
            SettingsHelper.DrawLabel(weightLabel, purchaseWeightText);
            Widgets.TextFieldNumeric(weightField, ref item.Data.Data.Weight, ref weightBuffer);
        }

        private void DrawRightExpandedSettingsColumn(Rect canvas, ItemTableItem item)
        {
            Widgets.CheckboxLabeled(
                new Rect(0f, 0f, canvas.width, RowLineHeight),
                isStuffText,
                ref item.Data.Data.IsStuffAllowed
            );

            Widgets.CheckboxLabeled(
                new Rect(
                    0f,
                    RowLineHeight,
                    canvas.width - (item.Data.Data.HasQuantityLimit ? Mathf.FloorToInt(canvas.width * 0.2f) + 4f : 0f),
                    RowLineHeight
                ),
                quantityLimitText,
                ref item.Data.Data.HasQuantityLimit
            );

            if (!item.Data.Data.HasQuantityLimit)
            {
                return;
            }

            var quantityBuffer = item.Data.Data.QuantityLimit.ToString();
            var quantityField = new Rect(
                Mathf.FloorToInt(canvas.width * 0.8f),
                RowLineHeight,
                Mathf.FloorToInt(canvas.width * 0.2f),
                RowLineHeight
            );
            Widgets.TextFieldNumeric(quantityField, ref item.Data.Data.QuantityLimit, ref quantityBuffer, 1f);

            if (SettingsHelper.DrawFieldButton(quantityField, Textures.Stack, stackLimitTooltip))
            {
                item.Data.Data.QuantityLimit = item.Data.Thing?.stackLimit ?? item.Data.Data.QuantityLimit;
            }
        }

        public override void EnsureExists(ItemTableItem data)
        {
            if (!_data.Any(i => i.Data.DefName.Equals(data.Data.DefName)))
            {
                _data.Add(data);
            }
        }

        private void LoadTranslations()
        {
            nameHeaderText = "TKUtils.Headers.Name".Localize();
            priceHeaderText = "TKUtils.Headers.Price".Localize();
            categoryHeaderText = "TKUtils.Headers.Category".Localize();

            quantityLimitText = "TKUtils.Fields.QuantityLimit".Localize();
            isStuffText = "TKUtils.Fields.CanBeStuff".Localize();
            karmaTypeText = "TKUtils.Fields.KarmaType".Localize();
            purchaseWeightText = "TKUtils.Fields.Weight".Localize();
            defaultKarmaTypeText = "TKUtils.Fields.DefaultKarmaType".Localize();

            stackLimitTooltip = "TKUtils.ItemTableTooltips.ToStackLimit".Localize();
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
                    _data = _data.OrderBy(i => i.Data.Price).ThenBy(i => i.Data.Name).ToList();
                    return;
                case SortKey.Category:
                    _data = _data.OrderBy(i => i.Data.Category).ThenBy(i => i.Data.Name).ToList();
                    return;
                default:
                    _data = _data.OrderBy(i => i.Data.Name).ToList();
                    return;
            }
        }

        private void NotifyAscendingSortRequested()
        {
            switch (sortKey)
            {
                case SortKey.Price:
                    _data = _data.OrderByDescending(i => i.Data.Price).ThenByDescending(i => i.Data.Name).ToList();
                    return;
                case SortKey.Category:
                    _data = _data.OrderByDescending(i => i.Data.Category).ThenByDescending(i => i.Data.Name).ToList();
                    return;
                default:
                    _data = _data.OrderByDescending(i => i.Data.Name).ToList();
                    return;
            }
        }

        public override void NotifySearchRequested(string query)
        {
            FilterDataBySearch(query);
        }

        public override void NotifyResolutionChanged(Rect canvas)
        {
            float consumedWidth = canvas.width - 18f - LineHeight * 2f; // Icon buttons
            float distributedWidth = Mathf.FloorToInt(consumedWidth * 0.36f);
            float remainingWidth = consumedWidth - distributedWidth * 2f;
            stateHeaderRect = new Rect(0f, 0f, LineHeight, LineHeight);
            stateHeaderInnerRect = stateHeaderRect.ContractedBy(2f);
            NameHeaderRect = new Rect(LineHeight + 1f, 0f, distributedWidth, LineHeight);
            NameHeaderTextRect = new Rect(
                NameHeaderRect.x + 4f,
                NameHeaderRect.y,
                NameHeaderRect.width - 8f,
                NameHeaderRect.height
            );
            PriceHeaderRect = new Rect(NameHeaderRect.x + NameHeaderRect.width + 1f, 0f, remainingWidth, LineHeight);
            PriceHeaderTextRect = new Rect(
                PriceHeaderRect.x + 4f,
                PriceHeaderRect.y,
                PriceHeaderRect.width - 8f,
                PriceHeaderRect.height
            );
            CategoryHeaderRect = new Rect(
                PriceHeaderRect.x + PriceHeaderRect.width + 1f,
                0f,
                distributedWidth,
                LineHeight
            );
            CategoryHeaderTextRect = new Rect(
                CategoryHeaderRect.x + 4f,
                CategoryHeaderRect.y,
                CategoryHeaderRect.width - 8f,
                CategoryHeaderRect.height
            );
            expandedHeaderRect = new Rect(
                CategoryHeaderRect.x + CategoryHeaderRect.width + 1f,
                0f,
                LineHeight,
                LineHeight
            );
            expandedHeaderInnerRect = expandedHeaderRect.ContractedBy(2f);
        }

        public override void NotifyCustomSearchRequested(Func<ItemTableItem, bool> worker)
        {
            foreach (ItemTableItem item in Data)
            {
                item.IsHidden = !worker(item);
            }
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
