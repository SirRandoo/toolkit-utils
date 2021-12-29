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
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class PromptDialog : Window
    {
        private Action cancelAction;
        private Action closeAction;
        private Action confirmAction;
        private readonly string message;
        private string confirmText;
        private string cancelText;


        public PromptDialog(string message, Action onConfirm, Action onCancel, [CanBeNull] Action onClose = null)
        {
            this.message = message;

            closeAction = onClose;
            cancelAction = onCancel;
            confirmAction = onConfirm;
        }
        public PromptDialog(string title, string message, Action onConfirm, Action onCancel, [CanBeNull] Action onClose = null) : this(message, onConfirm, onCancel, onClose)
        {
            optionalTitle = title;
        }

        public override void PostOpen()
        {
            confirmText = "TKUtils.Buttons.Confirm".Localize();
            cancelText = "TKUtils.Buttons.Cancel".Localize();
        }

        public override void DoWindowContents(Rect region)
        {
            float btnHeight = Mathf.CeilToInt(Text.SmallFontHeight * 1.25f);
            var messageRect = new Rect(0f, 0f, region.width, region.height - btnHeight);
            var buttonRect = new Rect(0f, region.height - btnHeight, region.width, btnHeight);

            GUI.BeginGroup(region);

            GUI.BeginGroup(messageRect);
            SettingsHelper.DrawLabel(messageRect, message, TextAnchor.MiddleCenter);
            GUI.EndGroup();

            GUI.BeginGroup(buttonRect);
            DrawButtons(buttonRect.AtZero());
            GUI.EndGroup();

            GUI.EndGroup();
        }

        private void DrawButtons(Rect region)
        {
            var templateRect = new Rect(region.width - Window.CloseButSize.x, 0f, Window.CloseButSize.x, region.height);

            if (Widgets.ButtonText(templateRect, cancelText))
            {
                cancelAction();
                Close();
            }

            if (!Widgets.ButtonText(templateRect.ShiftLeft(), confirmText))
            {
                return;
            }

            confirmAction();
            Close();
        }

        public override void PostClose()
        {
            base.PostClose();
            closeAction();
        }
    }
}
