// MIT License
//
// Copyright (c) 2021 SirRandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class EventTableWorker : TableWorker<TableSettingsItem<EventItem>>
    {
        private const float BaseExpandedLineSpan = 3f;
        private string capBuffer;
        private string closeEventNameTooltip;
        private string editEventNameTooltip;
        private string eventCapText;
        private Rect expandedHeaderInnerRect = Rect.zero;
        private Rect expandedHeaderRect = Rect.zero;
        private protected Rect KarmaHeaderRect = Rect.zero;
        private string karmaHeaderText;
        private protected Rect KarmaHeaderTextRect = Rect.zero;
        private string karmaTypeText;
        private string maxWagerText;
        private protected Rect NameHeaderRect = Rect.zero;

        private string nameHeaderText;
        private protected Rect NameHeaderTextRect = Rect.zero;
        private string openSettingsText;
        private protected Rect PriceHeaderRect = Rect.zero;
        private string priceHeaderText;
        private protected Rect PriceHeaderTextRect = Rect.zero;
        private string resetEventNameTooltip;
        private Vector2 scrollPos = Vector2.zero;
        private SettingsKey settingsKey = SettingsKey.Collapse;
        private SortKey sortKey = SortKey.Name;
        private SortOrder sortOrder = SortOrder.Descending;
        private Rect stateHeaderInnerRect = Rect.zero;
        private Rect stateHeaderRect = Rect.zero;
        private StateKey stateKey = StateKey.Enable;
        private string wagerBuffer;

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

            if (SettingsHelper.DrawTableHeader(KarmaHeaderRect, KarmaHeaderTextRect, karmaHeaderText))
            {
                sortKey = SortKey.KarmaType;
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
                case SortKey.KarmaType:
                    SettingsHelper.DrawSortIndicator(KarmaHeaderRect, sortOrder);
                    return;
                default:
                    return;
            }
        }

        private void NotifyGlobalSettingsChanged(SettingsKey newState)
        {
            foreach (TableSettingsItem<EventItem> ev in Data.Where(i => !i.IsHidden))
            {
                ev.SettingsVisible = newState == SettingsKey.Expand;
            }
        }

        private void NotifyGlobalStateChanged(StateKey newState)
        {
            foreach (TableSettingsItem<EventItem> ev in Data.Where(i => !i.IsHidden))
            {
                ev.Data.Enabled = newState == StateKey.Enable;
            }
        }

        public override void DrawTableContents(Rect canvas)
        {
            float expectedLine = Data.Where(i => !i.IsHidden).Sum(GetLineSpan);
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, RowLineHeight * expectedLine);

            var alternate = false;
            var expandedLineSpan = 0;
            GUI.BeginGroup(canvas);
            scrollPos = GUI.BeginScrollView(canvas, scrollPos, viewPort);

            foreach (TableSettingsItem<EventItem> ev in Data.Where(i => !i.IsHidden))
            {
                var lineRect = new Rect(
                    0f,
                    RowLineHeight * expandedLineSpan,
                    canvas.width - 16f,
                    RowLineHeight * (ev.SettingsVisible ? GetLineSpan(ev) : 1f)
                );

                if (!lineRect.IsRegionVisible(canvas, scrollPos))
                {
                    alternate = !alternate;
                    expandedLineSpan += GetLineSpan(ev);
                    continue;
                }

                GUI.BeginGroup(lineRect);
                Rect rect = lineRect.AtZero();

                if (alternate)
                {
                    Widgets.DrawLightHighlight(rect);
                }

                DrawEvent(rect, ev);
                GUI.EndGroup();

                alternate = !alternate;
                expandedLineSpan += GetLineSpan(ev);
            }

            GUI.EndScrollView();
            GUI.EndGroup();
        }

        protected virtual void DrawEvent(Rect canvas, [NotNull] TableSettingsItem<EventItem> ev)
        {
            Rect checkboxRect = SettingsHelper.RectForIcon(
                new Rect(stateHeaderRect.x + 2f, canvas.y + 2f, stateHeaderRect.width - 4f, RowLineHeight - 4f)
            );
            var nameMouseOverRect = new Rect(NameHeaderRect.x, canvas.y, NameHeaderRect.width, RowLineHeight);
            var nameRect = new Rect(NameHeaderTextRect.x, canvas.y, NameHeaderTextRect.width, RowLineHeight);
            var priceRect = new Rect(PriceHeaderTextRect.x, canvas.y, PriceHeaderTextRect.width, RowLineHeight);
            var karmaRect = new Rect(KarmaHeaderTextRect.x, canvas.y, KarmaHeaderTextRect.width, RowLineHeight);
            Rect settingRect = SettingsHelper.RectForIcon(
                new Rect(
                    expandedHeaderRect.x + 2f,
                    canvas.y + Mathf.FloorToInt(Mathf.Abs(expandedHeaderRect.width - RowLineHeight) / 2f) + 2f,
                    expandedHeaderRect.width - 4f,
                    expandedHeaderRect.width - 4f
                )
            );

            bool proxy = ev.Data.Enabled;
            if (SettingsHelper.DrawCheckbox(checkboxRect, ref proxy))
            {
                if (!ev.Data.Enabled && proxy)
                {
                    Store_IncidentEditor.LoadBackup(ev.Data.Incident);
                }

                ev.Data.Enabled = proxy;

                if (!Store_IncidentEditor.CopyExists(ev.Data.Incident))
                {
                    Store_IncidentEditor.SaveCopy(ev.Data.Incident);
                }
            }

            DrawConfigurableEventName(nameRect, ev);

            if (!ev.EditingName)
            {
                Widgets.DrawHighlightIfMouseover(nameMouseOverRect);

                if (!ev.Data.Incident.description.NullOrEmpty())
                {
                    TooltipHandler.TipRegion(nameMouseOverRect, ev.Data.Incident.description);
                }
            }

            if (ev.Data.Enabled && ev.Data.CostEditable)
            {
                int cost = ev.Data.Cost;
                SettingsHelper.DrawPriceField(priceRect, ref cost);
                ev.Data.Cost = cost;
            }

            SettingsHelper.DrawLabel(karmaRect, ev.Data.KarmaType.ToString());

            if (Widgets.ButtonImage(settingRect, Textures.Gear))
            {
                ev.SettingsVisible = !ev.SettingsVisible;
            }

            if (!ev.SettingsVisible)
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
            DrawExpandedSettings(expandedRect.AtZero(), ev);
            GUI.EndGroup();
        }

        private void DrawExpandedSettings(Rect canvas, [NotNull] TableItem<EventItem> ev)
        {
            float columnWidth = Mathf.FloorToInt(canvas.width / 2f) - 26f;
            float columnHeight = (BaseExpandedLineSpan - 1) * RowLineHeight;
            var leftColumnRect = new Rect(canvas.x, canvas.y, columnWidth, columnHeight);
            var rightColumnRect = new Rect(canvas.x + leftColumnRect.width + 52f, canvas.y, columnWidth, columnHeight);
            var embedRect = new Rect(canvas.x, canvas.y + columnHeight, canvas.width, canvas.height - columnHeight);

            Widgets.DrawLineVertical(Mathf.FloorToInt(canvas.width / 2f), 0f, columnHeight - 5f);

            GUI.BeginGroup(leftColumnRect);
            DrawLeftExpandedSettingsColumn(leftColumnRect.AtZero(), ev);
            GUI.EndGroup();

            GUI.BeginGroup(rightColumnRect);
            DrawRightExpandedSettingsColumn(rightColumnRect.AtZero(), ev);
            GUI.EndGroup();

            if (!ev.Data.HasSettingsEmbed)
            {
                return;
            }

            GUI.BeginGroup(embedRect);
            ev.Data.SettingsEmbed!.Draw(embedRect.AtZero(), RowLineHeight);
            GUI.EndGroup();
        }

        private void DrawLeftExpandedSettingsColumn(Rect canvas, [NotNull] TableItem<EventItem> ev)
        {
            (Rect capLabel, Rect capField) = new Rect(0f, 0f, canvas.width, RowLineHeight).ToForm(0.6f);
            SettingsHelper.DrawLabel(capLabel, eventCapText);
            int capProxy = ev.Data.EventCap;
            if (SettingsHelper.DrawNumberField(capField, ref capProxy, ref capBuffer, out int newCap, 1f, 200f))
            {
                ev.Data.EventCap = newCap;
            }

            (Rect karmaLabel, Rect karmaField) = new Rect(0f, RowLineHeight, canvas.width, RowLineHeight).ToForm(0.6f);
            SettingsHelper.DrawLabel(karmaLabel, karmaTypeText);
            if (Widgets.ButtonText(karmaField, ev.Data.KarmaType.ToString()))
            {
                Find.WindowStack.Add(
                    new FloatMenu(
                        ToolkitUtils.Data.KarmaTypes
                           .Select(i => new FloatMenuOption(i.ToString(), () => ev.Data.KarmaType = i))
                           .ToList()
                    )
                );
            }
        }

        private void DrawRightExpandedSettingsColumn(Rect canvas, [NotNull] TableItem<EventItem> ev)
        {
            var wagerShown = false;
            if (ev.Data.IsVariables && ev.Data.MaxWager > 0)
            {
                wagerShown = true;
                (Rect wagerLabel, Rect wagerField) = new Rect(0f, 0f, canvas.width, RowLineHeight).ToForm(0.6f);
                SettingsHelper.DrawLabel(wagerLabel, maxWagerText);
                int wagerProxy = ev.Data.MaxWager;
                if (SettingsHelper.DrawNumberField(
                    wagerField,
                    ref wagerProxy,
                    ref wagerBuffer,
                    out int newWager,
                    ev.Data.Variables?.minPointsToFire ?? ev.Data.Cost,
                    20000f
                ))
                {
                    ev.Data.MaxWager = newWager;
                }
            }

            if (ev.Data.HasSettings
                && !ev.Data.HasSettingsEmbed
                && Widgets.ButtonText(
                    new Rect(0f, wagerShown ? RowLineHeight : 0f, canvas.width, RowLineHeight).ToForm(0.6f).Item2,
                    openSettingsText
                ))
            {
                ev.Data.Settings?.EditSettings();
            }
        }

        private void DrawConfigurableEventName(Rect canvas, [NotNull] TableSettingsItem<EventItem> ev)
        {
            if (ev.EditingName)
            {
                var fieldRect = new Rect(canvas.x, canvas.y, canvas.width - canvas.height, canvas.height);

                if (SettingsHelper.DrawTextField(fieldRect, ev.Data.Name, out string result))
                {
                    ev.Data.Name = result!.ToToolkit();
                }

                if (!ev.Data.Name.NullOrEmpty()
                    && SettingsHelper.DrawFieldButton(fieldRect, Textures.Reset, resetEventNameTooltip))
                {
                    ev.Data.Incident.abbreviation = ev.Data.GetDefaultAbbreviation();
                }
            }
            else
            {
                SettingsHelper.DrawLabel(canvas, ev.Data.Name);
            }

            GUI.color = new Color(1f, 1f, 1f, 0.7f);
            if (SettingsHelper.DrawFieldButton(
                canvas,
                ev.EditingName ? Widgets.CheckboxOffTex : Textures.Edit,
                ev.EditingName ? closeEventNameTooltip : editEventNameTooltip
            ))
            {
                ev.EditingName = !ev.EditingName;
            }

            GUI.color = Color.white;
        }

        public override void Prepare()
        {
            LoadTranslations();

            _data ??= new List<TableSettingsItem<EventItem>>();
            _data.AddRange(
                ToolkitUtils.Data.Events.OrderBy(i => i.Name).Select(i => new TableSettingsItem<EventItem> {Data = i})
            );
        }

        private void LoadTranslations()
        {
            nameHeaderText = "TKUtils.Headers.Name".Localize();
            priceHeaderText = "TKUtils.Headers.Price".Localize();
            karmaHeaderText = "TKUtils.Headers.Karma".Localize();

            openSettingsText = "TKUtils.Buttons.Settings".Localize();

            editEventNameTooltip = "TKUtils.EventTableTooltips.EditEventName".Localize();
            closeEventNameTooltip = "TKUtils.EventTableTooltips.CloseEventName".Localize();
            resetEventNameTooltip = "TKUtils.EventTableTooltips.ResetEventName".Localize();

            eventCapText = "TKUtils.Fields.EventCap".Localize();
            maxWagerText = "TKUtils.Fields.MaxWager".Localize();
            karmaTypeText = "TKUtils.Fields.KarmaType".Localize();
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
                case SortKey.KarmaType:
                    _data = _data.OrderBy(i => (int) i.Data.KarmaType).ThenBy(i => i.Data.Name).ToList();
                    return;
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
                case SortKey.KarmaType:
                    _data = _data.OrderByDescending(i => (int) i.Data.KarmaType)
                       .ThenByDescending(i => i.Data.Name)
                       .ToList();
                    return;
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
            float consumedWidth = canvas.width - 10f - LineHeight * 2f; // Icon buttons
            float labelWidth = Mathf.FloorToInt(consumedWidth * 0.4f);
            float remainingWidth = consumedWidth - labelWidth;
            float distributedWidth = Mathf.FloorToInt(remainingWidth / 2f);
            stateHeaderRect = new Rect(0f, 0f, LineHeight, LineHeight);
            stateHeaderInnerRect = stateHeaderRect.ContractedBy(2f);
            NameHeaderRect = new Rect(LineHeight + 1f, 0f, labelWidth, LineHeight);
            NameHeaderTextRect = new Rect(
                NameHeaderRect.x + 4f,
                NameHeaderRect.y,
                NameHeaderRect.width - 8f,
                NameHeaderRect.height
            );
            PriceHeaderRect = new Rect(NameHeaderRect.x + NameHeaderRect.width + 1f, 0f, distributedWidth, LineHeight);
            PriceHeaderTextRect = new Rect(
                PriceHeaderRect.x + 4f,
                PriceHeaderRect.y,
                PriceHeaderRect.width - 8f,
                PriceHeaderRect.height
            );
            KarmaHeaderRect = new Rect(
                PriceHeaderRect.x + PriceHeaderRect.width + 1f,
                0f,
                distributedWidth,
                LineHeight
            );
            KarmaHeaderTextRect = new Rect(
                KarmaHeaderRect.x + 4f,
                KarmaHeaderRect.y,
                KarmaHeaderRect.width - 8f,
                KarmaHeaderRect.height
            );
            expandedHeaderRect = new Rect(KarmaHeaderRect.x + KarmaHeaderRect.width + 1f, 0f, LineHeight, LineHeight);
            expandedHeaderInnerRect = expandedHeaderRect.ContractedBy(2f);
        }

        protected override void FilterDataBySearch(string query)
        {
            foreach (TableSettingsItem<EventItem> ev in Data)
            {
                ev.IsHidden = !query.NullOrEmpty() && !ev.Data.Name.ToLower().Contains(query.ToLower());
            }
        }

        public override void EnsureExists(TableSettingsItem<EventItem> data)
        {
            if (!_data.Any(i => i.Data.DefName.Equals(data.Data.DefName)))
            {
                _data.Add(data);
            }
        }

        public override void NotifyCustomSearchRequested(Func<TableSettingsItem<EventItem>, bool> worker)
        {
            foreach (TableSettingsItem<EventItem> ev in Data)
            {
                ev.IsHidden = !worker(ev);
            }
        }

        private static int GetLineSpan([NotNull] TableSettingsItem<EventItem> ev)
        {
            if (!ev.SettingsVisible)
            {
                return 1;
            }

            if (!ev.Data.HasSettingsEmbed && ev.Data.HasSettings)
            {
                return Mathf.RoundToInt(BaseExpandedLineSpan + 1);
            }

            return ev.Data.HasSettingsEmbed
                ? Mathf.RoundToInt(ev.Data.SettingsEmbed!.LineSpan + BaseExpandedLineSpan + 1)
                : Mathf.RoundToInt(BaseExpandedLineSpan + 1);
        }

        private enum SortKey { Name, Price, KarmaType }
    }
}
