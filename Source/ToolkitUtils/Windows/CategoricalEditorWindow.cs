// MIT License
// 
// Copyright (c) 2022 SirRandoo
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
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public abstract class CategoricalEditorWindow<TDef> : Window where TDef : Def
    {
        private readonly List<DefEntry> _defEntries = new List<DefEntry>();
        private readonly string _header;
        protected readonly QuickSearchWidget SearchWidget = new QuickSearchWidget();
        private string _disableAllText;

        private string _editText;
        private string _resetAllText;
        private Vector2 _scrollPos = Vector2.zero;
        private int _visibleEntries;

        protected CategoricalEditorWindow(string header)
        {
            _header = header;
            doCloseButton = true;
        }

        /// <inheritdoc/>
        public override Vector2 InitialSize => new Vector2(425, 500);

        /// <inheritdoc/>
        public override void PreOpen()
        {
            foreach (TDef def in DefDatabase<TDef>.AllDefs)
            {
                _defEntries.Add(new DefEntry { Def = def, Visible = true });
                _visibleEntries++;
            }

            _defEntries.SortBy(s => s.Def.label);

            GetTranslations();
            base.PreOpen();
        }

        private void GetTranslations()
        {
            _editText = "TKUtils.Buttons.Edit".TranslateSimple();
            _resetAllText = "TKUtils.Buttons.ResetAll".TranslateSimple();
            _disableAllText = "TKUtils.Buttons.DisableAll".TranslateSimple();
        }

        /// <inheritdoc/>
        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            var headerRegion = new Rect(0f, 0f, inRect.width, Text.LineHeightOf(GameFont.Medium) + Text.SmallFontHeight + StandardMargin);

            var contentsRegion = new Rect(
                0f,
                headerRegion.height + StandardMargin,
                inRect.width,
                inRect.height - headerRegion.height - CloseButSize.y - StandardMargin * 2f
            );

            GUI.BeginGroup(headerRegion);
            DrawHeader(headerRegion.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(contentsRegion);
            Widgets.DrawMenuSection(contentsRegion.AtZero());

            Rect innerContentsRegion = contentsRegion.AtZero().ContractedBy(17f);

            GUI.BeginGroup(innerContentsRegion);
            DrawContents(innerContentsRegion.AtZero());
            GUI.EndGroup();

            GUI.EndGroup();

            GUI.EndGroup();
        }

        protected virtual void DrawHeader(Rect region)
        {
            var headerTextRegion = new Rect(0f, 0f, region.width, Text.LineHeightOf(GameFont.Medium));
            var globalActionsRegion = new Rect(0f, headerTextRegion.height + StandardMargin, region.width, region.height - headerTextRegion.height - StandardMargin);

            UiHelper.Label(headerTextRegion, _header, TextAnchor.MiddleCenter, GameFont.Medium);

            GUI.BeginGroup(globalActionsRegion);
            DrawGlobalActions(globalActionsRegion.AtZero());
            GUI.EndGroup();
        }

        protected virtual void DrawGlobalActions(Rect region)
        {
            var searchRegion = new Rect(0f, 0f, Mathf.FloorToInt(region.width * 0.6f), region.height);

            float buttonWidths = Mathf.FloorToInt((region.width - searchRegion.width) * 0.5f);
            var disableAllRegion = new Rect(searchRegion.width, 0f, buttonWidths, region.height);
            var resetAllRegion = new Rect(searchRegion.width + buttonWidths, 0f, buttonWidths, region.height);

            if (Widgets.ButtonText(disableAllRegion, _disableAllText))
            {
                DisableAllEntries();
            }

            if (Widgets.ButtonText(resetAllRegion, _resetAllText))
            {
                foreach (DefEntry entry in _defEntries)
                {
                    if (!entry.Visible)
                    {
                        continue;
                    }
                    
                    ResetEntry(entry.Def);
                }
            }

            SearchWidget.OnGUI(searchRegion, OnSearchQueryChanged);
        }

        protected void DisableAllEntries()
        {
            foreach (DefEntry entry in _defEntries)
            {
                if (!entry.Visible)
                {
                    continue;
                }
                
                DisableEntry(entry.Def);
            }
        }


        protected virtual void DrawContents(Rect region)
        {
            var viewRect = new Rect(0f, 0f, region.width - 16f, Text.LineHeight * _visibleEntries);

            GUI.BeginGroup(region);
            _scrollPos = GUI.BeginScrollView(region, _scrollPos, viewRect);

            var line = 0;

            foreach (DefEntry entry in _defEntries)
            {
                if (!entry.Visible)
                {
                    continue;
                }

                var lineRegion = new Rect(0f, Text.LineHeight * line, viewRect.width, Text.LineHeight);

                if (!lineRegion.IsVisible(region, _scrollPos))
                {
                    line++;

                    continue;
                }

                line++;
                (Rect textRegion, Rect editButtonRegion) = lineRegion.Split(0.8f);

                if (IsEntryDisabled(entry.Def))
                {
                    UiHelper.Label(textRegion, entry.Def.label.Tagged("i").CapitalizeFirst(), Color.grey, TextAnchor.MiddleLeft, GameFont.Small);
                }
                else
                {
                    UiHelper.Label(textRegion, entry.Def.label.CapitalizeFirst(), TextAnchor.MiddleLeft, GameFont.Small);
                }

                if (Widgets.ButtonText(editButtonRegion, _editText))
                {
                    OpenEditorFor(entry.Def);
                }

                if (!string.IsNullOrEmpty(entry.Def.description))
                {
                    TooltipHandler.TipRegion(textRegion, entry.Def.description);
                }

                Widgets.DrawHighlightIfMouseover(lineRegion);
            }

            GUI.EndScrollView();
            GUI.EndGroup();
        }

        private void OnSearchQueryChanged()
        {
            _visibleEntries = 0;

            foreach (DefEntry entry in _defEntries)
            {
                entry.Visible = VisibleInSearch(entry.Def);

                if (entry.Visible)
                {
                    _visibleEntries++;
                }
            }
        }

        protected abstract bool VisibleInSearch(TDef entry);

        protected abstract bool IsEntryDisabled(TDef entry);

        protected abstract void OpenEditorFor(TDef entry);

        protected abstract void DisableEntry(TDef entry);

        protected abstract void ResetEntry(TDef entry);

        private sealed class DefEntry
        {
            public TDef Def { get; set; }
            public bool Visible { get; set; }
        }
    }
}
