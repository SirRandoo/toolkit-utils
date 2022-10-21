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
    public class IntegerEntryDialog : Window
    {
        private int _tmpValue;
        private string _buffer = "0";
        private bool _bufferValid;

        public IntegerEntryDialog([CanBeNull] string title)
        {
            optionalTitle = title;
            
            doCloseButton = false;
            doCloseX = false;
            closeOnClickedOutside = false;
            closeOnCancel = false;
            closeOnAccept = false;
        }
        
        public Action<int> OnAccept { get; set; }
        public Action OnCancel { get; set; }
        public int Maximum { get; set; } = int.MaxValue;
        public int Minimum { get; set; }

        /// <inheritdoc/>
        public override void DoWindowContents(Rect inRect)
        {
            var fieldRegion = new Rect(0f, 0f, inRect.width, Text.LineHeight);
            var actionsRegion = new Rect(0f, Text.LineHeight, inRect.width, Text.LineHeight);

            GUI.BeginGroup(inRect);

            GUI.BeginGroup(fieldRegion);

            if (UiHelper.NumberField(fieldRegion, out int newValue, ref _buffer, ref _bufferValid, Minimum, Maximum))
            {
                _tmpValue = newValue;
            }
            
            GUI.EndGroup();

            GUI.BeginGroup(actionsRegion);
            DrawActions(actionsRegion.AtZero());
            GUI.EndGroup();
            
            GUI.EndGroup();
        }

        private void DrawActions(Rect region)
        {
            var applyRegion = new Rect(0f, 0f, Mathf.FloorToInt(region.width * 0.5f) - 10f, region.height);
            var cancelRegion = new Rect(applyRegion.width + 20f, 0f, applyRegion.width, region.height);

            if (Widgets.ButtonText(applyRegion, "Apply"))
            {
                OnAccept?.Invoke(_tmpValue);
                
                Close();
            }

            if (Widgets.ButtonText(cancelRegion, "Cancel"))
            {
                OnCancel?.Invoke();
                
                Close();
            }
        }

        /// <inheritdoc />
        public override void OnAcceptKeyPressed()
        {
            OnAccept?.Invoke(_tmpValue);

            Close();
        }

        /// <inheritdoc />
        public override void OnCancelKeyPressed()
        {
            if (!string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()))
            {
                GUI.FocusControl(null);
            }
            else
            {
                OnCancel?.Invoke();
            
                Close();
            }
        }
    }
}
