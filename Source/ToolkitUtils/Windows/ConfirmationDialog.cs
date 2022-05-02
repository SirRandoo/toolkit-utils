// ToolkitUtils
// Copyright (C) 2022  SirRandoo
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
using JetBrains.Annotations;
using SirRandoo.CommonLib.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    /// <summary>
    ///     A dialog for prompting users for actions that require
    ///     confirmation.
    /// </summary>
    public class ConfirmationDialog : Window
    {
        private readonly Action _cancelAction;
        private readonly Action _confirmAction;
        private readonly string _prompt;
        private string _cancelText;
        private string _confirmText;

        public ConfirmationDialog(string title, string prompt, Action onConfirm, [CanBeNull] Action onCancel = null) : this(prompt, onConfirm, onCancel)
        {
            optionalTitle = title;
        }

        public ConfirmationDialog(string prompt, Action onConfirm, [CanBeNull] Action onCancel = null)
        {
            doCloseX = false;
            doCloseButton = false;
            closeOnClickedOutside = false;
            closeOnCancel = false;
            closeOnAccept = false;
            absorbInputAroundWindow = true;
            focusWhenOpened = true;
            forcePause = true;
            layer = WindowLayer.Dialog;

            _prompt = prompt;
            _confirmAction = onConfirm;
            _cancelAction = onCancel;
        }

        /// <inheritdoc cref="Window.InitialSize"/>
        public override Vector2 InitialSize => new Vector2(300, Text.CalcHeight(_prompt, 264f) + 102f);

        /// <inheritdoc cref="Window.PostOpen"/>
        public override void PostOpen()
        {
            _confirmText = "TKUtils.Buttons.Confirm".TranslateSimple();
            _cancelText = "TKUtils.Buttons.Cancel".TranslateSimple();

            base.PostOpen();
        }

        /// <inheritdoc cref="Window.DoWindowContents"/>
        public override void DoWindowContents(Rect region)
        {
            GUI.BeginGroup(region);

            int buttonHeight = Mathf.CeilToInt(Text.LineHeight * 1.5f);
            var promptRect = new Rect(0f, 0f, region.width, region.height - buttonHeight - 5f);
            var buttonGroupRect = new Rect(0f, promptRect.height + 5f, region.width, buttonHeight);

            GUI.BeginGroup(promptRect);
            DrawPrompt(promptRect);
            GUI.EndGroup();

            GUI.BeginGroup(buttonGroupRect);
            DrawButtons(buttonGroupRect.AtZero());
            GUI.EndGroup();

            GUI.EndGroup();
        }

        private void DrawPrompt(Rect region)
        {
            UiHelper.Label(region, _prompt, TextAnchor.MiddleCenter);
        }

        private void DrawButtons(Rect region)
        {
            var confirmRect = new Rect(0f, 0f, Mathf.FloorToInt(region.width / 2f) - 2f, region.height);
            var cancelRect = new Rect(confirmRect.x + confirmRect.width + 4f, 0f, confirmRect.width, region.height);

            if (Widgets.ButtonText(confirmRect, _confirmText))
            {
                _confirmAction?.Invoke();
                Close();
            }

            if (Widgets.ButtonText(cancelRect, _cancelText))
            {
                _cancelAction?.Invoke();
                Close();
            }
        }

        /// <summary>
        ///     Opens a new confirmation dialog with the given actions.
        /// </summary>
        /// <param name="title">The title of the dialog</param>
        /// <param name="prompt">The body of the dialog</param>
        /// <param name="onConfirm">
        ///     The action to do when the user confirms their
        ///     decision
        /// </param>
        /// <param name="onCancel">
        ///     The action to do when the user abandons their
        ///     decision
        /// </param>
        public static void Open(string title, string prompt, Action onConfirm, [CanBeNull] Action onCancel = null)
        {
            Find.WindowStack.Add(new ConfirmationDialog(title, prompt, onConfirm, onCancel));
        }

        /// <summary>
        ///     Opens a new confirmation dialog with the given actions.
        /// </summary>
        /// <param name="prompt">The body of the dialog</param>
        /// <param name="onConfirm">
        ///     The action to do when the user confirms their
        ///     decision
        /// </param>
        /// <param name="onCancel">
        ///     The action to do when the user abandons their
        ///     decision
        /// </param>
        public static void Open(string prompt, Action onConfirm, [CanBeNull] Action onCancel = null)
        {
            Find.WindowStack.Add(new ConfirmationDialog(prompt, onConfirm, onCancel));
        }
    }
}
