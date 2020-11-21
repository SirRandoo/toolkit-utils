using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
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
        private string countText;
        private Pawn current;

        private Texture2D currentDiceSide;
        private string emptyQueueText;
        private string nextTooltip;
        private string pawnTooltip;
        private string previousTooltip;
        private string randomTooltip;
        private Vector2 scrollPos = Vector2.zero;
        private float timestamp;

        private string unassignedText;
        private string unassignedTooltip;
        private string username;
        private Rect usernameFieldPosition;
        private string usernameText;
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

        public override Vector2 InitialSize => new Vector2(550f, 500f);

        private void GetTranslations()
        {
            emptyQueueText = "TKUtils.NameQueue.None".Localize();
            assignedText = "TKUtils.NameQueue.Assigned".Localize();
            unassignedText = "TKUtils.NameQueue.Unassigned".Localize();
            applyText = "TKUtils.Buttons.Apply".Localize();
            usernameText = "TKUtils.Inputs.Username".Localize();
            countText = "TKUtils.NameQueue.Count".Localize();
            viewerTooltip = "TKUtils.NameQueue.Tooltips.ViewerName".Localize();
            unassignedTooltip = "TKUtils.NameQueue.Tooltips.Unassigned".Localize();
            assignedTooltip = "TKUtils.NameQueue.Tooltips.Assigned".Localize();
            nextTooltip = "TKUtils.NameQueue.Tooltips.Next".Localize();
            previousTooltip = "TKUtils.NameQueue.Tooltips.Previous".Localize();
            pawnTooltip = "TKUtils.NameQueue.Tooltips.Pawn".Localize();
            randomTooltip = "TKUtils.NameQueue.Tooltips.Random".Localize();

            applyTextWidth = Text.CalcSize(applyText).x + 16f;
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            Text.Font = GameFont.Small;

            ProcessShortcutKeys();
            Rect pawnRect = new Rect(0f, 0f, inRect.width * 0.3333f, 152f + Text.LineHeight).Rounded();
            var contentRect = new Rect(pawnRect.width + 10f, 0f, inRect.width - 15f - pawnRect.width, inRect.height);

            GUI.BeginGroup(pawnRect);
            DrawPawnSection(pawnRect);
            GUI.EndGroup();


            GUI.BeginGroup(contentRect);
            var usernameRect = new Rect(0f, 0f, contentRect.width, Text.LineHeight);
            Widgets.Label(usernameRect.LeftHalf(), usernameText);

            Rect usernameFieldHalf = usernameRect.RightHalf();
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


            var listing = new Listing_Standard();
            Rect adjustedLineRect = new Rect(0f, Text.LineHeight * 4f, contentRect.width * 0.95f - 2f, Text.LineHeight)
               .Rounded();
            var queueNoticeRect = new Rect(
                0f,
                Text.LineHeight * 5f,
                adjustedLineRect.width - Text.LineHeight - 5f,
                Text.LineHeight
            );
            var queueRandomRect = new Rect(
                adjustedLineRect.x + adjustedLineRect.width - Text.LineHeight,
                Text.LineHeight * 5f,
                Text.LineHeight,
                Text.LineHeight
            );
            var queueRect = new Rect(
                0f,
                Text.LineHeight * 6f,
                contentRect.width,
                contentRect.height - Text.LineHeight * 6f
            );
            var queueInnerRect = new Rect(0f, 0f, queueRect.width, queueRect.height);
            var queueView = new Rect(
                0f,
                0f,
                queueRect.width - 16f,
                Text.LineHeight * pawnComponent.viewerNameQueue.Count
            );

            if (pawnComponent.ViewerNameQueue.Count <= 0)
            {
                Widgets.Label(queueNoticeRect, emptyQueueText);
                GUI.EndGroup();
                return;
            }

            Widgets.Label(queueNoticeRect, $"{pawnComponent.ViewerNameQueue.Count:N0} {countText}");
            TooltipHandler.TipRegion(queueRandomRect, randomTooltip);

            if (Widgets.ButtonImage(queueRandomRect, currentDiceSide))
            {
                username = pawnComponent.ViewerNameQueue.RandomElement();
            }

            GUI.BeginGroup(queueRect);
            listing.BeginScrollView(queueInnerRect, ref scrollPos, ref queueView);
            DrawNameQueue(listing);

            GUI.EndGroup();
            listing.EndScrollView(ref queueView);
            GUI.EndGroup();
            Text.Font = GameFont.Small;
        }

        private void DrawNameQueue(Listing listing)
        {
            for (var index = 0; index < pawnComponent.ViewerNameQueue.Count; index++)
            {
                string name = pawnComponent.ViewerNameQueue[index];

                Rect line = listing.GetRect(Text.LineHeight);
                var buttonRect = new Rect(line.x + line.width - line.height, line.y, line.height, line.height);
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
            Widgets.Label(nameRect, name);

            if (Widgets.ButtonInvisible(nameRect))
            {
                username = name.ToLowerInvariant();
            }

            if (!Widgets.ButtonImage(buttonRect, Widgets.CheckboxOffTex))
            {
                return;
            }

            try
            {
                pawnComponent.ViewerNameQueue.RemoveAt(index);
            }
            catch (IndexOutOfRangeException) { }
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
            infoCard.SetTab(Dialog_InfoCard.InfoCardTab.Character);
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
            current = allPawns.Where(p => !p.IsUndead())
               .FirstOrDefault(p => pawnComponent.pawnHistory.All(pair => pair.Value != p) && p != current);

            if (current == null)
            {
                GoToNextPawn();
            }

            Notify__CurrentPawnChanged();
        }

        private void PreviousUnnamedColonist()
        {
            current = allPawns.Where(p => !p.IsUndead())
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
            if (timestamp >= startup || startup % 5 >= 1)
            {
                return;
            }

            currentDiceSide = Textures.DiceSides.RandomElement();
            timestamp = startup;
        }

        public override void PreOpen()
        {
            base.PreOpen();

            if (pawnComponent != null)
            {
                ReconnectViewers();
                // TODO: Remove name queue dummy data
                pawnComponent.viewerNameQueue.AddRange(
                    new[]
                    {
                        "scavenging_mechanic",
                        "ericcode",
                        "crystalroseeve",
                        "bogrin",
                        "reishella",
                        "hodlhodl",
                        "dramravett",
                        "itanshi",
                        "winterbrass",
                        "nightbot",
                        "sasachi"
                    }
                );
                return;
            }

            TkLogger.Warn("Pawn game component was null!");
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

                Viewer viewer = Viewers.All.FirstOrDefault(
                    v => v.username.Equals(pawnName.Nick, StringComparison.InvariantCultureIgnoreCase)
                );

                if (viewer == null)
                {
                    string assignment = pawnComponent.UserAssignedToPawn(pawn);

                    if (assignment != null)
                    {
                        pawnComponent.pawnHistory.Remove(assignment);
                        pawn.Name = new NameTriple(pawnName.First, pawnName.Last, pawnName.Last);
                    }

                    continue;
                }

                pawnComponent.pawnHistory[viewer.username] = pawn;
                pawnComponent.viewerNameQueue.RemoveAll(
                    u => u.Equals(viewer.username, StringComparison.InvariantCultureIgnoreCase)
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
