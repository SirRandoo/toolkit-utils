// MIT License
// 
// Copyright (c) 2023 SirRandoo
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
using SirRandoo.CommonLib.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Verse;
using Text = Verse.Text;

namespace ToolkitUtils.UX
{
    public class DropdownMenu : Window
    {
        private readonly List<DropdownEntry> _entries = new List<DropdownEntry>();
        private readonly Rect _parentRegion;
        private float _biggestHeight;
        private float _biggestWidth;
        private int _entryCount;
        private Vector2 _scrollPosition = new Vector2(0f, 0f);

        public DropdownMenu(Rect parentRegion)
        {
            _parentRegion = parentRegion;
        }

        /// <inheritdoc/>
        protected override float Margin => 0f;

        /// <inheritdoc/>
        protected override void SetInitialSizeAndPosition()
        {
            float halfBiggestWidth = _biggestWidth / 2f;
            Vector2 parentCenter = _parentRegion.center;

            float x = parentCenter.x - halfBiggestWidth;
            float y = _parentRegion.y + _parentRegion.height + 1f;
            float height = Mathf.FloorToInt((UI.screenHeight - y) / _biggestHeight);

            windowRect = new Rect(x, y, _biggestWidth, height);
        }

        public void AddOption(string id, [NotNull] string label, Action callback)
        {
            var entry = new DropdownEntry(id, label, callback);
            
            _entries.Add(entry);
            _entryCount++;

            if (entry.RequiredHeight > _biggestHeight)
            {
                _biggestHeight = entry.RequiredHeight;
            }

            if (entry.RequiredWidth > _biggestWidth)
            {
                _biggestWidth = entry.RequiredWidth;
            }
        }

        public bool RemoveOption(string id)
        {
            var widest = 0f;
            var tallest = 0f;
            int position = -1;

            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                DropdownEntry entry = _entries[i];

                if (string.Equals(entry.Id, id, StringComparison.Ordinal))
                {
                    position = i;
                    continue;
                }

                if (widest < entry.RequiredWidth)
                {
                    widest = entry.RequiredWidth;
                }

                if (tallest < entry.RequiredHeight)
                {
                    tallest = entry.RequiredHeight;
                }
            }

            if (position == -1)
            {
                return false;
            }

            _entryCount--;
            _entries.RemoveAt(position);

            return true;
        }

        /// <inheritdoc/>
        public override void DoWindowContents(Rect inRect)
        {
            var scrollView = new Rect(0f, 0f, inRect.width - 16f, _biggestHeight * _entryCount);
            _scrollPosition = GUI.BeginScrollView(inRect, _scrollPosition, scrollView);
            
            for (var index = 0; index < _entries.Count; index++)
            {
                var region = new Rect(0f, _biggestHeight * index, _biggestWidth, _biggestHeight);

                if (!region.IsVisible(inRect, _scrollPosition))
                {
                    continue;
                }
                
                DropdownEntry entry = _entries[index];

                Widgets.DrawAtlas(region, TexUI.FloatMenuOptionBG);
                UiHelper.Label(region, entry.Label);

                if (!Widgets.ButtonInvisible(region))
                {
                    continue;
                }

                entry.OnClick.Invoke();
                Close();
            }
            
            GUI.EndScrollView();
        }

        private sealed class DropdownEntry
        {
            public DropdownEntry(string id, string label, Action onClick)
            {
                Id = id;
                Label = label;
                OnClick = onClick;

                GameFont cache = Text.Font;
                Text.Font = GameFont.Small;

                Vector2 dimensions = Text.CalcSize(label);
                RequiredWidth = dimensions.x;
                RequiredHeight = dimensions.y;
                
                Text.Font = cache;
            }
            
            public string Id { get; }
            public string Label { get; }
            public Action OnClick { get; }

            public float RequiredHeight { get; }
            public float RequiredWidth { get; }
        }
    }
}
