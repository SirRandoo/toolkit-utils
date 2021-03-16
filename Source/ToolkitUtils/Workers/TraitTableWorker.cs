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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class TraitTableWorker : TableWorker<TableSettingsItem<TraitItem>>
    {
        private const float ExpandedLineSpan = 3f;
        private string addKarmaTypeText;
        private protected Rect AddPriceHeaderRect = Rect.zero;
        private string addPriceHeaderText;
        private protected Rect AddPriceHeaderTextRect = Rect.zero;
        private Rect addStateHeaderInnerRect = Rect.zero;
        private Rect addStateHeaderRect = Rect.zero;
        private StateKey addStateKey = StateKey.Enable;
        private string bypassLimitText;
        private string closeTraitNameTooltip;
        private string defaultKarmaTypeText;
        private string editTraitNameTooltip;
        private Rect expandedHeaderInnerRect = Rect.zero;
        private Rect expandedHeaderRect = Rect.zero;
        private protected Rect NameHeaderRect = Rect.zero;
        private string nameHeaderText;
        private protected Rect NameHeaderTextRect = Rect.zero;
        private string removeKarmaTypeText;
        private protected Rect RemovePriceHeaderRect = Rect.zero;
        private string removePriceHeaderText;
        private protected Rect RemovePriceHeaderTextRect = Rect.zero;
        private Rect removeStateHeaderInnerRect = Rect.zero;
        private Rect removeStateHeaderRect = Rect.zero;
        private StateKey removeStateKey = StateKey.Enable;
        private string resetTraitKarmaTooltip;
        private string resetTraitNameTooltip;
        private Vector2 scrollPos = Vector2.zero;
        private SettingsKey settingsKey = SettingsKey.Collapse;
        private SortKey sortKey = SortKey.Name;
        private SortOrder sortOrder = SortOrder.Descending;

        public override void DrawHeaders(Rect canvas)
        {
            if (SettingsHelper.DrawTableHeader(
                addStateHeaderRect,
                addStateHeaderInnerRect,
                addStateKey == StateKey.Enable ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                addStateKey = addStateKey == StateKey.Enable ? StateKey.Disable : StateKey.Enable;
                NotifyGlobalAddStateChanged(addStateKey);
            }

            if (SettingsHelper.DrawTableHeader(
                removeStateHeaderRect,
                removeStateHeaderInnerRect,
                removeStateKey == StateKey.Enable ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                removeStateKey = removeStateKey == StateKey.Enable ? StateKey.Disable : StateKey.Enable;
                NotifyGlobalRemoveStateChanged(removeStateKey);
            }

            if (SettingsHelper.DrawTableHeader(expandedHeaderRect, expandedHeaderInnerRect, Textures.Gear))
            {
                settingsKey = settingsKey == SettingsKey.Expand ? SettingsKey.Collapse : SettingsKey.Expand;
                NotifyGlobalSettingsChanged(settingsKey);
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
                case SortKey.AddPrice:
                    SettingsHelper.DrawSortIndicator(AddPriceHeaderRect, sortOrder);
                    return;
                case SortKey.RemovePrice:
                    SettingsHelper.DrawSortIndicator(RemovePriceHeaderRect, sortOrder);
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

            if (SettingsHelper.DrawTableHeader(AddPriceHeaderRect, AddPriceHeaderTextRect, addPriceHeaderText))
            {
                sortKey = SortKey.AddPrice;
                anyClicked = true;
            }

            if (SettingsHelper.DrawTableHeader(RemovePriceHeaderRect, RemovePriceHeaderTextRect, removePriceHeaderText))
            {
                sortKey = SortKey.RemovePrice;
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

        public override void DrawTableContents(Rect canvas)
        {
            float expectedLines = Data.Where(i => !i.IsHidden).Sum(i => i.SettingsVisible ? ExpandedLineSpan + 1f : 1f);
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, RowLineHeight * expectedLines);

            var index = 0;
            var expanded = 0;
            var alternate = false;
            GUI.BeginGroup(canvas);
            scrollPos = GUI.BeginScrollView(canvas, scrollPos, viewPort);

            foreach (TableSettingsItem<TraitItem> trait in Data.Where(i => !i.IsHidden))
            {
                var lineRect = new Rect(
                    0f,
                    index * RowLineHeight + RowLineHeight * ExpandedLineSpan * expanded,
                    canvas.width - 16f,
                    RowLineHeight * (trait.SettingsVisible ? ExpandedLineSpan + 1f : 1f)
                );

                if (!lineRect.IsRegionVisible(canvas, scrollPos))
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

        protected virtual void DrawTrait(Rect canvas, TableSettingsItem<TraitItem> trait)
        {
            var nameMouseOverRect = new Rect(NameHeaderRect.x, canvas.y, NameHeaderRect.width, RowLineHeight);
            var nameRect = new Rect(NameHeaderTextRect.x, canvas.y, NameHeaderTextRect.width, RowLineHeight);
            Rect addCheckRect = SettingsHelper.RectForIcon(
                new Rect(addStateHeaderRect.x + 2f, canvas.y + 2f, addStateHeaderRect.width - 4f, RowLineHeight - 4f)
            );
            var addPriceRect = new Rect(
                AddPriceHeaderTextRect.x,
                canvas.y,
                AddPriceHeaderTextRect.width,
                RowLineHeight
            );
            Rect removeCheckRect = SettingsHelper.RectForIcon(
                new Rect(
                    removeStateHeaderRect.x + 2f,
                    canvas.y + 2f,
                    removeStateHeaderRect.width - 4f,
                    RowLineHeight - 4f
                )
            );
            var removePriceRect = new Rect(
                RemovePriceHeaderTextRect.x,
                canvas.y,
                RemovePriceHeaderTextRect.width,
                RowLineHeight
            );
            Rect settingRect = SettingsHelper.RectForIcon(
                new Rect(
                    expandedHeaderRect.x + 2f,
                    canvas.y + Mathf.FloorToInt(Mathf.Abs(expandedHeaderRect.width - RowLineHeight) / 2f) + 2f,
                    expandedHeaderRect.width - 4f,
                    expandedHeaderRect.width - 4f
                )
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
                    builder.AppendLine($"- {i}");
                }

                TooltipHandler.TipRegion(nameMouseOverRect, builder.ToString());
            }

            Widgets.Checkbox(
                addCheckRect.x,
                addCheckRect.y,
                ref trait.Data.CanAdd,
                addCheckRect.height,
                paintable: true
            );

            if (trait.Data.CanAdd)
            {
                SettingsHelper.DrawPriceField(addPriceRect, ref trait.Data.CostToAdd);
            }

            Widgets.Checkbox(
                removeCheckRect.x,
                removeCheckRect.y,
                ref trait.Data.CanRemove,
                removeCheckRect.height,
                paintable: true
            );

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

        private void DrawConfigurableTraitName(Rect canvas, TableSettingsItem<TraitItem> trait)
        {
            if (trait.EditingName)
            {
                var fieldRect = new Rect(canvas.x, canvas.y, canvas.width - canvas.height, canvas.height);

                if (SettingsHelper.DrawTextField(fieldRect, trait.Data.Name, out string result))
                {
                    trait.Data.Name = result.ToToolkit();
                    trait.Data.TraitData.CustomName = true;
                }

                if (trait.Data.TraitData.CustomName
                    && SettingsHelper.DrawFieldButton(fieldRect, Textures.Reset, resetTraitNameTooltip))
                {
                    trait.Data.TraitData.CustomName = false;
                    trait.Data.Name = trait.Data.GetDefaultName();
                }
            }
            else
            {
                SettingsHelper.DrawLabel(canvas, trait.Data.Name);
            }

            GUI.color = new Color(1f, 1f, 1f, 0.7f);
            if (SettingsHelper.DrawFieldButton(
                canvas,
                trait.EditingName ? Widgets.CheckboxOffTex : Textures.Edit,
                trait.EditingName ? closeTraitNameTooltip : editTraitNameTooltip
            ))
            {
                trait.EditingName = !trait.EditingName;
            }

            GUI.color = Color.white;
        }

        public override void Prepare()
        {
            LoadTranslations();

            _data ??= new List<TableSettingsItem<TraitItem>>();
            _data.AddRange(
                ToolkitUtils.Data.Traits.OrderBy(i => i.Name).Select(i => new TableSettingsItem<TraitItem> {Data = i})
            );
        }

        private void DrawExpandedSettings(Rect canvas, TableSettingsItem<TraitItem> trait)
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

        private void DrawLeftExpandedSettingsColumn(Rect canvas, TableSettingsItem<TraitItem> trait)
        {
            (Rect addKarmaLabel, Rect addKarmaField) = new Rect(0f, 0f, canvas.width, RowLineHeight).ToForm();
            SettingsHelper.DrawLabel(addKarmaLabel, addKarmaTypeText);
            if (Widgets.ButtonText(
                addKarmaField,
                trait.Data.Data.KarmaType == null ? defaultKarmaTypeText : trait.Data.Data.KarmaType.ToString()
            ))
            {
                Find.WindowStack.Add(
                    new FloatMenu(
                        ToolkitUtils.Data.KarmaTypes.Select(
                                i => new FloatMenuOption(i.ToString(), () => trait.Data.Data.KarmaType = i)
                            )
                           .ToList()
                    )
                );
            }

            if (trait.Data.Data.KarmaType != null
                && SettingsHelper.DrawFieldButton(addKarmaLabel, Textures.Reset, resetTraitKarmaTooltip))
            {
                trait.Data.Data.KarmaType = null;
            }

            (Rect removeKarmaLabel, Rect removeKarmaField) =
                new Rect(0f, RowLineHeight, canvas.width, RowLineHeight).ToForm();
            SettingsHelper.DrawLabel(removeKarmaLabel, removeKarmaTypeText);
            if (Widgets.ButtonText(
                removeKarmaField,
                trait.Data.TraitData.KarmaTypeForRemoving == null
                    ? defaultKarmaTypeText
                    : trait.Data.TraitData.KarmaTypeForRemoving.ToString()
            ))
            {
                Find.WindowStack.Add(
                    new FloatMenu(
                        ToolkitUtils.Data.KarmaTypes.Select(
                                i => new FloatMenuOption(
                                    i.ToString(),
                                    () => trait.Data.TraitData.KarmaTypeForRemoving = i
                                )
                            )
                           .ToList()
                    )
                );
            }

            if (trait.Data.TraitData.KarmaTypeForRemoving != null
                && SettingsHelper.DrawFieldButton(removeKarmaLabel, Textures.Reset, resetTraitKarmaTooltip))
            {
                trait.Data.TraitData.KarmaTypeForRemoving = null;
            }
        }

        private void DrawRightExpandedSettingsColumn(Rect canvas, TableSettingsItem<TraitItem> trait)
        {
            bool proxy = trait.Data.TraitData.CanBypassLimit;
            if (SettingsHelper.LabeledPaintableCheckbox(
                new Rect(0f, 0f, canvas.width, RowLineHeight),
                bypassLimitText,
                ref proxy
            ))
            {
                trait.Data.TraitData.CanBypassLimit = proxy;
            }
        }

        public override void EnsureExists(TableSettingsItem<TraitItem> data)
        {
            if (!_data.Any(i => i.Data.DefName.Equals(data.Data.DefName)))
            {
                _data.Add(data);
            }
        }

        private void LoadTranslations()
        {
            nameHeaderText = "TKUtils.Headers.Name".Localize();
            addPriceHeaderText = "TKUtils.Headers.AddPrice".Localize();
            removePriceHeaderText = "TKUtils.Headers.RemovePrice".Localize();

            bypassLimitText = "TKUtils.Fields.BypassTraitLimit".Localize();
            defaultKarmaTypeText = "TKUtils.Fields.DefaultKarmaType".Localize();
            addKarmaTypeText = "TKUtils.Fields.AddKarmaType".Localize();
            removeKarmaTypeText = "TKUtils.Fields.RemoveKarmaType".Localize();

            editTraitNameTooltip = "TKUtils.TraitTableTooltips.EditTraitName".Localize();
            closeTraitNameTooltip = "TKUtils.TraitTableTooltips.CloseTraitName".Localize();
            resetTraitNameTooltip = "TKUtils.TraitTableTooltips.ResetTraitName".Localize();
            resetTraitKarmaTooltip = "TKUtils.TraitTableTooltips.ResetTraitKarma".Localize();
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
                case SortKey.AddPrice:
                    _data = _data.OrderBy(i => i.Data.CostToAdd).ThenBy(i => i.Data.Name).ToList();
                    return;
                case SortKey.RemovePrice:
                    _data = _data.OrderBy(i => i.Data.CostToRemove).ThenBy(i => i.Data.Name).ToList();
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
                case SortKey.AddPrice:
                    _data = _data.OrderByDescending(i => i.Data.CostToAdd).ThenByDescending(i => i.Data.Name).ToList();
                    return;
                case SortKey.RemovePrice:
                    _data = _data.OrderByDescending(i => i.Data.CostToRemove)
                       .ThenByDescending(i => i.Data.Name)
                       .ToList();
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
            float consumedWidth = canvas.width - 18f - LineHeight * 3f; // Icon buttons
            float nameWidth = Mathf.FloorToInt(consumedWidth * 0.45f);
            float distributedWidth = Mathf.FloorToInt((consumedWidth - nameWidth) * 0.5f);
            NameHeaderRect = new Rect(0f, 0f, nameWidth, LineHeight);
            NameHeaderTextRect = new Rect(
                NameHeaderRect.x + 4f,
                NameHeaderRect.y,
                NameHeaderRect.width - 8f,
                NameHeaderRect.height
            );
            addStateHeaderRect = new Rect(NameHeaderRect.x + NameHeaderRect.width + 1f, 0f, LineHeight, LineHeight);
            addStateHeaderInnerRect = addStateHeaderRect.ContractedBy(2f);
            AddPriceHeaderRect = new Rect(
                addStateHeaderRect.x + addStateHeaderRect.width + 1f,
                0f,
                distributedWidth,
                LineHeight
            );
            AddPriceHeaderTextRect = new Rect(
                AddPriceHeaderRect.x + 4f,
                AddPriceHeaderRect.y,
                AddPriceHeaderRect.width - 8f,
                AddPriceHeaderRect.height
            );
            removeStateHeaderRect = new Rect(
                AddPriceHeaderRect.x + AddPriceHeaderRect.width + 1f,
                0f,
                LineHeight,
                LineHeight
            );
            removeStateHeaderInnerRect = removeStateHeaderRect.ContractedBy(2f);
            RemovePriceHeaderRect = new Rect(
                removeStateHeaderRect.x + removeStateHeaderRect.width + 1f,
                0f,
                distributedWidth,
                LineHeight
            );
            RemovePriceHeaderTextRect = new Rect(
                RemovePriceHeaderRect.x + 4f,
                RemovePriceHeaderRect.y,
                RemovePriceHeaderRect.width - 8f,
                RemovePriceHeaderRect.height
            );
            expandedHeaderRect = new Rect(
                RemovePriceHeaderRect.x + RemovePriceHeaderRect.width + 1f,
                0f,
                LineHeight,
                LineHeight
            );
            expandedHeaderInnerRect = expandedHeaderRect.ContractedBy(2f);
        }

        public override void NotifyCustomSearchRequested(Func<TableSettingsItem<TraitItem>, bool> worker)
        {
            foreach (TableSettingsItem<TraitItem> item in Data)
            {
                item.IsHidden = !worker(item);
            }
        }

        protected override void FilterDataBySearch(string query)
        {
            foreach (TableSettingsItem<TraitItem> item in Data)
            {
                item.IsHidden = !query.NullOrEmpty() && !item.Data.Name.ToLower().Contains(query.ToLower());
            }
        }

        private enum StateKey { Enable, Disable }

        private enum SettingsKey { Expand, Collapse }

        private enum SortKey { Name, AddPrice, RemovePrice }
    }
}
