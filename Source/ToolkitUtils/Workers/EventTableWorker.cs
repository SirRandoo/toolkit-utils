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
        private string _closeEventNameTooltip;
        private string _editEventNameTooltip;
        private string _eventCapText;
        private Rect _expandedHeaderInnerRect = Rect.zero;
        private Rect _expandedHeaderRect = Rect.zero;
        private string _karmaHeaderText;
        private string _karmaTypeText;
        private string _maxWagerText;

        private string _nameHeaderText;
        private string _openSettingsText;
        private string _priceHeaderText;
        private string _resetEventNameTooltip;
        private Vector2 _scrollPos = Vector2.zero;
        private SettingsKey _settingsKey = SettingsKey.Collapse;
        private SortKey _sortKey = SortKey.Name;
        private SortOrder _sortOrder = SortOrder.Descending;
        private Rect _stateHeaderInnerRect = Rect.zero;
        private Rect _stateHeaderRect = Rect.zero;
        private StateKey _stateKey = StateKey.Enable;
        private string _wagerBuffer;
        private protected Rect KarmaHeaderRect = Rect.zero;
        private protected Rect KarmaHeaderTextRect = Rect.zero;
        private protected Rect NameHeaderRect = Rect.zero;
        private protected Rect NameHeaderTextRect = Rect.zero;
        private protected Rect PriceHeaderRect = Rect.zero;
        private protected Rect PriceHeaderTextRect = Rect.zero;

        public override void DrawHeaders(Rect canvas)
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

            if (SettingsHelper.DrawTableHeader(KarmaHeaderRect, KarmaHeaderTextRect, _karmaHeaderText))
            {
                _sortKey = SortKey.KarmaType;
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

        private protected void DrawSortableHeaderIcon()
        {
            switch (_sortKey)
            {
                case SortKey.Name:
                    SettingsHelper.DrawSortIndicator(NameHeaderRect, _sortOrder);

                    return;
                case SortKey.Price:
                    SettingsHelper.DrawSortIndicator(PriceHeaderRect, _sortOrder);

                    return;
                case SortKey.KarmaType:
                    SettingsHelper.DrawSortIndicator(KarmaHeaderRect, _sortOrder);

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
            _scrollPos = GUI.BeginScrollView(canvas, _scrollPos, viewPort);

            foreach (TableSettingsItem<EventItem> ev in Data.Where(i => !i.IsHidden))
            {
                var lineRect = new Rect(0f, RowLineHeight * expandedLineSpan, canvas.width - 16f, RowLineHeight * (ev.SettingsVisible ? GetLineSpan(ev) : 1f));

                if (!lineRect.IsRegionVisible(canvas, _scrollPos))
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
            Rect checkboxRect = SettingsHelper.RectForIcon(new Rect(_stateHeaderRect.x + 2f, canvas.y + 2f, _stateHeaderRect.width - 4f, RowLineHeight - 4f));
            var nameMouseOverRect = new Rect(NameHeaderRect.x, canvas.y, NameHeaderRect.width, RowLineHeight);
            var nameRect = new Rect(NameHeaderTextRect.x, canvas.y, NameHeaderTextRect.width, RowLineHeight);
            var priceRect = new Rect(PriceHeaderTextRect.x, canvas.y, PriceHeaderTextRect.width, RowLineHeight);
            var karmaRect = new Rect(KarmaHeaderTextRect.x, canvas.y, KarmaHeaderTextRect.width, RowLineHeight);

            Rect settingRect = SettingsHelper.RectForIcon(
                new Rect(
                    _expandedHeaderRect.x + 2f,
                    canvas.y + Mathf.FloorToInt(Mathf.Abs(_expandedHeaderRect.width - RowLineHeight) / 2f) + 2f,
                    _expandedHeaderRect.width - 4f,
                    _expandedHeaderRect.width - 4f
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

            var expandedRect = new Rect(NameHeaderRect.x + 10f, canvas.y + RowLineHeight + 10f, canvas.width - checkboxRect.width - settingRect.width - 20f, canvas.height - RowLineHeight - 20f);

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
            SettingsHelper.DrawLabel(capLabel, _eventCapText);
            int capProxy = ev.Data.EventCap;
            var capBuffer = capProxy.ToString();

            if (SettingsHelper.DrawNumberField(capField, ref capProxy, ref capBuffer, out int newCap, 1f, 200f))
            {
                ev.Data.EventCap = newCap;
            }

            (Rect karmaLabel, Rect karmaField) = new Rect(0f, RowLineHeight, canvas.width, RowLineHeight).ToForm(0.6f);
            SettingsHelper.DrawLabel(karmaLabel, _karmaTypeText);

            if (Widgets.ButtonText(karmaField, ev.Data.KarmaType.ToString()))
            {
                Find.WindowStack.Add(new FloatMenu(ToolkitUtils.Data.KarmaTypes.Select(i => new FloatMenuOption(i.ToString(), () => ev.Data.KarmaType = i)).ToList()));
            }
        }

        private void DrawRightExpandedSettingsColumn(Rect canvas, [NotNull] TableItem<EventItem> ev)
        {
            var wagerShown = false;

            if (ev.Data.IsVariables && ev.Data.MaxWager > 0)
            {
                wagerShown = true;
                (Rect wagerLabel, Rect wagerField) = new Rect(0f, 0f, canvas.width, RowLineHeight).ToForm(0.6f);
                SettingsHelper.DrawLabel(wagerLabel, _maxWagerText);
                int wagerProxy = ev.Data.MaxWager;

                if (SettingsHelper.DrawNumberField(wagerField, ref wagerProxy, ref _wagerBuffer, out int newWager, ev.Data.Variables?.minPointsToFire ?? ev.Data.Cost, 20000f))
                {
                    ev.Data.MaxWager = newWager;
                }
            }

            if (ev.Data.HasSettings
                && !ev.Data.HasSettingsEmbed
                && Widgets.ButtonText(new Rect(0f, wagerShown ? RowLineHeight : 0f, canvas.width, RowLineHeight).ToForm(0.6f).Item2, _openSettingsText))
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

                if (!ev.Data.Name.NullOrEmpty() && SettingsHelper.DrawFieldButton(fieldRect, Textures.Reset, _resetEventNameTooltip))
                {
                    ev.Data.Incident.abbreviation = ev.Data.GetDefaultAbbreviation();
                }
            }
            else
            {
                SettingsHelper.DrawLabel(canvas, ev.Data.Name);
            }

            GUI.color = new Color(1f, 1f, 1f, 0.7f);

            if (SettingsHelper.DrawFieldButton(canvas, ev.EditingName ? Widgets.CheckboxOffTex : Textures.Edit, ev.EditingName ? _closeEventNameTooltip : _editEventNameTooltip))
            {
                ev.EditingName = !ev.EditingName;
            }

            GUI.color = Color.white;
        }

        public override void Prepare()
        {
            LoadTranslations();

            InternalData ??= new List<TableSettingsItem<EventItem>>();
            InternalData.AddRange(ToolkitUtils.Data.Events.OrderBy(i => i.Name).Select(i => new TableSettingsItem<EventItem> { Data = i }));
        }

        private void LoadTranslations()
        {
            _nameHeaderText = "TKUtils.Headers.Name".Localize();
            _priceHeaderText = "TKUtils.Headers.Price".Localize();
            _karmaHeaderText = "TKUtils.Headers.Karma".Localize();

            _openSettingsText = "TKUtils.Buttons.Settings".Localize();

            _editEventNameTooltip = "TKUtils.EventTableTooltips.EditEventName".Localize();
            _closeEventNameTooltip = "TKUtils.EventTableTooltips.CloseEventName".Localize();
            _resetEventNameTooltip = "TKUtils.EventTableTooltips.ResetEventName".Localize();

            _eventCapText = "TKUtils.Fields.EventCap".Localize();
            _maxWagerText = "TKUtils.Fields.MaxWager".Localize();
            _karmaTypeText = "TKUtils.Fields.KarmaType".Localize();
        }

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
                case SortKey.KarmaType:
                    InternalData = InternalData.OrderBy(i => (int)i.Data.KarmaType).ThenBy(i => i.Data.Name).ToList();

                    return;
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
                case SortKey.KarmaType:
                    InternalData = InternalData.OrderByDescending(i => (int)i.Data.KarmaType).ThenByDescending(i => i.Data.Name).ToList();

                    return;
                case SortKey.Price:
                    InternalData = InternalData.OrderByDescending(i => i.Data.Cost).ThenByDescending(i => i.Data.Name).ToList();

                    return;
                default:
                    InternalData = InternalData.OrderByDescending(i => i.Data.Name).ToList();

                    return;
            }
        }

        public override void NotifySearchRequested(string query)
        {
            FilterDataBySearch(query);
        }

        public override void NotifyResolutionChanged(Rect canvas)
        {
            float consumedWidth = canvas.width - 20f - LineHeight * 2f; // Icon buttons
            float labelWidth = Mathf.FloorToInt(consumedWidth * 0.4f);
            float remainingWidth = consumedWidth - labelWidth;
            float distributedWidth = Mathf.FloorToInt(remainingWidth / 2f);
            _stateHeaderRect = new Rect(0f, 0f, LineHeight, LineHeight);
            _stateHeaderInnerRect = _stateHeaderRect.ContractedBy(2f);
            NameHeaderRect = new Rect(LineHeight + 1f, 0f, labelWidth, LineHeight);
            NameHeaderTextRect = new Rect(NameHeaderRect.x + 4f, NameHeaderRect.y, NameHeaderRect.width - 8f, NameHeaderRect.height);
            PriceHeaderRect = new Rect(NameHeaderRect.x + NameHeaderRect.width + 1f, 0f, distributedWidth, LineHeight);
            PriceHeaderTextRect = new Rect(PriceHeaderRect.x + 4f, PriceHeaderRect.y, PriceHeaderRect.width - 8f, PriceHeaderRect.height);
            KarmaHeaderRect = new Rect(PriceHeaderRect.x + PriceHeaderRect.width + 1f, 0f, distributedWidth, LineHeight);
            KarmaHeaderTextRect = new Rect(KarmaHeaderRect.x + 4f, KarmaHeaderRect.y, KarmaHeaderRect.width - 8f, KarmaHeaderRect.height);
            _expandedHeaderRect = new Rect(KarmaHeaderRect.x + KarmaHeaderRect.width + 1f, 0f, LineHeight, LineHeight);
            _expandedHeaderInnerRect = _expandedHeaderRect.ContractedBy(2f);
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
            if (!InternalData.Any(i => i.Data.DefName.Equals(data.Data.DefName)))
            {
                InternalData.Add(data);
            }
        }

        public override void NotifyGlobalDataChanged()
        {
            var wasDirty = false;

            foreach (EventItem item in ToolkitUtils.Data.Events.Select(item => new { item, existing = InternalData.Find(i => i.Data.Equals(item)) })
               .Where(t => t.existing == null)
               .Select(t => t.item))
            {
                InternalData.Add(new TableSettingsItem<EventItem> { Data = item });
                wasDirty = true;
            }

            if (wasDirty)
            {
                NotifySortRequested();
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

            return ev.Data.HasSettingsEmbed ? Mathf.RoundToInt(ev.Data.SettingsEmbed!.LineSpan + BaseExpandedLineSpan + 1) : Mathf.RoundToInt(BaseExpandedLineSpan + 1);
        }

        private enum SortKey { Name, Price, KarmaType }
    }
}
