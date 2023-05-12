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
using System.Text;
using JetBrains.Annotations;
using SirRandoo.CommonLib.Enums;
using SirRandoo.CommonLib.Helpers;
using ToolkitUtils.Helpers;
using ToolkitUtils.Interfaces;
using ToolkitUtils.Models.Serialization;
using ToolkitUtils.Models.Tables;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Workers
{
    /// <summary>
    ///     A class for drawing the pawn kind shop's data in a portable way.
    /// </summary>
    public class PawnTableWorker : TableWorker<TableSettingsItem<PawnKindItem>>
    {
        private const float ExpandedLineSpan = 2f;

        private string _closePawnNameTooltip;
        private string _defaultKarmaTypeText;
        private string _editPawnNameTooltip;
        private Rect _expandedHeaderInnerRect = Rect.zero;
        private Rect _expandedHeaderRect = Rect.zero;
        private string _karmaTypeText;
        private string _nameHeaderText;
        private string _priceHeaderText;
        private string _resetPawnKarmaTooltip;
        private string _resetPawnNameTooltip;
        private Vector2 _scrollPos = Vector2.zero;
        private SettingsKey _settingsKey = SettingsKey.Collapse;

        private SortKey _sortKey = SortKey.Name;
        private SortOrder _sortOrder = SortOrder.Descending;
        private Rect _stateHeaderInnerRect = Rect.zero;

        private Rect _stateHeaderRect = Rect.zero;
        private StateKey _stateKey = StateKey.Enable;
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
            foreach (TableSettingsItem<PawnKindItem> item in Data.Where(i => !i.IsHidden))
            {
                item.SettingsVisible = newState == SettingsKey.Expand;
            }
        }

        private void NotifyGlobalStateChanged(StateKey newState)
        {
            foreach (TableSettingsItem<PawnKindItem> item in Data.Where(i => !i.IsHidden))
            {
                item.Data.Enabled = newState == StateKey.Enable;
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

            foreach (TableSettingsItem<PawnKindItem> item in Data.Where(i => !i.IsHidden))
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

                DrawKind(rect, item);
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
        ///     Draws a <see cref="PawnKindItem"/> in a given row of the
        ///     <see cref="PawnTableWorker"/> area.
        /// </summary>
        /// <param name="region">
        ///     The region to draw the
        ///     <see cref="PawnKindItem"/> in
        /// </param>
        /// <param name="item">
        ///     The <see cref="PawnKindItem"/> to draw in the
        ///     region
        /// </param>
        protected virtual void DrawKind(Rect region, [NotNull] TableSettingsItem<PawnKindItem> item)
        {
            Rect checkboxRect = LayoutHelper.IconRect(_stateHeaderRect.x + 2f, region.y + 2f, _stateHeaderRect.width - 4f, RowLineHeight - 4f);
            var nameMouseOverRect = new Rect(NameHeaderRect.x, region.y, NameHeaderRect.width, RowLineHeight);
            var nameRect = new Rect(NameHeaderTextRect.x, region.y, NameHeaderTextRect.width, RowLineHeight);
            var priceRect = new Rect(PriceHeaderTextRect.x, region.y, PriceHeaderTextRect.width, RowLineHeight);

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
            }

            DrawConfigurableItemName(nameRect, item);

            if (!item.EditingName)
            {
                Widgets.DrawHighlightIfMouseover(nameMouseOverRect);

                var builder = new StringBuilder();

                if (!item.Data.Description.NullOrEmpty())
                {
                    builder.AppendLine(item.Data.Description);
                    builder.AppendLine();
                }

                foreach (string i in item.Data.PawnData.Stats)
                {
                    builder.AppendLine(i);
                }

                TooltipHandler.TipRegion(nameMouseOverRect, builder.ToString());
            }

            if (item.Data.Enabled)
            {
                int cost = item.Data.Cost;
                SettingsHelper.DrawPriceField(priceRect, ref cost);
                item.Data.Cost = cost;
            }

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

        private void DrawConfigurableItemName(Rect region, [NotNull] TableSettingsItem<PawnKindItem> item)
        {
            if (item.EditingName)
            {
                var fieldRect = new Rect(region.x, region.y, region.width - region.height, region.height);

                if (UiHelper.TextField(fieldRect, item.Data.Name, out string result))
                {
                    item.Data.Name = result.ToToolkit();
                    item.Data.PawnData.CustomName = true;
                }

                if (item.Data.PawnData.CustomName && UiHelper.FieldButton(fieldRect, Textures.Reset, _resetPawnNameTooltip))
                {
                    item.Data.PawnData.CustomName = false;
                }
            }
            else
            {
                UiHelper.Label(region, item.Data.Name);
            }

            GUI.color = new Color(1f, 1f, 1f, 0.7f);

            if (UiHelper.FieldButton(region, item.EditingName ? Widgets.CheckboxOffTex : Textures.Edit, item.EditingName ? _closePawnNameTooltip : _editPawnNameTooltip))
            {
                item.EditingName = !item.EditingName;
            }

            GUI.color = Color.white;
        }

        /// <inheritdoc cref="TableWorkerBase.Prepare"/>
        public override void Prepare()
        {
            LoadTranslations();

            InternalData ??= new List<TableSettingsItem<PawnKindItem>>();
            InternalData.AddRange(ToolkitUtils.Data.PawnKinds.OrderBy(i => i.Name).Select(i => new TableSettingsItem<PawnKindItem> { Data = i }));
        }

        private void DrawExpandedSettings(Rect region, [NotNull] TableSettingsItem<PawnKindItem> item)
        {
            float columnWidth = Mathf.FloorToInt(region.width / 2f) - 26f;
            var leftColumnRect = new Rect(region.x, region.y, columnWidth, region.height);
            var rightColumnRect = new Rect(region.x + leftColumnRect.width + 52f, region.y, columnWidth, region.height);

            Widgets.DrawLineVertical(Mathf.FloorToInt(region.width / 2f), 0f, region.height - 5f);

            GUI.BeginGroup(leftColumnRect);
            DrawLeftExpandedSettingsColumn(leftColumnRect.AtZero(), item);
            GUI.EndGroup();

            GUI.BeginGroup(rightColumnRect);
            DrawRightExpandedSettingsColumn(rightColumnRect.AtZero(), item);
            GUI.EndGroup();
        }

        private void DrawLeftExpandedSettingsColumn(Rect region, [NotNull] ITableItem<PawnKindItem> item)
        {
            (Rect karmaLabel, Rect karmaField) = new Rect(0f, 0f, region.width, RowLineHeight).Split(0.6f);
            UiHelper.Label(karmaLabel, _karmaTypeText);

            if (Widgets.ButtonText(karmaField, item.Data.Data.KarmaType == null ? _defaultKarmaTypeText : item.Data.Data.KarmaType.ToString()))
            {
                Find.WindowStack.Add(
                    new FloatMenu(ToolkitUtils.Data.KarmaTypes.Values.Select(i => new FloatMenuOption(i.ToString(), () => item.Data.Data.KarmaType = i)).ToList())
                );
            }

            if (item.Data.Data.KarmaType != null && UiHelper.FieldButton(karmaLabel, Textures.Reset, _resetPawnKarmaTooltip))
            {
                item.Data.Data.KarmaType = null;
            }
        }

        private void DrawRightExpandedSettingsColumn(Rect region, TableSettingsItem<PawnKindItem> item)
        {
            // unused
        }

        /// <inheritdoc cref="TableWorker{T}.EnsureExists"/>
        public override void EnsureExists(TableSettingsItem<PawnKindItem> data)
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

            foreach (PawnKindItem item in ToolkitUtils.Data.PawnKinds.Select(item => new { item, existing = InternalData.Find(i => i.Data.Equals(item)) })
               .Where(t => t.existing == null)
               .Select(t => t!.item))
            {
                InternalData.Add(new TableSettingsItem<PawnKindItem> { Data = item });
                wasDirty = true;
            }

            if (wasDirty)
            {
                NotifySortRequested();
            }
        }

        private void LoadTranslations()
        {
            _nameHeaderText = "TKUtils.Headers.Name".TranslateSimple();
            _priceHeaderText = "TKUtils.Headers.Price".TranslateSimple();

            _karmaTypeText = "TKUtils.Fields.KarmaType".TranslateSimple();
            _defaultKarmaTypeText = "TKUtils.Fields.DefaultKarmaType".TranslateSimple();

            _editPawnNameTooltip = "TKUtils.PawnTableTooltips.EditPawnName".TranslateSimple();
            _closePawnNameTooltip = "TKUtils.PawnTableTooltips.ClosePawnName".TranslateSimple();
            _resetPawnNameTooltip = "TKUtils.PawnTableTooltips.ResetPawnName".TranslateSimple();
            _resetPawnKarmaTooltip = "TKUtils.PawnTableTooltips.ResetPawnKarma".TranslateSimple();
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
            _stateHeaderRect = new Rect(0f, 0f, LineHeight, LineHeight);
            _stateHeaderInnerRect = _stateHeaderRect.ContractedBy(2f);
            NameHeaderRect = new Rect(LineHeight + 1f, 0f, Mathf.FloorToInt(consumedWidth * 0.55f), LineHeight);
            NameHeaderTextRect = new Rect(NameHeaderRect.x + 4f, NameHeaderRect.y, NameHeaderRect.width - 8f, NameHeaderRect.height);
            PriceHeaderRect = new Rect(NameHeaderRect.x + NameHeaderRect.width + 1f, 0f, Mathf.FloorToInt(consumedWidth * 0.45f), LineHeight);
            PriceHeaderTextRect = new Rect(PriceHeaderRect.x + 4f, PriceHeaderRect.y, PriceHeaderRect.width - 8f, PriceHeaderRect.height);
            _expandedHeaderRect = new Rect(PriceHeaderRect.x + PriceHeaderRect.width + 1f, 0f, LineHeight, LineHeight);
            _expandedHeaderInnerRect = _expandedHeaderRect.ContractedBy(2f);
        }

        /// <inheritdoc cref="TableWorker{T}.NotifyCustomSearchRequested"/>
        public override void NotifyCustomSearchRequested(Func<TableSettingsItem<PawnKindItem>, bool> worker)
        {
            foreach (TableSettingsItem<PawnKindItem> item in Data)
            {
                item.IsHidden = !worker(item);
            }
        }

        /// <inheritdoc cref="TableWorkerBase.FilterDataBySearch"/>
        private protected override void FilterDataBySearch(string query)
        {
            foreach (TableSettingsItem<PawnKindItem> item in Data)
            {
                item.IsHidden = !query.NullOrEmpty() && !item.Data.Name.ToLower().Contains(query.ToLower());
            }
        }

        private enum SortKey { Name, Price }
    }
}
