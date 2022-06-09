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
using SirRandoo.CommonLib.Enums;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class ItemTableWorker : TableWorker<TableSettingsItem<ThingItem>>
    {
        private const float ExpandedLineSpan = 5f;
        private string _categoryHeaderText;
        private string _closeItemNameTooltip;
        private string _defaultKarmaTypeText;
        private string _editItemNameTooltip;
        private string _equipKarmaTypeText;
        private Rect _expandedHeaderInnerRect = Rect.zero;
        private Rect _expandedHeaderRect = Rect.zero;
        private string _isEquippableText;
        private string _isStuffText;
        private string _isUsableText;
        private string _isWearableText;

        private string _karmaTypeText;
        private string _nameHeaderText;
        private string _priceHeaderText;
        private string _purchaseWeightText;
        private string _quantityLimitText;
        private string _resetItemKarmaTooltip;
        private string _resetItemNameTooltip;
        private Vector2 _scrollPos = Vector2.zero;

        private SettingsKey _settingsKey = SettingsKey.Collapse;
        private SortKey _sortKey = SortKey.Name;
        private SortOrder _sortOrder = SortOrder.Descending;
        private string _stackLimitTooltip;
        private Rect _stateHeaderInnerRect = Rect.zero;

        private Rect _stateHeaderRect = Rect.zero;
        private StateKey _stateKey = StateKey.Enable;
        private string _useKarmaTypeText;
        private string _wearKarmaTypeText;
        private protected Rect CategoryHeaderRect = Rect.zero;
        private protected Rect CategoryHeaderTextRect = Rect.zero;
        private protected Rect NameHeaderRect = Rect.zero;
        private protected Rect NameHeaderTextRect = Rect.zero;
        private protected Rect PriceHeaderRect = Rect.zero;
        private protected Rect PriceHeaderTextRect = Rect.zero;

        /// <inheritdoc cref="TableWorkerBase.DrawHeaders"/>
        protected override void DrawHeaders(Rect region)
        {
            if (SettingsHelper.DrawTableHeader(_stateHeaderRect, _stateHeaderInnerRect, _stateKey == StateKey.Enable ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                _stateKey = _stateKey == StateKey.Enable ? StateKey.Disable : StateKey.Enable;
                NotifyGlobalStateChanged(_stateKey);
            }

            if (SettingsHelper.DrawTableHeader(_expandedHeaderRect, _expandedHeaderInnerRect, Textures.Gear))
            {
                _settingsKey = _settingsKey == SettingsKey.Expand ? SettingsKey.Collapse : SettingsKey.Expand;
                NotifyGlobalSettingsChanged(_settingsKey);
            }

            DrawSortableHeaders();
            DrawSortableHeaderIcon();
        }

        private protected void DrawSortableHeaderIcon()
        {
            switch (_sortKey)
            {
                case SortKey.Name:
                    UiHelper.SortIndicator(NameHeaderRect, _sortOrder);

                    return;
                case SortKey.Price:
                    UiHelper.SortIndicator(PriceHeaderRect, _sortOrder);

                    return;
                case SortKey.Category:
                    UiHelper.SortIndicator(CategoryHeaderRect, _sortOrder);

                    return;
                default:
                    return;
            }
        }

        private protected void DrawSortableHeaders()
        {
            var anyClicked = false;
            SortKey previousKey = _sortKey;

            if (SettingsHelper.DrawTableHeader(NameHeaderRect, NameHeaderTextRect, _nameHeaderText))
            {
                _sortKey = SortKey.Name;
                anyClicked = true;
            }

            if (SettingsHelper.DrawTableHeader(PriceHeaderRect, PriceHeaderTextRect, _priceHeaderText))
            {
                _sortKey = SortKey.Price;
                anyClicked = true;
            }

            if (SettingsHelper.DrawTableHeader(CategoryHeaderRect, CategoryHeaderTextRect, _categoryHeaderText))
            {
                _sortKey = SortKey.Category;
                anyClicked = true;
            }

            if (_sortKey != previousKey)
            {
                _sortOrder = SortOrder.Descending;
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
            _sortOrder = _sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }

        private void NotifyGlobalSettingsChanged(SettingsKey newState)
        {
            foreach (TableSettingsItem<ThingItem> item in Data.Where(i => !i.IsHidden))
            {
                item.SettingsVisible = newState == SettingsKey.Expand;
            }
        }

        private void NotifyGlobalStateChanged(StateKey newState)
        {
            foreach (TableSettingsItem<ThingItem> item in Data.Where(i => !i.IsHidden))
            {
                item.Data.Enabled = newState == StateKey.Enable;
                item.Data.Update();
            }
        }

        /// <inheritdoc cref="TableWorkerBase.DrawTableContents"/>
        protected override void DrawTableContents(Rect region)
        {
            float expectedLines = Data.Where(i => !i.IsHidden).Sum(i => i.SettingsVisible ? ExpandedLineSpan + 1f : 1f);
            var viewPort = new Rect(0f, 0f, region.width - 16f, RowLineHeight * expectedLines);

            var index = 0;
            var expanded = 0;
            var alternate = false;
            GUI.BeginGroup(region);
            _scrollPos = GUI.BeginScrollView(region, _scrollPos, viewPort);

            foreach (TableSettingsItem<ThingItem> item in Data.Where(i => !i.IsHidden))
            {
                var lineRect = new Rect(
                    0f,
                    index * RowLineHeight + RowLineHeight * ExpandedLineSpan * expanded,
                    region.width - 16f,
                    RowLineHeight * (item.SettingsVisible ? ExpandedLineSpan + 1f : 1f)
                );

                if (!lineRect.IsVisible(region, _scrollPos))
                {
                    index++;
                    alternate = !alternate;
                    expanded += item.SettingsVisible ? 1 : 0;

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

        /// <summary>
        ///     Draws the given <see cref="ThingItem"/> at the given region.
        /// </summary>
        /// <param name="region">
        ///     The region to draw the <see cref="ThingItem"/>
        ///     in
        /// </param>
        /// <param name="item">The <see cref="ThingItem"/> to draw</param>
        protected virtual void DrawItem(Rect region, [NotNull] TableSettingsItem<ThingItem> item)
        {
            bool hasIcon = Widgets.CanDrawIconFor(item.Data.Thing);

            Rect checkboxRect = LayoutHelper.IconRect(_stateHeaderRect.x + 2f, region.y + 2f, _stateHeaderRect.width - 4f, RowLineHeight - 4f);
            var iconRect = new Rect(NameHeaderRect.x + 4f, region.y + 4f, RowLineHeight - 8f, RowLineHeight - 8f);

            var nameRect = new Rect(
                hasIcon ? NameHeaderRect.x + RowLineHeight : NameHeaderTextRect.x,
                region.y,
                hasIcon ? NameHeaderRect.width - RowLineHeight - 4f : NameHeaderTextRect.width,
                RowLineHeight
            );

            var thingRect = new Rect(
                hasIcon ? NameHeaderRect.x : NameHeaderTextRect.x,
                region.y,
                hasIcon ? NameHeaderRect.width - nameRect.height - 4f : NameHeaderTextRect.width,
                nameRect.height
            );

            var priceRect = new Rect(PriceHeaderTextRect.x, region.y, PriceHeaderTextRect.width, RowLineHeight);
            var categoryRect = new Rect(CategoryHeaderTextRect.x, region.y, CategoryHeaderTextRect.width, RowLineHeight);

            Rect settingRect = LayoutHelper.IconRect(
                _expandedHeaderRect.x + 2f,
                region.y + Mathf.FloorToInt(Mathf.Abs(_expandedHeaderRect.width - RowLineHeight) / 2f) + 2f,
                _expandedHeaderRect.width - 4f,
                _expandedHeaderRect.width - 4f
            );

            bool proxy = item.Data.Enabled;

            if (UiHelper.DrawCheckbox(checkboxRect, ref proxy))
            {
                item.Data.Enabled = proxy;
                item.Data.Update();
            }

            if (hasIcon)
            {
                Widgets.ThingIcon(iconRect, item.Data.Thing);
            }

            DrawConfigurableItemName(nameRect, item);

            if (!item.EditingName && item.Data.Thing != null && Current.Game != null)
            {
                Widgets.DrawHighlightIfMouseover(thingRect);

                if (Widgets.ButtonInvisible(thingRect))
                {
                    Find.WindowStack.Add(new Dialog_InfoCard(item.Data.Thing));
                }
            }

            if (item.Data.Cost > 0)
            {
                SettingsHelper.DrawPriceField(priceRect, ref item.Data.Item!.price);
            }

            UiHelper.Label(categoryRect, item.Data.Category);

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
                region.y + RowLineHeight + 10f,
                region.width - checkboxRect.width - settingRect.width - 20f,
                region.height - RowLineHeight - 20f
            );

            GUI.BeginGroup(expandedRect);
            DrawExpandedSettings(expandedRect.AtZero(), item);
            GUI.EndGroup();
        }

        private void DrawConfigurableItemName(Rect canvas, [NotNull] IConfigurableTableItem<ThingItem> item)
        {
            if (item.EditingName)
            {
                var fieldRect = new Rect(canvas.x, canvas.y, canvas.width - canvas.height, canvas.height);

                if (UiHelper.TextField(fieldRect, item.Data.Name, out string result))
                {
                    item.Data.ItemData!.CustomName = result.ToToolkit();
                }

                if (item.Data.ItemData!.CustomName != null && UiHelper.FieldButton(fieldRect, Textures.Reset, _resetItemNameTooltip))
                {
                    item.Data.ItemData.CustomName = null;
                }
            }
            else
            {
                UiHelper.Label(canvas, item.Data.Name, item.Data.Thing == null ? Color.yellow : Color.white, TextAnchor.MiddleLeft, GameFont.Small);
            }

            GUI.color = new Color(1f, 1f, 1f, 0.7f);

            if (UiHelper.FieldButton(canvas, item.EditingName ? Widgets.CheckboxOffTex : Textures.Edit, item.EditingName ? _closeItemNameTooltip : _editItemNameTooltip))
            {
                item.EditingName = !item.EditingName;
            }

            GUI.color = Color.white;
        }

        /// <inheritdoc cref="TableWorkerBase.Prepare"/>
        public override void Prepare()
        {
            LoadTranslations();

            InternalData ??= new List<TableSettingsItem<ThingItem>>();
            InternalData.AddRange(ToolkitUtils.Data.Items.OrderBy(i => i.Name).Select(i => new TableSettingsItem<ThingItem> { Data = i }));
        }

        private void DrawExpandedSettings(Rect canvas, [NotNull] TableSettingsItem<ThingItem> item)
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

        private void DrawLeftExpandedSettingsColumn(Rect canvas, [NotNull] TableSettingsItem<ThingItem> item)
        {
            var row = 0;
            (Rect karmaLabel, Rect karmaField) = new Rect(0f, row, canvas.width, RowLineHeight).Split();

            SettingsHelper.DrawKarmaField(
                karmaLabel,
                _karmaTypeText,
                karmaField,
                _defaultKarmaTypeText,
                item.Data.Data!.KarmaType,
                k => item.Data.Data.KarmaType = k,
                item.Data.Data.KarmaType != null,
                _resetItemKarmaTooltip
            );

            if (item.Data.Thing?.IsApparel == true)
            {
                row += 1;
                (Rect wearLabel, Rect wearField) = new Rect(0f, row * RowLineHeight, canvas.width, RowLineHeight).Split();

                SettingsHelper.DrawKarmaField(
                    wearLabel,
                    _wearKarmaTypeText,
                    wearField,
                    _defaultKarmaTypeText,
                    item.Data.ItemData!.KarmaTypeForWearing,
                    k => item.Data.ItemData.KarmaTypeForWearing = k,
                    item.Data.ItemData.KarmaTypeForWearing != null,
                    _resetItemKarmaTooltip
                );
            }

            if (item.Data.Thing?.IsWeapon == true)
            {
                row += 1;
                (Rect equipLabel, Rect equipField) = new Rect(0f, row * RowLineHeight, canvas.width, RowLineHeight).Split();

                SettingsHelper.DrawKarmaField(
                    equipLabel,
                    _equipKarmaTypeText,
                    equipField,
                    _defaultKarmaTypeText,
                    item.Data.ItemData!.KarmaTypeForEquipping,
                    k => item.Data.ItemData.KarmaTypeForEquipping = k,
                    item.Data.ItemData.KarmaTypeForEquipping != null,
                    _resetItemKarmaTooltip
                );
            }

            if (item.Data.IsUsable)
            {
                row += 1;
                (Rect usableLabel, Rect usableField) = new Rect(0f, row * RowLineHeight, canvas.width, RowLineHeight).Split();

                SettingsHelper.DrawKarmaField(
                    usableLabel,
                    _useKarmaTypeText,
                    usableField,
                    _defaultKarmaTypeText,
                    item.Data.ItemData!.KarmaTypeForUsing,
                    k => item.Data.ItemData.KarmaTypeForUsing = k,
                    item.Data.ItemData.KarmaTypeForUsing != null,
                    _resetItemKarmaTooltip
                );
            }

            row += 1;
            var weightBuffer = item.Data.ItemData!.Weight.ToString("N1");
            (Rect weightLabel, Rect weightField) = new Rect(0f, row * RowLineHeight, canvas.width, RowLineHeight).Split();
            UiHelper.Label(weightLabel, _purchaseWeightText);

            float proxy = item.Data.ItemData.Weight;

            if (SettingsHelper.DrawNumberField(weightField, ref proxy, ref weightBuffer, out float value))
            {
                item.Data.ItemData.Weight = value;
            }
        }

        private void DrawRightExpandedSettingsColumn(Rect canvas, [NotNull] TableSettingsItem<ThingItem> item)
        {
            var row = 0;
            bool proxy = item.Data.ItemData!.HasQuantityLimit;

            if (UiHelper.LabeledPaintableCheckbox(
                new Rect(0f, 0f, canvas.width - (item.Data.ItemData!.HasQuantityLimit ? Mathf.FloorToInt(canvas.width * 0.2f) + 2f : 0f), RowLineHeight),
                _quantityLimitText,
                ref proxy
            ))
            {
                item.Data.ItemData.HasQuantityLimit = proxy;
            }

            if (item.Data.ItemData.HasQuantityLimit)
            {
                var quantityBuffer = item.Data.ItemData.QuantityLimit.ToString();
                var quantityField = new Rect(Mathf.FloorToInt(canvas.width * 0.8f), 0, Mathf.FloorToInt(canvas.width * 0.2f), RowLineHeight);

                int proxy2 = item.Data.ItemData.QuantityLimit;

                if (SettingsHelper.DrawNumberField(quantityField, ref proxy2, ref quantityBuffer, out int newValue, 1f))
                {
                    item.Data.ItemData.QuantityLimit = newValue;
                }

                if (UiHelper.FieldButton(quantityField, Textures.Stack, _stackLimitTooltip))
                {
                    item.Data.ItemData.QuantityLimit = item.Data.Thing?.stackLimit ?? item.Data.ItemData.QuantityLimit;
                }
            }

            if (item.Data.Thing is { IsStuff: true })
            {
                row += 1;
                bool proxy3 = item.Data.ItemData.IsStuffAllowed;

                if (UiHelper.LabeledPaintableCheckbox(new Rect(0f, row * RowLineHeight, canvas.width, RowLineHeight), _isStuffText, ref proxy3))
                {
                    item.Data.ItemData.IsStuffAllowed = proxy3;
                }
            }

            if (item.Data.Thing?.IsApparel == true)
            {
                row += 1;
                bool proxy4 = item.Data.ItemData.IsWearable;

                if (UiHelper.LabeledPaintableCheckbox(new Rect(0f, row * RowLineHeight, canvas.width, RowLineHeight), _isWearableText, ref proxy4))
                {
                    item.Data.ItemData.IsWearable = proxy4;
                }
            }

            if (item.Data.Thing?.IsWeapon == true)
            {
                row += 1;
                bool proxy5 = item.Data.ItemData.IsEquippable;

                if (UiHelper.LabeledPaintableCheckbox(new Rect(0f, row * RowLineHeight, canvas.width, RowLineHeight), _isEquippableText, ref proxy5))
                {
                    item.Data.ItemData.IsEquippable = proxy5;
                }
            }

            if (item.Data.IsUsable)
            {
                row += 1;
                bool proxy6 = item.Data.ItemData.IsUsable;

                if (UiHelper.LabeledPaintableCheckbox(new Rect(0f, row * RowLineHeight, canvas.width, RowLineHeight), _isUsableText, ref proxy6))
                {
                    item.Data.ItemData.IsUsable = proxy6;
                }
            }
        }

        /// <inheritdoc cref="TableWorker{T}.EnsureExists"/>
        public override void EnsureExists(TableSettingsItem<ThingItem> data)
        {
            if (!InternalData.Any(i => i.Data.DefName.Equals(data.Data.DefName)))
            {
                InternalData.Add(data);
            }
        }

        /// <inheritdoc cref="TableWorker{T}.NotifyGlobalDataChanged"/>
        public override void NotifyGlobalDataChanged()
        {
            var wasDirty = false;

            foreach (ThingItem item in ToolkitUtils.Data.Items.Select(item => new { item, existing = InternalData.Find(i => i.Data.Equals(item)) })
               .Where(t => t.existing == null)
               .Select(t => t.item))
            {
                InternalData.Add(new TableSettingsItem<ThingItem> { Data = item });
                wasDirty = true;
            }

            if (wasDirty)
            {
                NotifySortRequested();
            }
        }

        private void LoadTranslations()
        {
            _nameHeaderText = "TKUtils.Headers.Name".Localize();
            _priceHeaderText = "TKUtils.Headers.Price".Localize();
            _categoryHeaderText = "TKUtils.Headers.Category".Localize();

            _quantityLimitText = "TKUtils.Fields.QuantityLimit".Localize();
            _isStuffText = "TKUtils.Fields.CanBeStuff".Localize();
            _karmaTypeText = "TKUtils.Fields.KarmaType".Localize();
            _purchaseWeightText = "TKUtils.Fields.Weight".Localize();
            _defaultKarmaTypeText = "TKUtils.Fields.DefaultKarmaType".Localize();
            _useKarmaTypeText = "TKUtils.Fields.UseKarmaType".Localize();
            _wearKarmaTypeText = "TKUtils.Fields.WearKarmaType".Localize();
            _equipKarmaTypeText = "TKUtils.Fields.EquipKarmaType".Localize();
            _isEquippableText = "TKUtils.Fields.CanEquip".Localize();
            _isUsableText = "TKUtils.Fields.CanUse".Localize();
            _isWearableText = "TKUtils.Fields.CanWear".Localize();

            _stackLimitTooltip = "TKUtils.ItemTableTooltips.ToStackLimit".Localize();
            _editItemNameTooltip = "TKUtils.ItemTableTooltips.EditItemName".Localize();
            _closeItemNameTooltip = "TKUtils.ItemTableTooltips.CloseItemName".Localize();
            _resetItemNameTooltip = "TKUtils.ItemTableTooltips.ResetItemName".Localize();
            _resetItemKarmaTooltip = "TKUtils.ItemTableTooltips.ResetItemKarma".Localize();
        }

        /// <inheritdoc cref="TableWorkerBase.NotifySortRequested"/>
        public override void NotifySortRequested()
        {
            switch (_sortOrder)
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
            switch (_sortKey)
            {
                case SortKey.Price:
                    InternalData = InternalData.OrderBy(i => i.Data.Cost).ThenBy(i => i.Data.Name).ToList();

                    return;
                case SortKey.Category:
                    InternalData = InternalData.OrderBy(i => i.Data.Category).ThenBy(i => i.Data.Name).ToList();

                    return;
                default:
                    InternalData = InternalData.OrderBy(i => i.Data.Name).ToList();

                    return;
            }
        }

        private void NotifyAscendingSortRequested()
        {
            switch (_sortKey)
            {
                case SortKey.Price:
                    InternalData = InternalData.OrderByDescending(i => i.Data.Cost).ThenByDescending(i => i.Data.Name).ToList();

                    return;
                case SortKey.Category:
                    InternalData = InternalData.OrderByDescending(i => i.Data.Category).ThenByDescending(i => i.Data.Name).ToList();

                    return;
                default:
                    InternalData = InternalData.OrderByDescending(i => i.Data.Name).ToList();

                    return;
            }
        }

        /// <inheritdoc cref="TableWorkerBase.NotifySearchRequested"/>
        public override void NotifySearchRequested(string query)
        {
            FilterDataBySearch(query);
        }

        /// <inheritdoc cref="TableWorkerBase.NotifyResolutionChanged"/>
        public override void NotifyResolutionChanged(Rect region)
        {
            float consumedWidth = region.width - 18f - LineHeight * 2f; // Icon buttons
            float distributedWidth = Mathf.FloorToInt(consumedWidth * 0.36f);
            float remainingWidth = consumedWidth - distributedWidth * 2f;
            _stateHeaderRect = new Rect(0f, 0f, LineHeight, LineHeight);
            _stateHeaderInnerRect = _stateHeaderRect.ContractedBy(2f);
            NameHeaderRect = new Rect(LineHeight + 1f, 0f, distributedWidth, LineHeight);
            NameHeaderTextRect = new Rect(NameHeaderRect.x + 4f, NameHeaderRect.y, NameHeaderRect.width - 8f, NameHeaderRect.height);
            PriceHeaderRect = new Rect(NameHeaderRect.x + NameHeaderRect.width + 1f, 0f, remainingWidth, LineHeight);
            PriceHeaderTextRect = new Rect(PriceHeaderRect.x + 4f, PriceHeaderRect.y, PriceHeaderRect.width - 8f, PriceHeaderRect.height);
            CategoryHeaderRect = new Rect(PriceHeaderRect.x + PriceHeaderRect.width + 1f, 0f, distributedWidth, LineHeight);
            CategoryHeaderTextRect = new Rect(CategoryHeaderRect.x + 4f, CategoryHeaderRect.y, CategoryHeaderRect.width - 8f, CategoryHeaderRect.height);
            _expandedHeaderRect = new Rect(CategoryHeaderRect.x + CategoryHeaderRect.width + 1f, 0f, LineHeight, LineHeight);
            _expandedHeaderInnerRect = _expandedHeaderRect.ContractedBy(2f);
        }

        /// <inheritdoc cref="TableWorker{T}.NotifyCustomSearchRequested"/>
        public override void NotifyCustomSearchRequested(Func<TableSettingsItem<ThingItem>, bool> worker)
        {
            foreach (TableSettingsItem<ThingItem> item in Data)
            {
                item.IsHidden = !worker(item);
            }
        }

        /// <inheritdoc cref="TableWorkerBase.FilterDataBySearch"/>
        private protected override void FilterDataBySearch(string query)
        {
            foreach (TableSettingsItem<ThingItem> item in Data)
            {
                item.IsHidden = !query.NullOrEmpty() && !item.Data.Name.ToLower().Contains(query.ToLower());
            }
        }

        private enum SortKey { Name, Price, Category }
    }
}
