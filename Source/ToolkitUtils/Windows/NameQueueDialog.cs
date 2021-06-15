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
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class NameQueueDialog : Window
    {
        private readonly List<Pawn> allPawns;
        private readonly GameComponentPawns pawnComponent;
        private string applyText;

        private float applyTextWidth;
        private string assignedText;
        private string assignedTooltip;
        private string assignedToText;
        private float assignedToTextWidth;
        private string countText;
        private Pawn current;

        private Texture2D currentDiceSide;
        private string emptyQueueText;
        private string lastSeenText;
        private string nextTooltip;
        private string notifyText;
        private float notifyTextWidth;
        private string notSeenText;
        private string pawnTooltip;
        private string previousTooltip;
        private string randomTooltip;
        private Vector2 scrollPos = Vector2.zero;
        private float timestamp;

        private string unassignedText;
        private string unassignedTooltip;
        private bool userFromButton;
        private string username;
        private Rect usernameFieldPosition;
        private Viewer viewer;
        private string viewerDrawnText;
        private string viewerTooltip;

        public NameQueueDialog()
        {
            GetTranslations();

            doCloseX = true;
            forcePause = true;
            pawnComponent = Current.Game.GetComponent<GameComponentPawns>();

            allPawns = Find.ColonistBar.Entries.Select(e => e.pawn).ToList();
            currentDiceSide = Textures.DiceSides.RandomElement();

            current = allPawns.FirstOrDefault(p => !pawnComponent.HasPawnBeenNamed(p)) ?? allPawns.FirstOrDefault();
            Notify__CurrentPawnChanged();
        }

        public override Vector2 InitialSize => new Vector2(500, 450);


        private void GetTranslations()
        {
            emptyQueueText = "TKUtils.NameQueue.None".Localize();
            assignedText = "TKUtils.NameQueue.Assigned".Localize();
            unassignedText = "TKUtils.NameQueue.Unassigned".Localize();
            applyText = "TKUtils.Buttons.Apply".Localize();
            countText = "TKUtils.NameQueue.Count".Localize();
            viewerTooltip = "TKUtils.NameQueueTooltips.ViewerName".Localize();
            unassignedTooltip = "TKUtils.NameQueueTooltips.Unassigned".Localize();
            assignedTooltip = "TKUtils.NameQueueTooltips.Assigned".Localize();
            nextTooltip = "TKUtils.NameQueueTooltips.Next".Localize();
            previousTooltip = "TKUtils.NameQueueTooltips.Previous".Localize();
            pawnTooltip = "TKUtils.NameQueueTooltips.Pawn".Localize();
            randomTooltip = "TKUtils.NameQueueTooltips.Random".Localize();
            assignedToText = "TKUtils.NameQueue.AssignedTo".Localize();
            notifyText = "TKUtils.Buttons.Notify".Localize();
            notSeenText = "TKUtils.NameQueue.NotSeen".Localize();
            viewerDrawnText = "TKUtils.ViewerDrawn".Localize();

            applyTextWidth = Text.CalcSize(applyText).x + 16f;
            notifyTextWidth = Text.CalcSize(notifyText).x + 16f;
            assignedToTextWidth = Text.CalcSize(assignedToText).x + 16f;
        }

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
            var contentRect = new Rect(
                pawnRect.width + 10f,
                0f,
                inRect.width - pawnRect.width - Margin,
                pawnRect.height
            );
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
            var randomRect = new Rect(
                queueRect.width - Text.LineHeight,
                noticeRect.y,
                Text.LineHeight,
                Text.LineHeight
            );
            var nameQueueRect = new Rect(
                0f,
                noticeRect.height + 5f,
                queueRect.width,
                queueRect.height - Text.LineHeight - 5f
            );
            var queueInnerRect = new Rect(0f, 0f, nameQueueRect.width, nameQueueRect.height);
            var queueView = new Rect(
                0f,
                0f,
                nameQueueRect.width - 16f,
                Text.LineHeight * pawnComponent.viewerNameQueue.Count
            );

            if (queueView.height <= nameQueueRect.height)
            {
                queueView.height += 16;
            }

            if (pawnComponent.ViewerNameQueue.Count <= 0)
            {
                Widgets.Label(noticeRect, emptyQueueText);
                GUI.EndGroup();
                return;
            }

            Widgets.Label(noticeRect, $"{pawnComponent.ViewerNameQueue.Count:N0} {countText}");
            TooltipHandler.TipRegion(randomRect, randomTooltip);

            if (Widgets.ButtonImage(randomRect, currentDiceSide))
            {
                username = pawnComponent.ViewerNameQueue.RandomElement();
                userFromButton = true;
                viewer = Viewers.GetViewer(username);
                UpdateLastSeenText();
            }

            GUI.BeginGroup(nameQueueRect);
            listing.BeginScrollView(queueInnerRect, ref scrollPos, ref queueView);
            DrawNameQueue(listing);

            GUI.EndGroup();
            listing.EndScrollView(ref queueView);
            GUI.EndGroup();
            GUI.EndGroup();
        }

        private void UpdateLastSeenText()
        {
            lastSeenText = viewer?.last_seen == null
                ? notSeenText
                : "TKUtils.NameQueue.LastSeen".LocalizeKeyed(
                    (DateTime.Now - viewer.last_seen).TotalMinutes.ToString("N2")
                );
        }

        private void DrawContent(Rect contentRect)
        {
            var listing = new Listing_Standard(GameFont.Small);
            listing.Begin(contentRect);

            Rect usernameRect = listing.GetRect(Text.LineHeight);
            (Rect usernameLabel, Rect usernameFieldHalf) =
                usernameRect.ToForm(assignedToTextWidth / usernameRect.width);
            SettingsHelper.DrawLabel(usernameLabel, assignedToText);

            usernameFieldPosition = new Rect(
                usernameFieldHalf.x,
                usernameFieldHalf.y,
                usernameFieldHalf.width - applyTextWidth - 5f,
                usernameFieldHalf.height
            );
            username = Widgets.TextField(usernameFieldPosition, username).ToToolkit();

            if (username.Length > 0 && SettingsHelper.DrawClearButton(usernameFieldPosition))
            {
                username = "";
                userFromButton = false;
                viewer = null;
                lastSeenText = null;
            }

            if (viewer != null && !viewer.username.EqualsIgnoreCase(username))
            {
                userFromButton = false;
                viewer = null;
                lastSeenText = null;
            }

            var usernameApply = new Rect(
                usernameFieldPosition.x + usernameFieldPosition.width + 5f,
                usernameFieldPosition.y,
                applyTextWidth,
                Text.LineHeight
            );

            if (Widgets.ButtonText(usernameApply, applyText))
            {
                AssignColonist();
            }

            if (!userFromButton || viewer == null)
            {
                listing.End();
                return;
            }

            listing.GapLine(36f);
            Rect seenAtLine = listing.GetRect(Text.LineHeight * 2f);
            seenAtLine = seenAtLine.WithWidth(seenAtLine.width - 5f - notifyTextWidth);
            var notifyButton = new Rect(
                seenAtLine.x + seenAtLine.width + 5f,
                seenAtLine.y,
                notifyTextWidth,
                Mathf.FloorToInt(seenAtLine.height / 2f)
            );
            SettingsHelper.DrawLabel(seenAtLine, lastSeenText);

            if (Widgets.ButtonText(notifyButton, notifyText))
            {
                MessageHelper.ReplyToUser(viewer.username, viewerDrawnText, true);
            }

            listing.End();
        }

        private void DrawNameQueue(Listing listing)
        {
            for (var index = 0; index < pawnComponent.ViewerNameQueue.Count; index++)
            {
                string name = pawnComponent.ViewerNameQueue[index];

                Rect line = listing.GetRect(Text.LineHeight);
                var buttonRect = new Rect(
                    line.x + line.width - line.height * 3f,
                    line.y,
                    line.height * 3f,
                    line.height
                );
                var nameRect = new Rect(line.x, line.y, line.width - buttonRect.width - 5f, line.height);

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(line);
                }

                if (name.EqualsIgnoreCase(username))
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
                username = name.ToLowerInvariant();
            }

            if (Widgets.ButtonImage(buttonTemplateRect, Textures.Gear))
            {
                OpenViewerDetailsFor(name);
            }

            buttonTemplateRect = buttonTemplateRect.ShiftRight(0f);

            var remove = false;
            if (Widgets.ButtonImage(buttonTemplateRect, Textures.Hammer))
            {
                Viewers.GetViewer(name).BanViewer();
                remove = true;
            }

            buttonTemplateRect = buttonTemplateRect.ShiftRight(0f);
            if (Widgets.ButtonImage(buttonTemplateRect, Widgets.CheckboxOffTex))
            {
                remove = true;
            }

            if (remove)
            {
                try
                {
                    pawnComponent.ViewerNameQueue.RemoveAt(index);
                }
                catch (IndexOutOfRangeException) { }
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
            Rect previousRect = new Rect(
                5f,
                5f + colonistRect.height / 2 - arrowMax / 2f,
                arrowMax,
                arrowMax
            ).Rounded();
            var nextRect = new Rect(
                canvas.width - arrowMax - 5f,
                previousRect.y,
                previousRect.width,
                previousRect.height
            );
            var nameRect = new Rect(5f, colonistRect.y + colonistRect.height + 2f, canvas.width - 10f, Text.LineHeight);
            var viewerRect = new Rect(5f, nameRect.y + nameRect.height + 2f, canvas.width - 10f, Text.LineHeight);
            var stateRect = new Rect(5f, viewerRect.y + viewerRect.height + 2f, canvas.width - 10f, Text.LineHeight);

            Widgets.DrawMenuSection(canvas);
            TooltipHandler.TipRegion(nextRect, nextTooltip);
            TooltipHandler.TipRegion(previousRect, previousTooltip);
            TooltipHandler.TipRegion(colonistRect, pawnTooltip);

            if (allPawns.Count > 1 && Widgets.ButtonImage(previousRect, TexUI.ArrowTexLeft))
            {
                GoToPreviousPawn();
            }

            if (allPawns.Count > 1 && Widgets.ButtonImage(nextRect, TexUI.ArrowTexRight))
            {
                GoToNextPawn();
            }

            if (current == null)
            {
                return;
            }

            GUI.DrawTexture(
                colonistRect,
                PortraitsCache.Get(
                    current,
                    ColonistBarColonistDrawer.PawnTextureSize,
                    ColonistBarColonistDrawer.PawnTextureCameraOffset,
                    1.28205f
                )
            );
            Widgets.DrawHighlightIfMouseover(colonistRect);

            if (Widgets.ButtonInvisible(colonistRect))
            {
                OpenInfoDialog();
            }

            if (!(current.Name is NameTriple name))
            {
                return;
            }

            bool viewerAssigned = pawnComponent.pawnHistory.Any(
                pair => pair.Key.Equals(name.Nick, StringComparison.InvariantCultureIgnoreCase) && pair.Value == current
            );
            SettingsHelper.DrawFittedLabel(nameRect, $"{name.First} {name.Last}", TextAnchor.MiddleCenter);
            SettingsHelper.DrawFittedLabel(viewerRect, name.Nick?.CapitalizeFirst(), TextAnchor.MiddleCenter);

            DoStateMouseActions(stateRect, viewerAssigned, name.Nick);
            TooltipHandler.TipRegion(viewerRect, viewerTooltip);
            Widgets.DrawHighlightIfMouseover(viewerRect);

            if (!name.Nick.NullOrEmpty() && Widgets.ButtonInvisible(viewerRect))
            {
                username = name.Nick;
            }
        }

        private void OpenInfoDialog()
        {
            var infoCard = new Dialog_InfoCard(current);
        #if RW12
            infoCard.SetTab(Dialog_InfoCard.InfoCardTab.Character);
        #endif
            Find.WindowStack.Add(infoCard);
        }

        private void DoStateMouseActions(Rect canvas, bool assigned, string viewerName)
        {
            if (current == null || viewerName.NullOrEmpty())
            {
                return;
            }

            SettingsHelper.DrawFittedLabel(
                canvas,
                (assigned ? assignedText : unassignedText).CapitalizeFirst(),
                TextAnchor.MiddleCenter
            );
            Widgets.DrawHighlightIfMouseover(canvas);
            TooltipHandler.TipRegion(canvas, assigned ? assignedTooltip : unassignedTooltip);

            if (!Widgets.ButtonInvisible(canvas))
            {
                return;
            }

            if (assigned)
            {
                pawnComponent.pawnHistory.Remove(viewerName.ToLowerInvariant());

                var pawnName = current.Name as NameTriple;
                current.Name = new NameTriple(pawnName?.First, pawnName?.Last, pawnName?.Last);
            }
            else
            {
                pawnComponent.pawnHistory.Add(viewerName.ToLowerInvariant(), current);
            }
        }

        private void GoToNextPawn()
        {
            int index = allPawns.IndexOf(current);

            current = allPawns[index == allPawns.Count - 1 ? 0 : index + 1];
            Notify__CurrentPawnChanged();
        }

        private void GoToPreviousPawn()
        {
            int index = Mathf.Max(allPawns.IndexOf(current), 0);

            current = allPawns[index == 0 ? allPawns.Count - 1 : index - 1];
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
                case KeyCode.F2 when current != null:
                    OpenInfoDialog();
                    Event.current.Use();
                    break;
            }
        }

        private void Notify__CurrentPawnChanged()
        {
            string assigned = pawnComponent.pawnHistory.Where(p => p.Value == current)
               .Select(p => p.Key)
               .FirstOrDefault();

            username = assigned ?? "";
        }

        private void AssignColonist()
        {
            if (pawnComponent.HasPawnBeenNamed(current))
            {
                string key = pawnComponent.pawnHistory.Where(p => p.Value == current)
                   .Select(p => p.Key)
                   .FirstOrDefault();

                if (key != null)
                {
                    pawnComponent.pawnHistory.Remove(key);
                }
            }

            var name = current.Name as NameTriple;

            current.Name = new NameTriple(name?.First, username.NullOrEmpty() ? name?.Last : username, name?.Last);

            if (!username.NullOrEmpty())
            {
                pawnComponent.AssignUserToPawn(username.ToLowerInvariant(), current);
            }

            NextUnnamedColonist();
        }

        private void NextUnnamedColonist()
        {
            current = allPawns.Where(p => CompatRegistry.Magic?.IsUndead(p) != true)
               .FirstOrDefault(p => pawnComponent.pawnHistory.All(pair => pair.Value != p) && p != current);

            if (current == null)
            {
                GoToNextPawn();
            }

            Notify__CurrentPawnChanged();
        }

        private void PreviousUnnamedColonist()
        {
            current = allPawns.Where(p => !CompatRegistry.Magic?.IsUndead(p) ?? false)
               .LastOrDefault(p => pawnComponent.pawnHistory.All(pair => pair.Value != p) && p != current);

            if (current == null)
            {
                GoToPreviousPawn();
            }

            Notify__CurrentPawnChanged();
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();

            int startup = Mathf.FloorToInt(Time.time);
            if (timestamp >= startup || startup % 3 >= 1)
            {
                return;
            }

            currentDiceSide = Textures.DiceSides.RandomElement();
            timestamp = startup;

            if (viewer == null)
            {
                return;
            }

            UpdateLastSeenText();
        }

        public override void PreOpen()
        {
            base.PreOpen();

            if (pawnComponent != null)
            {
                ReconnectViewers();
                return;
            }

            LogHelper.Warn("Pawn game component was null!");
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

                Viewer tempViewer = Viewers.All.FirstOrDefault(
                    v => v.username.Equals(pawnName.Nick, StringComparison.InvariantCultureIgnoreCase)
                );

                if (tempViewer == null)
                {
                    string assignment = pawnComponent.UserAssignedToPawn(pawn);

                    if (assignment != null)
                    {
                        pawnComponent.pawnHistory.Remove(assignment);
                        pawn.Name = new NameTriple(pawnName.First, pawnName.Last, pawnName.Last);
                    }

                    continue;
                }

                pawnComponent.pawnHistory[tempViewer.username] = pawn;
                pawnComponent.viewerNameQueue.RemoveAll(
                    u => u.Equals(tempViewer.username, StringComparison.InvariantCultureIgnoreCase)
                );
            }
        }

        public override void OnAcceptKeyPressed()
        {
            if (GUIUtility.keyboardControl == GUIUtility.GetControlID(FocusType.Keyboard, usernameFieldPosition))
            {
                AssignColonist();
                Event.current.Use();
                GUIUtility.keyboardControl = 0;
                return;
            }

            base.OnAcceptKeyPressed();
        }

        public override void OnCancelKeyPressed()
        {
            if (GUIUtility.keyboardControl == GUIUtility.GetControlID(FocusType.Keyboard, usernameFieldPosition))
            {
                Event.current.Use();
                GUIUtility.keyboardControl = 0;
                return;
            }

            base.OnCancelKeyPressed();
        }
    }
}
