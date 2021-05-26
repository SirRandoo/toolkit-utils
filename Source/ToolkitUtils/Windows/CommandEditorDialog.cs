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
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils.Windows
{
    [StaticConstructorOnStartup]
    public class CommandEditorDialog : Window_CommandEditor
    {
        private static readonly MethodInfo RemoveMethod;

        private readonly Command command;
        private readonly ICommandSettings settings;
        private string adminText;
        private string anyoneText;
        private string commandLabel;
        private bool confirmed;

        private bool confirming;
        private string confirmText;
        private float confirmTextWidth;
        private string deletedText;
        private float deletedTextWidth;
        private string deleteText;
        private float deleteTextWidth;
        private string disableText;
        private float disableTextWidth;
        private TextEditor editor;
        private Rect editorPosition;
        private string enableText;
        private float enableTextWidth;
        private string headerText;
        private bool invalidId;
        private string moderatorText;
        private Vector2 scrollPos = Vector2.zero;
        private string settingsText;
        private float settingsTextWidth;
        private bool showingSettings;
        private string tagTooltip;
        private List<FloatMenuOption> userLevelOptions;
        private string userLevelText;

        static CommandEditorDialog()
        {
            RemoveMethod = AccessTools.Method(typeof(DefDatabase<Command>), "Remove");
        }

        public CommandEditorDialog([NotNull] Command command) : base(command)
        {
            this.command = command;

            var ext = command.GetModExtension<CommandExtension>();

            if (ext?.SettingsHandler != null)
            {
                settings = Activator.CreateInstance(ext.SettingsHandler) as ICommandSettings;
            }
        }

        public override void PostOpen()
        {
            base.PostOpen();

            GetTranslations();
            userLevelOptions ??= new List<FloatMenuOption>
            {
                new FloatMenuOption(anyoneText, () => ChangeUserLevel(UserLevel.Anyone)),
                new FloatMenuOption(moderatorText, () => ChangeUserLevel(UserLevel.Moderator)),
                new FloatMenuOption(adminText, () => ChangeUserLevel(UserLevel.Admin))
            };
            invalidId = command.command.NullOrEmpty()
                        || command.command?.TrimStart(TkSettings.Prefix.ToCharArray()).NullOrEmpty() == true;
        }

        public override void DoWindowContents(Rect region)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            GUI.BeginGroup(region);

            if (!showingSettings)
            {
                var buttonBar = new Rect(0f, 0f, region.width, Text.SmallFontHeight);
                Rect content = new Rect(
                    0f,
                    Text.SmallFontHeight * 2,
                    region.width,
                    region.height - Text.SmallFontHeight
                ).ContractedBy(20f);

                GUI.BeginGroup(buttonBar);
                DrawButtonBar(buttonBar.AtZero());
                GUI.EndGroup();

                GUI.BeginGroup(content);
                DrawContent(content.AtZero());
                GUI.EndGroup();
            }
            else
            {
                settings?.Draw(region);
            }

            GUI.EndGroup();
        }

        private void DrawContent(Rect region)
        {
            var listing = new Listing_Standard();

            GUI.BeginGroup(region);
            listing.Begin(region);

            (Rect labelRect, Rect fieldRect) = listing.GetRectAsForm(0.6f);
            SettingsHelper.DrawLabel(labelRect, commandLabel);

            GUI.color = invalidId ? new Color(1f, 0.53f, 0.76f) : Color.white;
            if (SettingsHelper.DrawTextField(fieldRect, $"{TkSettings.Prefix}{command.command}", out string newContent))
            {
                if (newContent.ToToolkit().Length - TkSettings.Prefix.Length < 0)
                {
                    invalidId = true;
                }
                else
                {
                    command.command = newContent.Substring(TkSettings.Prefix.Length).ToToolkit();
                    invalidId = false;
                }
            }

            GUI.color = Color.white;

            if (command.isCustomMessage)
            {
                DrawCustomFields(listing);
            }

            listing.End();
            GUI.EndGroup();
        }

        private void DrawCustomFields([NotNull] Listing listing)
        {
            (Rect levelLabel, Rect levelField) = listing.GetRectAsForm(0.6f);
            SettingsHelper.DrawLabel(levelLabel, userLevelText);

            if (Widgets.ButtonText(levelField, GetInferredUserLevelText()))
            {
                Find.WindowStack.Add(new FloatMenu(userLevelOptions));
            }

            listing.Gap(24f);
            if (SettingsHelper.DrawFieldButton(
                listing.GetRect(Text.SmallFontHeight),
                Textures.QuestionMark,
                tagTooltip
            ))
            {
                Application.OpenURL("https://storytoolkit.fandom.com/wiki/Commands#Tags");
            }

            editorPosition = listing.GetRect(Text.SmallFontHeight * 11f);

            GUI.BeginGroup(editorPosition);
            command.outputMessage = Widgets.TextAreaScrollable(
                editorPosition.AtZero(),
                command.outputMessage,
                ref scrollPos
            );
            editor ??= GUIUtility.GetStateObject(
                typeof(TextEditor),
                GUIUtility.GetControlID(FocusType.Keyboard, editorPosition)
            ) as TextEditor;
            GUI.EndGroup();
        }

        public override void OnAcceptKeyPressed()
        {
            if (GUIUtility.keyboardControl <= 0 || editor == null)
            {
                base.OnAcceptKeyPressed();
                return;
            }

            command.outputMessage += "\n";
            editor.MoveDown();
            Event.current.Use();
        }

        public override void OnCancelKeyPressed()
        {
            if (GUIUtility.keyboardControl <= 0 || editor == null)
            {
                base.OnCancelKeyPressed();
                return;
            }

            GUIUtility.keyboardControl = 0;
            Event.current.Use();
        }

        public override void Close(bool doCloseSound = true)
        {
            if (showingSettings)
            {
                showingSettings = false;
                return;
            }

            base.Close(doCloseSound);
        }

        public override void PostClose()
        {
            base.PostClose();

            if (TkSettings.Offload)
            {
                Task.Run(async () => await Data.DumpCommandsAsync()).ConfigureAwait(false);
            }
            else
            {
                Data.DumpCommands();
            }
        }

        private void DrawButtonBar(Rect region)
        {
            float width = Mathf.Max(
                deleteTextWidth,
                confirmTextWidth,
                enableTextWidth,
                disableTextWidth,
                deletedTextWidth,
                settingsTextWidth
            );

            var buttonRect = new Rect(region.x + region.width - width, region.y, width, Text.SmallFontHeight);

            if (command.isCustomMessage)
            {
                DrawCustomCommandButtons(buttonRect);
                buttonRect = buttonRect.ShiftLeft(0f);
            }

            if (Widgets.ButtonText(buttonRect, command.enabled ? disableText : enableText))
            {
                command.enabled = !command.enabled;
            }

            if (settings != null)
            {
                buttonRect = buttonRect.ShiftLeft(0f);

                if (Widgets.ButtonText(buttonRect, settingsText))
                {
                    GUIUtility.keyboardControl = 0;
                    showingSettings = true;
                }
            }

            var headerRect = new Rect(0f, 0f, region.width - buttonRect.width * 2 - 5f, Text.SmallFontHeight);
            SettingsHelper.DrawFittedLabel(headerRect, headerText);
        }

        private void DrawCustomCommandButtons(Rect buttonRect)
        {
            if (!confirmed && Widgets.ButtonText(buttonRect, confirming ? confirmText : deleteText))
            {
                confirming = !confirming;
                confirmed = confirming == false;

                if (confirmed)
                {
                    ToolkitSettings.CustomCommandDefs.Remove(command.defName);
                    RemoveMethod.Invoke(typeof(DefDatabase<Command>), new object[] {command});
                }
            }

            if (confirmed)
            {
                SettingsHelper.DrawColoredLabel(buttonRect, deletedText, new Color(1f, 0.53f, 0.76f));
            }
        }

        private void ChangeUserLevel(UserLevel level)
        {
            command.requiresAdmin = level == UserLevel.Admin;
            command.requiresMod = level == UserLevel.Moderator;
        }

        private string GetInferredUserLevelText()
        {
            if (command.requiresAdmin)
            {
                return adminText;
            }

            return command.requiresMod ? moderatorText : anyoneText;
        }

        private void GetTranslations()
        {
            headerText =
                "TKUtils.CommandEditor.Header".LocalizeKeyed((command.label ?? command.defName).CapitalizeFirst());
            commandLabel = "TKUtils.Fields.Command".Localize();
            deleteText = "TKUtils.Buttons.Delete".Localize();
            enableText = "TKUtils.Buttons.Enable".Localize();
            disableText = "TKUtils.Buttons.Disable".Localize();
            confirmText = "TKUtils.Buttons.AreYouSure".Localize();
            deletedText = "TKUtils.Headers.RestartRequired".Localize();
            anyoneText = "TKUtils.CommandEditor.UserLevel.Anyone".Localize();
            moderatorText = "TKUtils.CommandEditor.UserLevel.Moderator".Localize();
            adminText = "TKUtils.CommandEditor.UserLevel.Admin".Localize();
            userLevelText = "TKUtils.Fields.UserLevel".Localize();
            tagTooltip = "TKUtils.CommandEditorTooltips.Tags".Localize();
            settingsText = "TKUtils.Buttons.Settings".Localize();

            GameFont cache = Text.Font;
            Text.Font = GameFont.Small;
            deleteTextWidth = Text.CalcSize(deleteText).x;
            confirmTextWidth = Text.CalcSize(confirmText).x;
            enableTextWidth = Text.CalcSize(enableText).x;
            disableTextWidth = Text.CalcSize(disableText).x;
            deletedTextWidth = Text.CalcSize(deletedText).x;
            settingsTextWidth = Text.CalcSize(settingsText).x;
            Text.Font = cache;
        }

        private enum UserLevel { Anyone, Moderator, Admin }
    }
}
