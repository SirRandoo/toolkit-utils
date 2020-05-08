﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.PawnQueue;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class NameQueueDialog : Window
    {
        private readonly List<Pawn> allPawns;
        private readonly GameComponentPawns pawnComponent;
        private TaggedString applyText;
        private TaggedString assignedText;
        private Pawn current;
        private TaggedString emptyQueueText;
        private TaggedString removeText;
        private Vector2 scrollPos = Vector2.zero;

        private TaggedString titleText;
        private TaggedString unassignedText;
        private string username;
        private TaggedString usernameText;

        public NameQueueDialog()
        {
            GetTranslations();

            optionalTitle = titleText;

            doCloseX = true;
            forcePause = true;
            pawnComponent = Current.Game.GetComponent<GameComponentPawns>();

            allPawns = Find.ColonistBar.Entries
                .Select(e => e.pawn)
                .ToList();

            current = allPawns.FirstOrDefault(p => !pawnComponent.HasPawnBeenNamed(p)) ?? allPawns.FirstOrDefault();
            Notify__CurrentPawnChanged();
        }

        public override Vector2 InitialSize => new Vector2(550f, 500f);

        private void GetTranslations()
        {
            titleText = "TKUtils.Windows.NameQueue.Title".Translate();
            emptyQueueText = "TKUtils.Windows.NameQueue.None".Translate();
            assignedText = "TKUtils.Windows.NameQueue.Assigned".Translate();
            unassignedText = "TKUtils.Windows.NameQueue.Unassigned".Translate();
            applyText = "TKUtils.Windows.Config.Buttons.Apply.Label".Translate();
            removeText = "TKUtils.Windows.Config.Buttons.Remove.Label".Translate();
            usernameText = "TKUtils.Windows.Config.Input.Username.Label".Translate();
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            var menuRect = new Rect(0f, 0f, inRect.width * 0.3333f, inRect.height * .7f);
            var contentRect = new Rect(menuRect.width + 10f, 0f, inRect.width - 10f - menuRect.width, inRect.height);

            GUI.BeginGroup(menuRect);
            DrawMenuSection(menuRect);
            GUI.EndGroup();


            GUI.BeginGroup(contentRect);
            var usernameRect = new Rect(0f, 0f, contentRect.width, Text.LineHeight);
            Widgets.Label(usernameRect.LeftHalf(), usernameText);

            var buttonWidth = Text.CalcSize(applyText).x * 1.5f;

            var usernameFieldHalf = usernameRect.RightHalf();
            var usernameField = new Rect(
                usernameFieldHalf.x,
                usernameFieldHalf.y,
                usernameFieldHalf.width - buttonWidth - 5f,
                usernameFieldHalf.height
            );
            username = Widgets.TextField(usernameField, username);

            if (username.Length > 0 && SettingsHelper.DrawClearButton(usernameField))
            {
                username = "";
            }

            var usernameApply = new Rect(
                usernameField.x + usernameField.width + 5f,
                usernameField.y,
                buttonWidth,
                Text.LineHeight
            );

            if (Widgets.ButtonText(usernameApply, applyText))
            {
                AssignColonist();
            }


            var listing = new Listing_Standard {maxOneColumn = true};
            var queueNoticeRect = new Rect(
                0f,
                Text.LineHeight * 4f,
                contentRect.width,
                Text.LineHeight
            );
            var queueRect = new Rect(
                0f,
                Text.LineHeight * 5f,
                contentRect.width,
                contentRect.height - Text.LineHeight * 5f
            );
            var queueView = new Rect(
                0f,
                queueRect.y,
                queueRect.width * 0.9f,
                Text.LineHeight * pawnComponent.viewerNameQueue.Count + 40f
            );

            if (pawnComponent.ViewerNameQueue.Count <= 0)
            {
                Widgets.Label(queueNoticeRect, emptyQueueText);
                return;
            }

            Widgets.Label(
                queueNoticeRect,
                "TKUtils.Windows.NameQueue.Count".Translate(pawnComponent.ViewerNameQueue.Count)
            );

            listing.BeginScrollView(queueRect, ref scrollPos, ref queueView);
            listing.Gap(Text.LineHeight * 5f);
            var rmvBtnSize = Text.CalcSize(removeText).x * 1.5f;
            for (var index = pawnComponent.ViewerNameQueue.Count - 1; index >= 0; index--)
            {
                var name = pawnComponent.ViewerNameQueue[index];

                var line = listing.GetRect(Text.LineHeight);
                var buttonRect = new Rect(line.x + line.width - rmvBtnSize, line.y, rmvBtnSize, line.height);
                var nameRect = new Rect(line.x, line.y, line.width - buttonRect.width - 5f, line.height);

                if (name.EqualsIgnoreCase(username))
                {
                    Widgets.DrawHighlightSelected(nameRect);
                }
                else if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(nameRect);
                }

                Widgets.DrawHighlightIfMouseover(nameRect);
                Widgets.Label(nameRect, name);

                if (Widgets.ButtonInvisible(nameRect))
                {
                    username = name.ToLowerInvariant();
                }

                if (Widgets.ButtonText(buttonRect, removeText))
                {
                    try
                    {
                        pawnComponent.ViewerNameQueue.RemoveAt(index);
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }
                }
            }

            listing.EndScrollView(ref queueView);

            GUI.EndGroup();
            GUI.EndGroup();
        }

        private void DrawMenuSection(Rect canvas)
        {
            var arrowMax = Mathf.Max(TexUI.ArrowTexLeft.width, TexUI.ArrowTexRight.width) * 0.333f;
            var colonistRect = new Rect(
                arrowMax + 2f + 5f,
                5f,
                canvas.width - (arrowMax * 2f) - 4f - 10f,
                canvas.width - (arrowMax * 2f) - 4f - 10f
            );
            var previousRect = new Rect(5f, 5f + colonistRect.height / 2 - arrowMax / 2f, arrowMax, arrowMax);
            var nextRect = new Rect(
                colonistRect.x + colonistRect.width + 2f,
                previousRect.y,
                previousRect.width,
                previousRect.height
            );
            var nameRect = new Rect(5f, colonistRect.y + colonistRect.height + 2f, canvas.width - 10f, Text.LineHeight);
            var viewerRect = new Rect(5f, nameRect.y + nameRect.height + 2f, canvas.width - 10f, Text.LineHeight);

            Widgets.DrawMenuSection(canvas);

            if (allPawns.Count > 1 && Widgets.ButtonImage(previousRect, TexUI.ArrowTexLeft))
            {
                var index = Mathf.Max(allPawns.IndexOf(current), 0);

                current = allPawns[index == 0 ? allPawns.Count - 1 : index - 1];
                Notify__CurrentPawnChanged();
            }

            if (allPawns.Count > 1 && Widgets.ButtonImage(nextRect, TexUI.ArrowTexRight))
            {
                var index = allPawns.IndexOf(current);

                current = allPawns[index == allPawns.Count - 1 ? 0 : index + 1];
                Notify__CurrentPawnChanged();
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
                Find.WindowStack.Add(new Dialog_InfoCard(current));
            }

            var cache = Text.Anchor;
            var name = current.Name as NameTriple;

            Text.Anchor = TextAnchor.MiddleCenter;
            DrawTextFitted(nameRect, $"{name?.First} {name?.Last}");
            DrawTextFitted(
                viewerRect,
                pawnComponent.pawnHistory.All(p => !p.Key.EqualsIgnoreCase(name?.Nick ?? string.Empty))
                    ? $@"<color=""#ff8080"">{name?.Nick} ({unassignedText.RawText})</color>"
                    : $@"<color=""#80ff80"">{name?.Nick} ({assignedText.RawText})</color>"
            );
            Text.Anchor = cache;

            if (name == null)
            {
                return;
            }

            Widgets.DrawHighlightIfMouseover(viewerRect);

            if (!name.Nick.NullOrEmpty() && Widgets.ButtonInvisible(viewerRect))
            {
                username = name.Nick;
            }
        }

        private void Notify__CurrentPawnChanged()
        {
            var assigned = pawnComponent.pawnHistory
                .Where(p => p.Value == current)
                .Select(p => p.Key)
                .FirstOrDefault();

            username = assigned ?? "";
        }

        private static void DrawTextFitted(Rect region, string text)
        {
            var cache = Text.Font;
            var wrapCache = Text.WordWrap;
            var marker = (int) GameFont.Medium + 1;

            Text.WordWrap = false;

            while (true)
            {
                marker -= 1;

                if (marker == -1)
                {
                    break;
                }

                Text.Font = (GameFont) Enum.ToObject(typeof(GameFont), marker);

                var size = Text.CalcSize(text);

                if (size.x <= region.width && size.y <= region.height)
                {
                    break;
                }
            }

            Widgets.Label(region, text);
            Text.Font = cache;
            Text.WordWrap = wrapCache;
        }

        private void AssignColonist()
        {
            if (pawnComponent.HasPawnBeenNamed(current) && pawnComponent.pawnHistory.ContainsValue(current))
            {
                var key = pawnComponent.pawnHistory
                    .Where(p => p.Value == current)
                    .Select(p => p.Key)
                    .FirstOrDefault();

                if (key != null)
                {
                    pawnComponent.pawnHistory.Remove(key);
                }
            }

            var name = current.Name as NameTriple;

            current.Name = new NameTriple(name?.First, username, name?.Last);
            pawnComponent.AssignUserToPawn(username.ToLowerInvariant(), current);

            NextUnnamedColonist();
        }

        private void NextUnnamedColonist()
        {
            current = allPawns
                .FirstOrDefault(p => pawnComponent.pawnHistory.All(pair => pair.Value != p));

            Notify__CurrentPawnChanged();
        }

        public override void PreOpen()
        {
            base.PreOpen();

            if (pawnComponent != null)
            {
                pawnComponent.ViewerNameQueue.AddRange(
                    new[]
                    {
                        "nightbot",
                        "streamlabs",
                        "streamjar",
                        "streamelements",
                        "moobot",
                        "scavmechbot",
                        "shovelbot",
                        "scavenging_mechanic"
                    }
                );
                return;
            }

            TkLogger.Warn("Pawn game component was null!");
            Close();
        }
    }
}