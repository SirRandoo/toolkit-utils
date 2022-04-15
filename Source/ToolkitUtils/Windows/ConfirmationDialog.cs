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
using JetBrains.Annotations;
using SirRandoo.CommonLib.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class ConfirmationDialog : Window
    {
        private string _confirmText;
        private string _cancelText;
        private readonly string _prompt;
        private readonly Action _confirmAction;
        private readonly Action _cancelAction;

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

        public override void PostOpen()
        {
            _confirmText = "TKUtils.Buttons.Confirm".TranslateSimple();
            _cancelText = "TKUtils.Buttons.Cancel".TranslateSimple();
        
            base.PostOpen();
        }

        public override Vector2 InitialSize => new Vector2(300, Text.CalcHeight(_prompt, 264f) + 102f);

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

        public static void Open(string title, string prompt, Action onConfirm, [CanBeNull] Action onCancel = null)
        {
            Find.WindowStack.Add(new ConfirmationDialog(title, prompt, onConfirm, onCancel));
        }

        public static void Open(string prompt, Action onConfirm, [CanBeNull] Action onCancel = null)
        {
            Find.WindowStack.Add(new ConfirmationDialog(prompt, onConfirm, onCancel));
        }
    }
}
