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
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows;

/// <summary>
///     A dialog for assigning viewers to the pawns that may appear
///     in-game.
/// </summary>
public class NameQueueDialog : Window
{
    private readonly List<Pawn> _allPawns;
    private readonly GameComponentPawns _pawnComponent;
    private string _applyText;

    private float _applyTextWidth;
    private string _assignedText;
    private string _assignedTooltip;
    private string _assignedToText;
    private float _assignedToTextWidth;
    private string _banViewerTooltip;
    private string _configureViewerTooltip;
    private string _countText;
    private Pawn _current;

    private Texture2D _currentDiceSide;
    private string _emptyQueueText;
    private string _lastSeenText;
    private string _nextTooltip;
    private string _notifyText;
    private float _notifyTextWidth;
    private string _notSeenText;
    private string _pawnTooltip;
    private string _previousTooltip;
    private string _randomTooltip;
    private string _removeViewerTooltip;
    private Vector2 _scrollPos = Vector2.zero;
    private float _timestamp;

    private string _unassignedText;
    private string _unassignedTooltip;
    private bool _userFromButton;
    private string _username;
    private Rect _usernameFieldPosition;
    private Viewer _viewer;
    private string _viewerDrawnText;
    private string _viewerTooltip;

    public NameQueueDialog()
    {
        GetTranslations();

        doCloseX = true;
        forcePause = true;
        _pawnComponent = Current.Game.GetComponent<GameComponentPawns>();

        _allPawns = Find.ColonistBar.Entries.Select(e => e.pawn).ToList();
        _currentDiceSide = Textures.DiceSides.RandomElement();

        _current = _allPawns.FirstOrDefault(p => !_pawnComponent.HasPawnBeenNamed(p)) ?? _allPawns.FirstOrDefault();
        Notify__CurrentPawnChanged();
    }

    /// <inheritdoc cref="Window.InitialSize"/>
    public override Vector2 InitialSize => new Vector2(500, 450);

    private void GetTranslations()
    {
        _emptyQueueText = "TKUtils.NameQueue.None".TranslateSimple();
        _assignedText = "TKUtils.NameQueue.Assigned".TranslateSimple();
        _unassignedText = "TKUtils.NameQueue.Unassigned".TranslateSimple();
        _applyText = "TKUtils.Buttons.Apply".TranslateSimple();
        _countText = "TKUtils.NameQueue.Count".TranslateSimple();
        _viewerTooltip = "TKUtils.NameQueueTooltips.ViewerName".TranslateSimple();
        _unassignedTooltip = "TKUtils.NameQueueTooltips.Unassigned".TranslateSimple();
        _assignedTooltip = "TKUtils.NameQueueTooltips.Assigned".TranslateSimple();
        _nextTooltip = "TKUtils.NameQueueTooltips.Next".TranslateSimple();
        _previousTooltip = "TKUtils.NameQueueTooltips.Previous".TranslateSimple();
        _pawnTooltip = "TKUtils.NameQueueTooltips.Pawn".TranslateSimple();
        _randomTooltip = "TKUtils.NameQueueTooltips.Random".TranslateSimple();
        _assignedToText = "TKUtils.NameQueue.AssignedTo".TranslateSimple();
        _notifyText = "TKUtils.Buttons.Notify".TranslateSimple();
        _notSeenText = "TKUtils.NameQueue.NotSeen".TranslateSimple();
        _viewerDrawnText = "TKUtils.ViewerDrawn".TranslateSimple();
        _banViewerTooltip = "TKUtils.NameQueueTooltips.Ban".TranslateSimple();
        _removeViewerTooltip = "TKUtils.NameQueueTooltips.Remove".TranslateSimple();
        _configureViewerTooltip = "TKUtils.NameQueueTooltips.Configure".TranslateSimple();

        _applyTextWidth = Text.CalcSize(_applyText).x + 16f;
        _notifyTextWidth = Text.CalcSize(_notifyText).x + 16f;
        _assignedToTextWidth = Text.CalcSize(_assignedToText).x + 16f;
    }

    /// <inheritdoc cref="Window.DoWindowContents"/>
    public override void DoWindowContents(Rect inRect)
    {
        if (Event.current.type == EventType.Layout)
        {
            return;
        }

        GUI.BeginGroup(inRect);
        Text.Font = GameFont.Small;

        ProcessShortcutKeys();
        Rect pawnRect = new Rect(0f, 0f, inRect.width * 0.3333f, 152f + Text.LineHeight).Rounded();
        var contentRect = new Rect(pawnRect.width + 10f, 0f, inRect.width - pawnRect.width - Margin, pawnRect.height);
        var queueRect = new Rect(0f, pawnRect.height + 50f, inRect.width, inRect.height - pawnRect.height - 50f);

        GUI.BeginGroup(pawnRect);
        DrawPawnSection(pawnRect);
        GUI.EndGroup();


        GUI.BeginGroup(contentRect);
        DrawContent(contentRect.AtZero());
        GUI.EndGroup();

        GUI.BeginGroup(queueRect);
        var listing = new Listing_Standard();
        var noticeRect = new Rect(0f, 0f, queueRect.width - Text.LineHeight - 5f, Text.LineHeight);
        var randomRect = new Rect(queueRect.width - Text.LineHeight, noticeRect.y, Text.LineHeight, Text.LineHeight);
        var nameQueueRect = new Rect(0f, noticeRect.height + 5f, queueRect.width, queueRect.height - Text.LineHeight - 5f);
        var queueInnerRect = new Rect(0f, 0f, nameQueueRect.width, nameQueueRect.height);
        var queueView = new Rect(0f, 0f, nameQueueRect.width - 16f, Text.LineHeight * _pawnComponent.viewerNameQueue.Count);

        if (queueView.height <= nameQueueRect.height)
        {
            queueView.height += 16;
        }

        if (_pawnComponent.ViewerNameQueue.Count <= 0)
        {
            Widgets.Label(noticeRect, _emptyQueueText);
            GUI.EndGroup();

            return;
        }

        Widgets.Label(noticeRect, $"{_pawnComponent.ViewerNameQueue.Count:N0} {_countText}");
        TooltipHandler.TipRegion(randomRect, _randomTooltip);

        if (Widgets.ButtonImage(randomRect, _currentDiceSide))
        {
            _username = _pawnComponent.ViewerNameQueue.RandomElement();
            _userFromButton = true;
            _viewer = Viewers.GetViewer(_username);
            UpdateLastSeenText();
        }

        GUI.BeginGroup(nameQueueRect);
        Widgets.BeginScrollView(queueInnerRect, ref _scrollPos, queueView);
        listing.Begin(queueView);
        DrawNameQueue(listing);

        GUI.EndGroup();
        listing.End();
        Widgets.EndScrollView();
        GUI.EndGroup();
        GUI.EndGroup();
    }

    private void UpdateLastSeenText()
    {
        if (_viewer?.last_seen == null)
        {
            _lastSeenText = _notSeenText;
        }

        TimeSpan diff = DateTime.Now - _viewer!.last_seen;

        if (diff.TotalHours >= 2)
        {
            _lastSeenText = _notSeenText;
        }

        _lastSeenText = "TKUtils.NameQueue.LastSeen".Translate(diff.TotalMinutes.ToString("N2"));
    }

    private void DrawContent(Rect contentRect)
    {
        var listing = new Listing_Standard(GameFont.Small);
        listing.Begin(contentRect);

        Rect usernameRect = listing.GetRect(Text.LineHeight);
        (Rect usernameLabel, Rect usernameFieldHalf) = usernameRect.Split(_assignedToTextWidth / usernameRect.width);
        UiHelper.Label(usernameLabel, _assignedToText);

        _usernameFieldPosition = new Rect(usernameFieldHalf.x, usernameFieldHalf.y, usernameFieldHalf.width - _applyTextWidth - 5f, usernameFieldHalf.height);
        _username = Widgets.TextField(_usernameFieldPosition, _username).ToToolkit();

        if (_username.Length > 0 && UiHelper.ClearButton(_usernameFieldPosition))
        {
            _username = "";
            _userFromButton = false;
            _viewer = null;
            _lastSeenText = null;
        }

        if (_viewer != null && !_viewer.username.EqualsIgnoreCase(_username))
        {
            _userFromButton = false;
            _viewer = null;
            _lastSeenText = null;
        }

        var usernameApply = new Rect(_usernameFieldPosition.x + _usernameFieldPosition.width + 5f, _usernameFieldPosition.y, _applyTextWidth, Text.LineHeight);

        if (Widgets.ButtonText(usernameApply, _applyText))
        {
            AssignColonist();
        }

        if (!_userFromButton || _viewer == null)
        {
            listing.End();

            return;
        }

        listing.GapLine(36f);
        Rect seenAtLine = listing.GetRect(Text.LineHeight * 2f);
        seenAtLine = seenAtLine.Trim(Direction8Way.East, seenAtLine.width - 5f - _notifyTextWidth);
        var notifyButton = new Rect(seenAtLine.x + seenAtLine.width + 5f, seenAtLine.y, _notifyTextWidth, Mathf.FloorToInt(seenAtLine.height / 2f));
        UiHelper.Label(seenAtLine, _lastSeenText);

        if (Widgets.ButtonText(notifyButton, _notifyText))
        {
            MessageHelper.ReplyToUser(_viewer.username, _viewerDrawnText);
        }

        listing.End();
    }

    private void DrawNameQueue(Listing listing)
    {
        for (var index = 0; index < _pawnComponent.ViewerNameQueue.Count; index++)
        {
            string name = _pawnComponent.ViewerNameQueue[index];

            Rect line = listing.GetRect(Text.LineHeight);
            var buttonRect = new Rect(line.x + line.width - line.height * 3f, line.y, line.height * 3f, line.height);
            var nameRect = new Rect(line.x, line.y, line.width - buttonRect.width - 5f, line.height);

            if (index % 2 == 0)
            {
                Widgets.DrawLightHighlight(line);
            }

            if (name.EqualsIgnoreCase(_username))
            {
                Widgets.DrawHighlightSelected(line);
            }

            Widgets.DrawHighlightIfMouseover(line);
            DrawNameFromQueue(nameRect, name, buttonRect, index);
        }
    }

    private void DrawNameFromQueue(Rect nameRect, string name, Rect buttonRect, int index)
    {
        var buttonTemplateRect = new Rect(buttonRect.x, buttonRect.y, buttonRect.height, buttonRect.height);
        Widgets.Label(nameRect, name);

        if (Widgets.ButtonInvisible(nameRect))
        {
            _username = name.ToLowerInvariant();
        }

        if (Widgets.ButtonImage(buttonTemplateRect, Textures.Gear))
        {
            OpenViewerDetailsFor(name);
        }

        buttonTemplateRect.TipRegion(_configureViewerTooltip);
        buttonTemplateRect = buttonTemplateRect.Shift(Direction8Way.East, 0f);

        var remove = false;

        if (Widgets.ButtonImage(buttonTemplateRect, Textures.Hammer))
        {
            Viewers.GetViewer(name).BanViewer();
            remove = true;
        }

        buttonTemplateRect.TipRegion(_banViewerTooltip);

        buttonTemplateRect = buttonTemplateRect.Shift(Direction8Way.East, 0f);

        if (Widgets.ButtonImage(buttonTemplateRect, Widgets.CheckboxOffTex))
        {
            remove = true;
        }

        buttonTemplateRect.TipRegion(_removeViewerTooltip);

        if (!remove)
        {
            return;
        }

        try
        {
            _pawnComponent.ViewerNameQueue.RemoveAt(index);
        }
        catch (IndexOutOfRangeException)
        {
            // Shouldn't happen, but we'll handle it anyway.
        }
    }

    private static void OpenViewerDetailsFor(string name)
    {
        var viewers = new Window_Viewers();
        viewers.SelectViewer(Viewers.GetViewer(name));
        Find.WindowStack.Add(viewers);
    }

    private void DrawPawnSection(Rect canvas)
    {
        float widthMidpoint = canvas.width / 2f;
        float arrowMax = Mathf.Max(TexUI.ArrowTexLeft.width, TexUI.ArrowTexRight.width) * 0.6f;
        var colonistRect = new Rect(widthMidpoint - 50f, 5f, 100f, 140f);
        Rect previousRect = new Rect(5f, 5f + colonistRect.height / 2 - arrowMax / 2f, arrowMax, arrowMax).Rounded();
        var nextRect = new Rect(canvas.width - arrowMax - 5f, previousRect.y, previousRect.width, previousRect.height);
        var nameRect = new Rect(5f, colonistRect.y + colonistRect.height + 2f, canvas.width - 10f, Text.LineHeight);
        var viewerRect = new Rect(5f, nameRect.y + nameRect.height + 2f, canvas.width - 10f, Text.LineHeight);
        var stateRect = new Rect(5f, viewerRect.y + viewerRect.height + 2f, canvas.width - 10f, Text.LineHeight);

        Widgets.DrawMenuSection(canvas);
        TooltipHandler.TipRegion(nextRect, _nextTooltip);
        TooltipHandler.TipRegion(previousRect, _previousTooltip);
        TooltipHandler.TipRegion(colonistRect, _pawnTooltip);

        if (_allPawns.Count > 1 && Widgets.ButtonImage(previousRect, TexUI.ArrowTexLeft))
        {
            GoToPreviousPawn();
        }

        if (_allPawns.Count > 1 && Widgets.ButtonImage(nextRect, TexUI.ArrowTexRight))
        {
            GoToNextPawn();
        }

        if (_current == null)
        {
            return;
        }

        GUI.DrawTexture(
            colonistRect,
            PortraitsCache.Get(_current, ColonistBarColonistDrawer.PawnTextureSize, Rot4.South, ColonistBarColonistDrawer.PawnTextureCameraOffset, 1.28205f)
        );

        Widgets.DrawHighlightIfMouseover(colonistRect);

        if (Widgets.ButtonInvisible(colonistRect))
        {
            OpenInfoDialog();
        }

        if (!(_current.Name is NameTriple name))
        {
            return;
        }

        bool viewerAssigned = _pawnComponent.pawnHistory.Any(pair => pair.Key.Equals(name.Nick, StringComparison.InvariantCultureIgnoreCase) && pair.Value == _current);
        UiHelper.Label(nameRect, $"{name.First} {name.Last}", TextAnchor.MiddleCenter);
        UiHelper.Label(viewerRect, name.Nick?.CapitalizeFirst(), TextAnchor.MiddleCenter);

        DoStateMouseActions(stateRect, viewerAssigned, name.Nick);
        TooltipHandler.TipRegion(viewerRect, _viewerTooltip);
        Widgets.DrawHighlightIfMouseover(viewerRect);

        if (!name.Nick.NullOrEmpty() && Widgets.ButtonInvisible(viewerRect))
        {
            _username = name.Nick;
        }
    }

    private void OpenInfoDialog()
    {
        var infoCard = new Dialog_InfoCard(_current);
        Find.WindowStack.Add(infoCard);
    }

    private void DoStateMouseActions(Rect canvas, bool assigned, string viewerName)
    {
        if (_current == null || viewerName.NullOrEmpty())
        {
            return;
        }

        UiHelper.Label(canvas, (assigned ? _assignedText : _unassignedText).CapitalizeFirst(), TextAnchor.MiddleCenter);
        Widgets.DrawHighlightIfMouseover(canvas);
        TooltipHandler.TipRegion(canvas, assigned ? _assignedTooltip : _unassignedTooltip);

        if (!Widgets.ButtonInvisible(canvas))
        {
            return;
        }

        if (assigned)
        {
            _pawnComponent.pawnHistory.Remove(viewerName.ToLowerInvariant());

            var pawnName = _current.Name as NameTriple;
            _current.Name = new NameTriple(pawnName?.First, pawnName?.Last, pawnName?.Last);
        }
        else
        {
            _pawnComponent.pawnHistory.Add(viewerName.ToLowerInvariant(), _current);
        }
    }

    private void GoToNextPawn()
    {
        int index = _allPawns.IndexOf(_current);

        _current = _allPawns[index == _allPawns.Count - 1 ? 0 : index + 1];
        Notify__CurrentPawnChanged();
    }

    private void GoToPreviousPawn()
    {
        int index = Mathf.Max(_allPawns.IndexOf(_current), 0);

        _current = _allPawns[index == 0 ? _allPawns.Count - 1 : index - 1];
        Notify__CurrentPawnChanged();
    }

    private void ProcessShortcutKeys()
    {
        if (GUIUtility.keyboardControl > 0 || Event.current.type != EventType.KeyDown)
        {
            return;
        }

        bool isControlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        switch (Event.current.keyCode)
        {
            case KeyCode.RightArrow when isControlPressed:
                NextUnnamedColonist();
                Event.current.Use();

                break;
            case KeyCode.LeftArrow when isControlPressed:
                PreviousUnnamedColonist();
                Event.current.Use();

                break;
            case KeyCode.LeftArrow:
                GoToPreviousPawn();
                Event.current.Use();

                break;
            case KeyCode.RightArrow:
                GoToNextPawn();
                Event.current.Use();

                break;
            case KeyCode.F2 when _current != null:
                OpenInfoDialog();
                Event.current.Use();

                break;
        }
    }

    private void Notify__CurrentPawnChanged()
    {
        string assigned = _pawnComponent.pawnHistory.Where(p => p.Value == _current).Select(p => p.Key).FirstOrDefault();

        _username = assigned ?? "";
    }

    private void AssignColonist()
    {
        if (_pawnComponent.HasPawnBeenNamed(_current))
        {
            string key = _pawnComponent.pawnHistory.Where(p => p.Value == _current).Select(p => p.Key).FirstOrDefault();

            if (key != null)
            {
                _pawnComponent.pawnHistory.Remove(key);
            }
        }

        var name = _current.Name as NameTriple;

        _current.Name = new NameTriple(name?.First, _username.NullOrEmpty() ? name?.Last : _username, name?.Last);

        if (!_username.NullOrEmpty())
        {
            _pawnComponent.AssignUserToPawn(_username.ToLowerInvariant(), _current);
        }

        NextUnnamedColonist();
    }

    private void NextUnnamedColonist()
    {
        _current = _allPawns.Where(p => CompatRegistry.Magic?.IsUndead(p) != true)
           .FirstOrDefault(p => _pawnComponent.pawnHistory.All(pair => pair.Value != p) && p != _current);

        if (_current == null)
        {
            GoToNextPawn();
        }

        Notify__CurrentPawnChanged();
    }

    private void PreviousUnnamedColonist()
    {
        _current = _allPawns.Where(p => !CompatRegistry.Magic?.IsUndead(p) ?? false)
           .LastOrDefault(p => _pawnComponent.pawnHistory.All(pair => pair.Value != p) && p != _current);

        if (_current == null)
        {
            GoToPreviousPawn();
        }

        Notify__CurrentPawnChanged();
    }

    /// <inheritdoc cref="Window.WindowUpdate"/>
    public override void WindowUpdate()
    {
        base.WindowUpdate();

        int startup = Mathf.FloorToInt(Time.time);

        if (_timestamp >= startup || startup % 3 >= 1)
        {
            return;
        }

        _currentDiceSide = Textures.DiceSides.RandomElement();
        _timestamp = startup;

        if (_viewer == null)
        {
            return;
        }

        UpdateLastSeenText();
    }

    /// <inheritdoc cref="Window.PreOpen"/>
    public override void PreOpen()
    {
        base.PreOpen();

        if (_pawnComponent != null)
        {
            ReconnectViewers();

            return;
        }

        TkUtils.Logger.Warn("Pawn game component was null!");
        Close();
    }

    private void ReconnectViewers()
    {
        foreach (Pawn pawn in Find.ColonistBar.GetColonistsInOrder())
        {
            var pawnName = pawn.Name as NameTriple;

            if (pawnName == null)
            {
                continue;
            }

            if (pawnName.Nick.NullOrEmpty())
            {
                continue;
            }

            Viewer tempViewer = Viewers.All.FirstOrDefault(v => v.username.Equals(pawnName.Nick, StringComparison.InvariantCultureIgnoreCase));

            if (tempViewer == null)
            {
                string assignment = _pawnComponent.UserAssignedToPawn(pawn);

                if (assignment != null)
                {
                    _pawnComponent.pawnHistory.Remove(assignment);
                    pawn.Name = new NameTriple(pawnName.First, pawnName.Last, pawnName.Last);
                }

                continue;
            }

            _pawnComponent.pawnHistory[tempViewer.username] = pawn;
            _pawnComponent.viewerNameQueue.RemoveAll(u => u.Equals(tempViewer.username, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    /// <inheritdoc cref="Window.OnAcceptKeyPressed"/>
    public override void OnAcceptKeyPressed()
    {
        if (GUIUtility.keyboardControl == GUIUtility.GetControlID(FocusType.Keyboard, _usernameFieldPosition))
        {
            AssignColonist();
            Event.current.Use();
            GUIUtility.keyboardControl = 0;

            return;
        }

        base.OnAcceptKeyPressed();
    }

    /// <inheritdoc cref="Window.OnCancelKeyPressed"/>
    public override void OnCancelKeyPressed()
    {
        if (GUIUtility.keyboardControl == GUIUtility.GetControlID(FocusType.Keyboard, _usernameFieldPosition))
        {
            Event.current.Use();
            GUIUtility.keyboardControl = 0;

            return;
        }

        base.OnCancelKeyPressed();
    }
}