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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    /// <summary>
    ///     A class for drawing the trait store in a portable, reusable way.
    /// </summary>
    public class TraitTableWorker : TableWorker<TableSettingsItem<TraitItem>>
    {
        private const float ExpandedLineSpan = 3f;
        private string _addKarmaTypeText;
        private string _addPriceHeaderText;
        private Rect _addStateHeaderInnerRect = Rect.zero;
        private Rect _addStateHeaderRect = Rect.zero;
        private StateKey _addStateKey = StateKey.Enable;
        private string _bypassLimitText;
        private string _closeTraitNameTooltip;
        private string _defaultKarmaTypeText;
        private string _editTraitNameTooltip;
        private Rect _expandedHeaderInnerRect = Rect.zero;
        private Rect _expandedHeaderRect = Rect.zero;
        private string _nameHeaderText;
        private string _removeKarmaTypeText;
        private string _removePriceHeaderText;
        private Rect _removeStateHeaderInnerRect = Rect.zero;
        private Rect _removeStateHeaderRect = Rect.zero;
        private StateKey _removeStateKey = StateKey.Enable;
        private string _resetTraitKarmaTooltip;
        private string _resetTraitNameTooltip;
        private Vector2 _scrollPos = Vector2.zero;
        private SettingsKey _settingsKey = SettingsKey.Collapse;
        private SortKey _sortKey = SortKey.Name;
        private SortOrder _sortOrder = SortOrder.Descending;
        private protected Rect AddPriceHeaderRect = Rect.zero;
        private protected Rect AddPriceHeaderTextRect = Rect.zero;
        private protected Rect NameHeaderRect = Rect.zero;
        private protected Rect NameHeaderTextRect = Rect.zero;
        private protected Rect RemovePriceHeaderRect = Rect.zero;
        private protected Rect RemovePriceHeaderTextRect = Rect.zero;

        protected override void DrawHeaders(Rect region)
        {
            if (SettingsHelper.DrawTableHeader(
                _addStateHeaderRect,
                _addStateHeaderInnerRect,
                _addStateKey == StateKey.Enable ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                _addStateKey = _addStateKey == StateKey.Enable ? StateKey.Disable : StateKey.Enable;
                NotifyGlobalAddStateChanged(_addStateKey);
            }

            if (SettingsHelper.DrawTableHeader(
                _removeStateHeaderRect,
                _removeStateHeaderInnerRect,
                _removeStateKey == StateKey.Enable ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                _removeStateKey = _removeStateKey == StateKey.Enable ? StateKey.Disable : StateKey.Enable;
                NotifyGlobalRemoveStateChanged(_removeStateKey);
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
                case SortKey.AddPrice:
                    UiHelper.SortIndicator(AddPriceHeaderRect, _sortOrder);

                    return;
                case SortKey.RemovePrice:
                    UiHelper.SortIndicator(RemovePriceHeaderRect, _sortOrder);

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

            if (SettingsHelper.DrawTableHeader(AddPriceHeaderRect, AddPriceHeaderTextRect, _addPriceHeaderText))
            {
                _sortKey = SortKey.AddPrice;
                anyClicked = true;
            }

            if (SettingsHelper.DrawTableHeader(RemovePriceHeaderRect, RemovePriceHeaderTextRect, _removePriceHeaderText))
            {
                _sortKey = SortKey.RemovePrice;
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
            foreach (TableSettingsItem<TraitItem> item in Data.Where(i => !i.IsHidden))
            {
                item.SettingsVisible = newState == SettingsKey.Expand;
            }
        }

        private void NotifyGlobalAddStateChanged(StateKey newState)
        {
            foreach (TableSettingsItem<TraitItem> item in Data.Where(i => !i.IsHidden))
            {
                item.Data.CanAdd = newState == StateKey.Enable;
            }
        }

        private void NotifyGlobalRemoveStateChanged(StateKey newState)
        {
            foreach (TableSettingsItem<TraitItem> item in Data.Where(i => !i.IsHidden))
            {
                item.Data.CanRemove = newState == StateKey.Enable;
            }
        }

        protected override void DrawTableContents(Rect region)
        {
            float expectedLines = Data.Where(i => !i.IsHidden).Sum(i => i.SettingsVisible ? ExpandedLineSpan + 1f : 1f);
            var viewPort = new Rect(0f, 0f, region.width - 16f, RowLineHeight * expectedLines);

            var index = 0;
            var expanded = 0;
            var alternate = false;
            GUI.BeginGroup(region);
            _scrollPos = GUI.BeginScrollView(region, _scrollPos, viewPort);

            foreach (TableSettingsItem<TraitItem> trait in Data.Where(i => !i.IsHidden))
            {
                var lineRect = new Rect(
                    0f,
                    index * RowLineHeight + RowLineHeight * ExpandedLineSpan * expanded,
                    region.width - 16f,
                    RowLineHeight * (trait.SettingsVisible ? ExpandedLineSpan + 1f : 1f)
                );

                if (!lineRect.IsVisible(region, _scrollPos))
                {
                    index++;
                    alternate = !alternate;
                    expanded += trait.SettingsVisible ? 1 : 0;

                    continue;
                }

                GUI.BeginGroup(lineRect);
                Rect rect = lineRect.AtZero();

                if (alternate)
                {
                    Widgets.DrawLightHighlight(rect);
                }

                DrawTrait(rect, trait);
                GUI.EndGroup();

                alternate = !alternate;
                index++;

                if (trait.SettingsVisible)
                {
                    expanded++;
                }
            }

            GUI.EndScrollView();
            GUI.EndGroup();
        }

        protected virtual void DrawTrait(Rect canvas, [NotNull] TableSettingsItem<TraitItem> trait)
        {
            var nameMouseOverRect = new Rect(NameHeaderRect.x, canvas.y, NameHeaderRect.width, RowLineHeight);
            var nameRect = new Rect(NameHeaderTextRect.x, canvas.y, NameHeaderTextRect.width, RowLineHeight);
            Rect addCheckRect = LayoutHelper.IconRect(_addStateHeaderRect.x + 2f, canvas.y + 2f, _addStateHeaderRect.width - 4f, RowLineHeight - 4f);
            var addPriceRect = new Rect(AddPriceHeaderTextRect.x, canvas.y, AddPriceHeaderTextRect.width, RowLineHeight);
            Rect removeCheckRect = LayoutHelper.IconRect(_removeStateHeaderRect.x + 2f, canvas.y + 2f, _removeStateHeaderRect.width - 4f, RowLineHeight - 4f);
            var removePriceRect = new Rect(RemovePriceHeaderTextRect.x, canvas.y, RemovePriceHeaderTextRect.width, RowLineHeight);

            Rect settingRect = LayoutHelper.IconRect(
                _expandedHeaderRect.x + 2f,
                canvas.y + Mathf.FloorToInt(Mathf.Abs(_expandedHeaderRect.width - RowLineHeight) / 2f) + 2f,
                _expandedHeaderRect.width - 4f,
                _expandedHeaderRect.width - 4f
            );

            DrawConfigurableTraitName(nameRect, trait);

            if (!trait.EditingName)
            {
                Widgets.DrawHighlightIfMouseover(nameMouseOverRect);

                var builder = new StringBuilder();
                builder.AppendLine(trait.Data.Description);
                builder.AppendLine();

                foreach (string i in trait.Data.Stats)
                {
                    builder.AppendLine(i);
                }

                TooltipHandler.TipRegion(nameMouseOverRect, builder.ToString());
            }

            Widgets.Checkbox(addCheckRect.x, addCheckRect.y, ref trait.Data.CanAdd, addCheckRect.height, paintable: true);

            if (trait.Data.CanAdd)
            {
                SettingsHelper.DrawPriceField(addPriceRect, ref trait.Data.CostToAdd);
            }

            Widgets.Checkbox(removeCheckRect.x, removeCheckRect.y, ref trait.Data.CanRemove, removeCheckRect.height, paintable: true);

            if (trait.Data.CanRemove)
            {
                SettingsHelper.DrawPriceField(removePriceRect, ref trait.Data.CostToRemove);
            }

            if (Widgets.ButtonImage(settingRect, Textures.Gear))
            {
                trait.SettingsVisible = !trait.SettingsVisible;
            }

            if (!trait.SettingsVisible)
            {
                return;
            }

            var expandedRect = new Rect(
                NameHeaderRect.x + 10f,
                canvas.y + RowLineHeight + 10f,
                canvas.width - settingRect.width * 2f - 20f,
                canvas.height - RowLineHeight - 20f
            );

            GUI.BeginGroup(expandedRect);
            DrawExpandedSettings(expandedRect.AtZero(), trait);
            GUI.EndGroup();
        }

        private void DrawConfigurableTraitName(Rect canvas, [NotNull] TableSettingsItem<TraitItem> trait)
        {
            if (trait.EditingName)
            {
                var fieldRect = new Rect(canvas.x, canvas.y, canvas.width - canvas.height, canvas.height);

                if (UiHelper.TextField(fieldRect, trait.Data.Name, out string result))
                {
                    trait.Data.Name = result.ToToolkit();
                    trait.Data.TraitData!.CustomName = true;
                }

                if (trait.Data.TraitData!.CustomName && UiHelper.FieldButton(fieldRect, Textures.Reset, _resetTraitNameTooltip))
                {
                    trait.Data.TraitData.CustomName = false;
                    trait.Data.Name = trait.Data.GetDefaultName();
                }
            }
            else
            {
                UiHelper.Label(canvas, trait.Data.Name);
            }

            GUI.color = new Color(1f, 1f, 1f, 0.7f);

            if (UiHelper.FieldButton(
                canvas,
                trait.EditingName ? Widgets.CheckboxOffTex : Textures.Edit,
                trait.EditingName ? _closeTraitNameTooltip : _editTraitNameTooltip
            ))
            {
                trait.EditingName = !trait.EditingName;
            }

            GUI.color = Color.white;
        }

        /// <inheritdoc cref="TableWorkerBase.Prepare"/>
        public override void Prepare()
        {
            LoadTranslations();

            InternalData ??= new List<TableSettingsItem<TraitItem>>();
            InternalData.AddRange(ToolkitUtils.Data.Traits.OrderBy(i => i.Name).Select(i => new TableSettingsItem<TraitItem> { Data = i }));
        }

        private void DrawExpandedSettings(Rect canvas, [NotNull] TableSettingsItem<TraitItem> trait)
        {
            float columnWidth = Mathf.FloorToInt(canvas.width / 2f) - 26f;
            var leftColumnRect = new Rect(canvas.x, canvas.y, columnWidth, canvas.height);
            var rightColumnRect = new Rect(canvas.x + leftColumnRect.width + 52f, canvas.y, columnWidth, canvas.height);

            Widgets.DrawLineVertical(Mathf.FloorToInt(canvas.width / 2f), 0f, canvas.height - 5f);

            GUI.BeginGroup(leftColumnRect);
            DrawLeftExpandedSettingsColumn(leftColumnRect.AtZero(), trait);
            GUI.EndGroup();

            GUI.BeginGroup(rightColumnRect);
            DrawRightExpandedSettingsColumn(rightColumnRect.AtZero(), trait);
            GUI.EndGroup();
        }

        private void DrawLeftExpandedSettingsColumn(Rect canvas, [NotNull] ITableItem<TraitItem> trait)
        {
            (Rect addKarmaLabel, Rect addKarmaField) = new Rect(0f, 0f, canvas.width, RowLineHeight).Split();
            UiHelper.Label(addKarmaLabel, _addKarmaTypeText);

            if (Widgets.ButtonText(addKarmaField, trait.Data.Data.KarmaType == null ? _defaultKarmaTypeText : trait.Data.Data.KarmaType.ToString()))
            {
                Find.WindowStack.Add(
                    new FloatMenu(ToolkitUtils.Data.KarmaTypes.Values.Select(i => new FloatMenuOption(i.ToString(), () => trait.Data.Data.KarmaType = i)).ToList())
                );
            }

            if (trait.Data.Data.KarmaType != null && UiHelper.FieldButton(addKarmaLabel, Textures.Reset, _resetTraitKarmaTooltip))
            {
                trait.Data.Data.KarmaType = null;
            }

            (Rect removeKarmaLabel, Rect removeKarmaField) = new Rect(0f, RowLineHeight, canvas.width, RowLineHeight).Split();
            UiHelper.Label(removeKarmaLabel, _removeKarmaTypeText);

            if (Widgets.ButtonText(
                removeKarmaField,
                trait.Data.TraitData!.KarmaTypeForRemoving == null ? _defaultKarmaTypeText : trait.Data.TraitData.KarmaTypeForRemoving.ToString()
            ))
            {
                Find.WindowStack.Add(
                    new FloatMenu(ToolkitUtils.Data.KarmaTypes.Values.Select(i => new FloatMenuOption(i.ToString(), () => trait.Data.TraitData.KarmaTypeForRemoving = i)).ToList())
                );
            }

            if (trait.Data.TraitData.KarmaTypeForRemoving != null && UiHelper.FieldButton(removeKarmaLabel, Textures.Reset, _resetTraitKarmaTooltip))
            {
                trait.Data.TraitData.KarmaTypeForRemoving = null;
            }
        }

        private void DrawRightExpandedSettingsColumn(Rect canvas, [NotNull] ITableItem<TraitItem> trait)
        {
            bool proxy = trait.Data.TraitData!.CanBypassLimit;

            if (UiHelper.LabeledPaintableCheckbox(new Rect(0f, 0f, canvas.width, RowLineHeight), _bypassLimitText, ref proxy))
            {
                trait.Data.TraitData.CanBypassLimit = proxy;
            }
        }

        /// <inheritdoc cref="TableWorker{T}.EnsureExists"/>
        public override void EnsureExists(TableSettingsItem<TraitItem> data)
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

            foreach (TraitItem item in ToolkitUtils.Data.Traits.Select(item => new { item, existing = InternalData.Find(i => i.Data.Equals(item)) })
               .Where(t => t.existing == null)
               .Select(t => t!.item))
            {
                InternalData.Add(new TableSettingsItem<TraitItem> { Data = item });
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
            _addPriceHeaderText = "TKUtils.Headers.AddPrice".TranslateSimple();
            _removePriceHeaderText = "TKUtils.Headers.RemovePrice".TranslateSimple();

            _bypassLimitText = "TKUtils.Fields.BypassTraitLimit".TranslateSimple();
            _defaultKarmaTypeText = "TKUtils.Fields.DefaultKarmaType".TranslateSimple();
            _addKarmaTypeText = "TKUtils.Fields.AddKarmaType".TranslateSimple();
            _removeKarmaTypeText = "TKUtils.Fields.RemoveKarmaType".TranslateSimple();

            _editTraitNameTooltip = "TKUtils.TraitTableTooltips.EditTraitName".TranslateSimple();
            _closeTraitNameTooltip = "TKUtils.TraitTableTooltips.CloseTraitName".TranslateSimple();
            _resetTraitNameTooltip = "TKUtils.TraitTableTooltips.ResetTraitName".TranslateSimple();
            _resetTraitKarmaTooltip = "TKUtils.TraitTableTooltips.ResetTraitKarma".TranslateSimple();
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
                case SortKey.AddPrice:
                    InternalData = InternalData.OrderBy(i => i.Data.CostToAdd).ThenBy(i => i.Data.Name).ToList();

                    return;
                case SortKey.RemovePrice:
                    InternalData = InternalData.OrderBy(i => i.Data.CostToRemove).ThenBy(i => i.Data.Name).ToList();

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
                case SortKey.AddPrice:
                    InternalData = InternalData.OrderByDescending(i => i.Data.CostToAdd).ThenByDescending(i => i.Data.Name).ToList();

                    return;
                case SortKey.RemovePrice:
                    InternalData = InternalData.OrderByDescending(i => i.Data.CostToRemove).ThenByDescending(i => i.Data.Name).ToList();

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
            float consumedWidth = region.width - 18f - LineHeight * 3f; // Icon buttons
            float nameWidth = Mathf.FloorToInt(consumedWidth * 0.45f);
            float distributedWidth = Mathf.FloorToInt((consumedWidth - nameWidth) * 0.5f);
            NameHeaderRect = new Rect(0f, 0f, nameWidth, LineHeight);
            NameHeaderTextRect = new Rect(NameHeaderRect.x + 4f, NameHeaderRect.y, NameHeaderRect.width - 8f, NameHeaderRect.height);
            _addStateHeaderRect = new Rect(NameHeaderRect.x + NameHeaderRect.width + 1f, 0f, LineHeight, LineHeight);
            _addStateHeaderInnerRect = _addStateHeaderRect.ContractedBy(2f);
            AddPriceHeaderRect = new Rect(_addStateHeaderRect.x + _addStateHeaderRect.width + 1f, 0f, distributedWidth, LineHeight);
            AddPriceHeaderTextRect = new Rect(AddPriceHeaderRect.x + 4f, AddPriceHeaderRect.y, AddPriceHeaderRect.width - 8f, AddPriceHeaderRect.height);
            _removeStateHeaderRect = new Rect(AddPriceHeaderRect.x + AddPriceHeaderRect.width + 1f, 0f, LineHeight, LineHeight);
            _removeStateHeaderInnerRect = _removeStateHeaderRect.ContractedBy(2f);
            RemovePriceHeaderRect = new Rect(_removeStateHeaderRect.x + _removeStateHeaderRect.width + 1f, 0f, distributedWidth, LineHeight);
            RemovePriceHeaderTextRect = new Rect(RemovePriceHeaderRect.x + 4f, RemovePriceHeaderRect.y, RemovePriceHeaderRect.width - 8f, RemovePriceHeaderRect.height);
            _expandedHeaderRect = new Rect(RemovePriceHeaderRect.x + RemovePriceHeaderRect.width + 1f, 0f, LineHeight, LineHeight);
            _expandedHeaderInnerRect = _expandedHeaderRect.ContractedBy(2f);
        }

        /// <inheritdoc cref="TableWorker{T}.NotifyCustomSearchRequested"/>
        public override void NotifyCustomSearchRequested(Func<TableSettingsItem<TraitItem>, bool> worker)
        {
            foreach (TableSettingsItem<TraitItem> item in Data)
            {
                item.IsHidden = !worker(item);
            }
        }

        private protected override void FilterDataBySearch(string query)
        {
            foreach (TableSettingsItem<TraitItem> item in Data)
            {
                item.IsHidden = !query.NullOrEmpty() && !item.Data.Name.ToLower().Contains(query.ToLower());
            }
        }

        private enum SortKey { Name, AddPrice, RemovePrice }
    }
}
