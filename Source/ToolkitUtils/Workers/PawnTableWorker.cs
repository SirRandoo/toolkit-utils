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
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class PawnTableWorker : TableWorker<TableSettingsItem<PawnKindItem>>
    {
        private const float ExpandedLineSpan = 2f;

        private string closePawnNameTooltip;
        private string defaultKarmaTypeText;
        private string editPawnNameTooltip;
        private Rect expandedHeaderInnerRect = Rect.zero;
        private Rect expandedHeaderRect = Rect.zero;
        private string karmaTypeText;
        private protected Rect NameHeaderRect = Rect.zero;
        private string nameHeaderText;
        private protected Rect NameHeaderTextRect = Rect.zero;
        private protected Rect PriceHeaderRect = Rect.zero;
        private string priceHeaderText;
        private protected Rect PriceHeaderTextRect = Rect.zero;
        private string resetPawnKarmaTooltip;
        private string resetPawnNameTooltip;
        private Vector2 scrollPos = Vector2.zero;
        private SettingsKey settingsKey = SettingsKey.Collapse;

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
                stateKey = stateKey == StateKey.Enable ? StateKey.Disable : StateKey.Enable;
                NotifyGlobalStateChanged(stateKey);
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
                case SortKey.Price:
                    SettingsHelper.DrawSortIndicator(PriceHeaderRect, sortOrder);
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

        public override void DrawTableContents(Rect canvas)
        {
            float expectedLines = Data.Where(i => !i.IsHidden).Sum(i => i.SettingsVisible ? ExpandedLineSpan + 1f : 1f);
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, RowLineHeight * expectedLines);

            var index = 0;
            var expanded = 0;
            var alternate = false;
            GUI.BeginGroup(canvas);
            scrollPos = GUI.BeginScrollView(canvas, scrollPos, viewPort);

            foreach (TableSettingsItem<PawnKindItem> item in Data.Where(i => !i.IsHidden))
            {
                var lineRect = new Rect(
                    0f,
                    index * RowLineHeight + RowLineHeight * ExpandedLineSpan * expanded,
                    canvas.width - 16f,
                    RowLineHeight * (item.SettingsVisible ? ExpandedLineSpan + 1f : 1f)
                );

                if (!lineRect.IsRegionVisible(canvas, scrollPos))
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

        protected virtual void DrawKind(Rect canvas, TableSettingsItem<PawnKindItem> item)
        {
            Rect checkboxRect = SettingsHelper.RectForIcon(
                new Rect(stateHeaderRect.x + 2f, canvas.y + 2f, stateHeaderRect.width - 4f, RowLineHeight - 4f)
            );
            var nameRect = new Rect(NameHeaderTextRect.x, canvas.y, NameHeaderTextRect.width, RowLineHeight);
            var priceRect = new Rect(PriceHeaderTextRect.x, canvas.y, PriceHeaderTextRect.width, RowLineHeight);
            Rect settingRect = SettingsHelper.RectForIcon(
                new Rect(
                    expandedHeaderRect.x + 2f,
                    canvas.y + Mathf.FloorToInt(Mathf.Abs(expandedHeaderRect.width - RowLineHeight) / 2f) + 2f,
                    expandedHeaderRect.width - 4f,
                    expandedHeaderRect.width - 4f
                )
            );

            bool proxy = item.Data.Enabled;
            if (SettingsHelper.DrawCheckbox(checkboxRect, ref proxy))
            {
                item.Data.Enabled = proxy;
            }

            DrawConfigurableItemName(nameRect, item);

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
                canvas.y + RowLineHeight + 10f,
                canvas.width - checkboxRect.width - settingRect.width - 20f,
                canvas.height - RowLineHeight - 20f
            );

            GUI.BeginGroup(expandedRect);
            DrawExpandedSettings(expandedRect.AtZero(), item);
            GUI.EndGroup();
        }

        private void DrawConfigurableItemName(Rect canvas, TableSettingsItem<PawnKindItem> item)
        {
            if (item.EditingName)
            {
                var fieldRect = new Rect(canvas.x, canvas.y, canvas.width - canvas.height, canvas.height);

                if (SettingsHelper.DrawTextField(fieldRect, item.Data.Name, out string result))
                {
                    item.Data.Name = result.ToToolkit();
                    item.Data.PawnData.CustomName = true;
                }

                if (item.Data.PawnData.CustomName
                    && SettingsHelper.DrawFieldButton(fieldRect, Textures.Reset, resetPawnNameTooltip))
                {
                    item.Data.PawnData.CustomName = false;
                }
            }
            else
            {
                SettingsHelper.DrawLabel(canvas, item.Data.Name);
            }

            GUI.color = new Color(1f, 1f, 1f, 0.7f);
            if (SettingsHelper.DrawFieldButton(
                canvas,
                item.EditingName ? Widgets.CheckboxOffTex : Textures.Edit,
                item.EditingName ? closePawnNameTooltip : editPawnNameTooltip
            ))
            {
                item.EditingName = !item.EditingName;
            }

            GUI.color = Color.white;
        }

        public override void Prepare()
        {
            LoadTranslations();

            _data ??= new List<TableSettingsItem<PawnKindItem>>();
            _data.AddRange(
                ToolkitUtils.Data.PawnKinds.OrderBy(i => i.Name)
                   .Select(i => new TableSettingsItem<PawnKindItem> {Data = i})
            );
        }

        private void DrawExpandedSettings(Rect canvas, TableSettingsItem<PawnKindItem> item)
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

        private void DrawLeftExpandedSettingsColumn(Rect canvas, TableSettingsItem<PawnKindItem> item)
        {
            (Rect karmaLabel, Rect karmaField) = new Rect(0f, 0f, canvas.width, RowLineHeight).ToForm(0.6f);
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

            if (item.Data.Data.KarmaType != null
                && SettingsHelper.DrawFieldButton(karmaLabel, Textures.Reset, resetPawnKarmaTooltip))
            {
                item.Data.Data.KarmaType = null;
            }
        }

        private void DrawRightExpandedSettingsColumn(Rect canvas, TableSettingsItem<PawnKindItem> item)
        {
            // unused
        }

        public override void EnsureExists(TableSettingsItem<PawnKindItem> data)
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

            karmaTypeText = "TKUtils.Fields.KarmaType".Localize();
            defaultKarmaTypeText = "TKUtils.Fields.DefaultKarmaType".Localize();

            editPawnNameTooltip = "TKUtils.PawnTableTooltips.EditPawnName".Localize();
            closePawnNameTooltip = "TKUtils.PawnTableTooltips.ClosePawnName".Localize();
            resetPawnNameTooltip = "TKUtils.PawnTableTooltips.ResetPawnName".Localize();
            resetPawnKarmaTooltip = "TKUtils.PawnTableTooltips.ResetPawnKarma".Localize();
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
                    _data = _data.OrderBy(i => i.Data.Cost).ThenBy(i => i.Data.Name).ToList();
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
                    _data = _data.OrderByDescending(i => i.Data.Cost).ThenByDescending(i => i.Data.Name).ToList();
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
            stateHeaderRect = new Rect(0f, 0f, LineHeight, LineHeight);
            stateHeaderInnerRect = stateHeaderRect.ContractedBy(2f);
            NameHeaderRect = new Rect(LineHeight + 1f, 0f, Mathf.FloorToInt(consumedWidth * 0.55f), LineHeight);
            NameHeaderTextRect = new Rect(
                NameHeaderRect.x + 4f,
                NameHeaderRect.y,
                NameHeaderRect.width - 8f,
                NameHeaderRect.height
            );
            PriceHeaderRect = new Rect(
                NameHeaderRect.x + NameHeaderRect.width + 1f,
                0f,
                Mathf.FloorToInt(consumedWidth * 0.45f),
                LineHeight
            );
            PriceHeaderTextRect = new Rect(
                PriceHeaderRect.x + 4f,
                PriceHeaderRect.y,
                PriceHeaderRect.width - 8f,
                PriceHeaderRect.height
            );
            expandedHeaderRect = new Rect(PriceHeaderRect.x + PriceHeaderRect.width + 1f, 0f, LineHeight, LineHeight);
            expandedHeaderInnerRect = expandedHeaderRect.ContractedBy(2f);
        }

        public override void NotifyCustomSearchRequested(Func<TableSettingsItem<PawnKindItem>, bool> worker)
        {
            foreach (TableSettingsItem<PawnKindItem> item in Data)
            {
                item.IsHidden = !worker(item);
            }
        }

        protected override void FilterDataBySearch(string query)
        {
            foreach (TableSettingsItem<PawnKindItem> item in Data)
            {
                item.IsHidden = !query.NullOrEmpty() && !item.Data.Name.ToLower().Contains(query.ToLower());
            }
        }

        private enum SortKey { Name, Price }

        private enum StateKey { Enable, Disable }

        private enum SettingsKey { Expand, Collapse }
    }
}
