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
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class ViewersDialog : Window
    {
        private readonly ViewerEditorWorker _editor = new ViewerEditorWorker();
        private readonly QuickSearchWidget _searchWidget = new QuickSearchWidget();
        private readonly List<ViewerEntry> _viewers = new List<ViewerEntry>();
        private readonly Dictionary<string, ViewerEntry> _viewersKeyed = new Dictionary<string, ViewerEntry>();
        private string _coinBuffer;
        private bool _coinBufferValid;
        private string _coinsText;
        private bool _editingCoins;
        private bool _editingKarma;
        private string _karmaBuffer;
        private bool _karmaBufferValid;
        private string _karmaText;
        private Vector2 _listScrollPos = Vector2.zero;

        private string _purgeText;
        private string _resetAllText;

        private int _viewerCount;

        public ViewersDialog()
        {
            doCloseX = true;
            closeOnAccept = false;
            closeOnCancel = false;
        }

        /// <inheritdoc/>
        public override void PreOpen()
        {
            GetTranslations();

            base.PreOpen();
        }

        private void GetTranslations()
        {
            _purgeText = "TKUtils.Buttons.Purge".TranslateSimple();
            _resetAllText = "TKUtils.Buttons.ResetAll".TranslateSimple();

            _karmaText = "TKUtils.PurgeMenu.Karma".TranslateSimple().CapitalizeFirst();
            _coinsText = "TKUtils.PurgeMenu.Coins".TranslateSimple().CapitalizeFirst();
        }

        /// <inheritdoc/>
        public override void WindowUpdate()
        {
            if (Time.unscaledTime % 5 == 0 || _viewers.Count == Viewers.All.Count)
            {
                return;
            }

            _viewers.Clear();
            _viewerCount = 0;

            foreach (Viewer viewer in Viewers.All)
            {
                if (_viewersKeyed.TryGetValue(viewer.username, out ViewerEntry entry))
                {
                    continue;
                }

                entry = new ViewerEntry { Viewer = viewer, Visible = ShouldBeVisible(viewer) };

                _viewerCount++;
                _viewers.Add(entry);
                _viewersKeyed[viewer.username] = entry;

                if (_editor.IsMulti)
                {
                    _editor.AddViewers(entry);
                }
            }
        }

        /// <inheritdoc/>
        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            var listRegion = new Rect(0f, 0f, Mathf.FloorToInt(inRect.width * 0.4f) - StandardMargin, inRect.height - Text.LineHeight * 2f - StandardMargin);
            Rect innerListRegion = listRegion.ContractedBy(4f);
            var editorRegion = new Rect(listRegion.width + 10f, 0f, inRect.width - listRegion.width - 10f, inRect.height);
            Rect innerEditorRegion = editorRegion.ContractedBy(8f);
            var purgeRegion = new Rect(0f, inRect.height - Text.LineHeight, listRegion.width, Text.LineHeight);

            Widgets.DrawMenuSection(listRegion);

            GUI.BeginGroup(innerListRegion);
            DrawViewerList(innerListRegion.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(innerEditorRegion);

            if (_editor.HasViewers)
            {
                DrawEditor(innerEditorRegion.AtZero());
            }

            GUI.EndGroup();

            if (Widgets.ButtonText(purgeRegion, _purgeText))
            {
                Find.WindowStack.Add(new PurgeViewersDialog());
            }

            if (Widgets.ButtonText(purgeRegion.Shift(Direction8Way.North, 0f), _resetAllText))
            {
                Find.WindowStack.Add(new ConfirmationDialog("TKUtils.ViewerDialog.ResetConfirmation".TranslateSimple(), ResetViewers));
            }

            GUI.EndGroup();
        }

        private void DrawEditor(Rect region)
        {
            float lineHeight = Text.LineHeight;
            var headerRegion = new Rect(0f, 0f, region.width, Text.LineHeightOf(GameFont.Medium));
            var coinsRegion = new Rect(0f, lineHeight + 8f, region.width, lineHeight);
            var karmaRegion = new Rect(0f, coinsRegion.y + coinsRegion.height, region.width, lineHeight);

            GUI.color = Color.grey;
            Widgets.DrawLineHorizontal(0f, lineHeight + 4f, region.width);
            GUI.color = Color.white;

            GUI.BeginGroup(headerRegion);
            DrawHeader(headerRegion);
            GUI.EndGroup();

            GUI.BeginGroup(coinsRegion);
            DrawCoinsEditor(coinsRegion.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(karmaRegion);
            DrawKarmaEditor(karmaRegion.AtZero());
            GUI.EndGroup();
        }

        private void DrawHeader(Rect region)
        {
            UiHelper.Label(region, _editor.EditingUser, _editor.IsMulti ? ColorLibrary.RedReadable : Color.white, TextAnchor.MiddleLeft, GameFont.Small);

            if (!_editor.IsSingle)
            {
                return;
            }

            ViewerEntry entry = _editor.User;

            if (entry == null)
            {
                return;
            }

            Rect actionBtnRegion = LayoutHelper.IconRect(region.width - region.height, 0f, region.height, region.height, 7f);

            if (Widgets.ButtonImage(actionBtnRegion, entry.IsBanned ? Textures.SlashedCircle : Textures.Hammer))
            {
                entry.IsBanned = !entry.IsBanned;
            }

            TooltipHandler.TipRegion(
                actionBtnRegion,
                (entry.IsBanned ? "TKUtils.ViewerTooltips.UnbanViewer" : "TKUtils.ViewerTooltips.BanViewer").Translate(entry.Viewer.username)
            );

            actionBtnRegion = actionBtnRegion.Shift(Direction8Way.West, 2f);

            if (Widgets.ButtonImage(actionBtnRegion, entry.IsModerator ? Textures.Sword : Textures.ArrowGhost))
            {
                entry.IsModerator = !entry.IsModerator;
            }

            TooltipHandler.TipRegion(
                actionBtnRegion,
                (entry.IsModerator ? "TKUtils.ViewerTooltips.UnmodViewer" : "TKUtils.ViewerTooltips.ModViewer").Translate(entry.Viewer.username)
            );

            actionBtnRegion = actionBtnRegion.Shift(Direction8Way.West, 2f);

            if (Widgets.ButtonImage(actionBtnRegion, Textures.Reset))
            {
                ResetViewer(entry.Viewer);
            }

            TooltipHandler.TipRegion(actionBtnRegion, "TKUtils.ViewerTooltips.ResetViewer".Translate(entry.Viewer.username));
        }

        private void DrawCoinsEditor(Rect region)
        {
            int labelWidth = Mathf.FloorToInt(region.width * 0.25f);
            float fieldWidth = Mathf.FloorToInt(region.width * 0.4f);

            var labelRegion = new Rect(0f, 0f, labelWidth, region.height);
            var fieldRegion = new Rect(labelRegion.width + 5f, 0f, fieldWidth, region.height);
            Rect editBtnRegion = LayoutHelper.IconRect(fieldRegion.x + fieldRegion.width + 5f, 0f, region.height, region.height);
            Rect addBtnRegion = editBtnRegion.Shift(Direction8Way.East, 2f);
            Rect removeBtnRegion = addBtnRegion.Shift(Direction8Way.East, 2f);

            UiHelper.Label(labelRegion, _coinsText);

            if (Widgets.ButtonImage(editBtnRegion, _editingCoins ? Widgets.CheckboxOnTex : Textures.Edit))
            {
                _editingCoins = !_editingCoins;

                if (_editingCoins)
                {
                    _coinBuffer = _editor.Coins.ToString();
                    _coinBufferValid = true;
                }
            }

            if (!_editingCoins || !_editor.HasViewers)
            {
                UiHelper.Label(fieldRegion, _editor.Coins.ToString("N0"));

                return;
            }

            if (UiHelper.NumberField(fieldRegion, out int newCoins, ref _coinBuffer, ref _coinBufferValid))
            {
                _editor.Coins = newCoins;
            }

            if (Widgets.ButtonImage(addBtnRegion, TexButton.Add))
            {
                Find.WindowStack.Add(new IntegerEntryDialog("TKUtils.ViewerDialog.AddCoins".Translate(_editor.EditingUser)) { OnAccept = AddCoins });
            }

            if (Widgets.ButtonImage(removeBtnRegion, TexButton.Minus))
            {
                Find.WindowStack.Add(new IntegerEntryDialog("TKUtils.ViewerDialog.RemoveCoins".Translate(_editor.EditingUser)) { OnAccept = SubtractCoins });
            }
        }

        private void DrawKarmaEditor(Rect region)
        {
            int labelWidth = Mathf.FloorToInt(region.width * 0.25f);
            float fieldWidth = Mathf.FloorToInt(region.width * 0.4f);

            var labelRegion = new Rect(0f, 0f, labelWidth, region.height);
            var fieldRegion = new Rect(labelRegion.width + 5f, 0f, fieldWidth, region.height);
            Rect editBtnRegion = LayoutHelper.IconRect(fieldRegion.x + fieldRegion.width + 5f, 0f, region.height, region.height);
            Rect addBtnRegion = editBtnRegion.Shift(Direction8Way.East, 2f);
            Rect removeBtnRegion = addBtnRegion.Shift(Direction8Way.East, 2f);

            UiHelper.Label(labelRegion, _karmaText);

            if (Widgets.ButtonImage(editBtnRegion, _editingKarma ? Widgets.CheckboxOnTex : Textures.Edit))
            {
                _editingKarma = !_editingKarma;

                if (_editingKarma)
                {
                    UpdateKarmaBuffer();
                }
            }

            if (!_editingKarma || !_editor.HasViewers)
            {
                UiHelper.Label(fieldRegion, _editor.Karma.ToString("N0"));

                return;
            }

            if (UiHelper.NumberField(fieldRegion, out int newKarma, ref _karmaBuffer, ref _karmaBufferValid))
            {
                _editor.Karma = newKarma;
            }

            if (Widgets.ButtonImage(addBtnRegion, TexButton.Add))
            {
                Find.WindowStack.Add(new IntegerEntryDialog("TKUtils.ViewerDialog.AddKarma".Translate(_editor.EditingUser)) { OnAccept = AddKarma });
            }

            if (Widgets.ButtonImage(removeBtnRegion, TexButton.Minus))
            {
                Find.WindowStack.Add(new IntegerEntryDialog("TKUtils.ViewerDialog.RemoveKarma".Translate(_editor.EditingUser)) { OnAccept = SubtractKarma });
            }
        }

        private void DrawViewerList(Rect region)
        {
            float lineHeight = Text.LineHeight;
            Rect groupRegion = LayoutHelper.IconRect(0f, 0f, lineHeight, lineHeight);
            var groupActiveRegion = new Rect(lineHeight - 16f, lineHeight - 16f, 16f, 16f);
            var searchRegion = new Rect(lineHeight, 0f, region.width - lineHeight, lineHeight);
            var listRegion = new Rect(0f, lineHeight + 7f, region.width, region.height - lineHeight - 7f);

            float lineSpan = lineHeight * _viewerCount;
            var listView = new Rect(0f, 0f, region.width - (lineHeight > listRegion.height ? 16f : 0f), lineSpan);

            GUI.color = new Color(0.52f, 0.52f, 0.52f);
            Widgets.DrawLineHorizontal(5f, lineHeight + 5f, region.width - 10f);
            GUI.color = Color.white;

            if (Widgets.ButtonImage(groupRegion, Textures.Group))
            {
                UpdateEveryoneStatus();
            }

            UiHelper.Icon(groupActiveRegion, _editor.IsMulti ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex, Color.white);

            GUI.BeginGroup(searchRegion);
            _searchWidget.OnGUI(searchRegion.AtZero(), OnSearchQueryChanged);
            GUI.EndGroup();

            GUI.BeginGroup(listRegion);
            _listScrollPos = GUI.BeginScrollView(listRegion.AtZero(), _listScrollPos, listView);

            var index = 0;

            foreach (ViewerEntry entry in _viewers)
            {
                if (!entry.Visible)
                {
                    continue;
                }

                var lineRegion = new Rect(0f, index++ * lineHeight, listView.width, lineHeight);

                if (!lineRegion.IsVisible(listRegion, _listScrollPos))
                {
                    continue;
                }

                GUI.BeginGroup(lineRegion);
                lineRegion = lineRegion.AtZero();

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRegion);
                }

                if (_editor.IsMulti || entry.Viewer == _editor.User?.Viewer)
                {
                    Widgets.DrawHighlightSelected(lineRegion);
                }

                DrawViewerEntry(lineRegion, entry);
                GUI.EndGroup();
            }

            GUI.EndScrollView();
            GUI.EndGroup();
        }

        private void DrawViewerEntry(Rect region, [NotNull] ViewerEntry entry)
        {
            Rect buttonRegion = LayoutHelper.IconRect(region.width - region.height, 0f, region.height, region.height);
            UiHelper.Label(region, entry.Viewer!.username);

            if (Widgets.ButtonImage(buttonRegion, Widgets.CheckboxOffTex))
            {
                Find.WindowStack.Add(new ConfirmationDialog("TKUtils.ViewerDialog.DeleteConfirmation".Translate(entry.Viewer.username), () => DeleteViewerData(entry)));
            }

            if (!Widgets.ButtonInvisible(region))
            {
                return;
            }

            _editor.ClearViewers();
            _editor.SetViewers(entry);
        }

        private void DeleteViewerData([NotNull] ViewerEntry entry)
        {
            Viewers.All.Remove(entry.Viewer);

            _viewers.Remove(entry);
            _viewersKeyed.Remove(entry.Viewer.username);
        }

        private void ResetViewers()
        {
            foreach (ViewerEntry entry in _viewers)
            {
                ResetViewer(entry.Viewer);
            }
        }

        private static void ResetViewer([NotNull] Viewer viewer)
        {
            viewer.coins = ToolkitSettings.StartingBalance;
            viewer.karma = ToolkitSettings.StartingKarma;
        }

        private void UpdateEveryoneStatus()
        {
            _searchWidget.filter.Text = "";

            UpdateCoinBuffer();
            UpdateKarmaBuffer();
            OnSearchQueryChanged();

            if (!_editor.IsMulti)
            {
                _editor.ClearViewers();
                _editor.SetViewers(_viewers.ToArray());
            }
            else
            {
                _editor.ClearViewers();
            }
        }

        private void OnSearchQueryChanged()
        {
            foreach (ViewerEntry entry in _viewers)
            {
                entry.Visible = ShouldBeVisible(entry.Viewer);
            }
        }

        private bool ShouldBeVisible(Viewer viewer)
        {
            if (string.IsNullOrEmpty(_searchWidget.filter.Text))
            {
                return true;
            }

            return viewer.username.IndexOf(_searchWidget.filter.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void AddKarma(int karma)
        {
            _editor.Karma += karma;

            UpdateKarmaBuffer();
        }

        private void SubtractKarma(int karma)
        {
            _editor.Karma -= karma;

            UpdateKarmaBuffer();
        }

        private void AddCoins(int coins)
        {
            _editor.Coins += coins;

            UpdateCoinBuffer();
        }

        private void SubtractCoins(int coins)
        {
            _editor.Coins -= coins;

            UpdateCoinBuffer();
        }

        private void UpdateKarmaBuffer()
        {
            _karmaBufferValid = true;
            _karmaBuffer = _editor.Karma.ToString();
        }

        private void UpdateCoinBuffer()
        {
            _coinBufferValid = true;
            _coinBuffer = _editor.Coins.ToString();
        }

        private sealed class ViewerEntry
        {
            private bool _isBanned;
            private bool _isMod;
            private Viewer _viewer;

            public Viewer Viewer
            {
                get => _viewer;
                set
                {
                    _viewer = value;
                    _isMod = _viewer.IsModerator();
                    _isBanned = _viewer.IsBanned;
                }
            }

            public bool Visible { get; set; }

            public bool IsModerator
            {
                get => _isMod;
                set
                {
                    _isMod = value;

                    switch (_isMod)
                    {
                        case true:
                            _viewer?.SetAsModerator();

                            break;
                        case false:
                            _viewer?.RemoveAsModerator();

                            break;
                    }
                }
            }

            public bool IsBanned
            {
                get => _isBanned;
                set
                {
                    _isBanned = value;

                    switch (_isBanned)
                    {
                        case true:
                            _viewer?.BanViewer();

                            break;
                        case false:
                            _viewer?.UnBanViewer();

                            break;
                    }
                }
            }
        }

        private sealed class ViewerEditorWorker
        {
            private readonly List<ViewerEntry> _viewers = new List<ViewerEntry>();
            private int _coins;
            private int _karma;
            private int _viewerCount;

            public bool HasViewers => _viewerCount > 0;
            public bool IsSingle => _viewerCount == 1;
            public bool IsMulti => _viewerCount > 1;

            public int Coins
            {
                get => _coins;
                set
                {
                    _coins = value;

                    foreach (ViewerEntry entry in _viewers)
                    {
                        entry.Viewer.coins = _coins;
                    }
                }
            }

            public int Karma
            {
                get => _karma;
                set
                {
                    _karma = value;

                    foreach (ViewerEntry entry in _viewers)
                    {
                        entry.Viewer.karma = _karma;
                    }
                }
            }

            public string EditingUser { get; private set; }

            [CanBeNull] public ViewerEntry User => _viewerCount <= 0 ? null : _viewers[0];
            [NotNull] public List<ViewerEntry> Users => new List<ViewerEntry>(_viewers);

            public void AddViewers([NotNull] params ViewerEntry[] viewers)
            {
                _viewers.AddRange(viewers);
                TkUtils.Logger.Warn($"{_viewerCount} + {viewers.Length} = {_viewerCount + viewers.Length}");

                _viewerCount += viewers.Length;

                RecalculateCoins();
                RecalculateKarma();
                UpdateEditingText();
            }

            public void RemoveViewers([NotNull] params ViewerEntry[] viewers)
            {
                var changed = false;

                for (int i = viewers.Length - 1; i >= 0; i--)
                {
                    ViewerEntry viewer = viewers[i];

                    if (!_viewers.Remove(viewer))
                    {
                        continue;
                    }

                    _viewerCount--;
                    changed = true;
                }

                if (!changed)
                {
                    return;
                }

                RecalculateCoins();
                RecalculateKarma();
                UpdateEditingText();
            }

            public void SetViewers([NotNull] params ViewerEntry[] viewers)
            {
                _viewers.Clear();
                _viewers.AddRange(viewers);
                _viewerCount = _viewers.Count;

                RecalculateCoins();
                RecalculateKarma();
                UpdateEditingText();
            }

            public void ClearViewers()
            {
                _viewers.Clear();
                _viewerCount = 0;
                _karma = 0;
                _coins = 0;

                UpdateEditingText();
            }

            private void RecalculateCoins()
            {
                var coins = 0;

                foreach (ViewerEntry entry in _viewers)
                {
                    coins += entry.Viewer.coins;
                }

                _coins = Mathf.FloorToInt(coins / (float)_viewerCount);
            }

            private void RecalculateKarma()
            {
                var karma = 0;

                foreach (ViewerEntry entry in _viewers)
                {
                    karma += entry.Viewer.karma;
                }

                _karma = Mathf.FloorToInt(karma / (float)_viewerCount);
            }

            private void UpdateEditingText()
            {
                if (_viewerCount > 1)
                {
                    EditingUser = "everyone";

                    return;
                }

                if (_viewerCount == 1)
                {
                    EditingUser = _viewers[0].Viewer.username;

                    return;
                }

                EditingUser = "None".TranslateSimple();
            }
        }
    }
}
