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

using System.Collections.Generic;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit.Storytellers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class StorytellerPackDialog : Window
    {
        private readonly int _packCount;
        private readonly float _packLineSpan;
        private readonly List<PackEntry> _packs = new List<PackEntry>();
        private Vector2 _scrollPos = Vector2.zero;
        private PackEntry _selected;

        public StorytellerPackDialog()
        {
            foreach (StorytellerPack pack in DefDatabase<StorytellerPack>.AllDefs)
            {
                var extension = pack.GetModExtension<StorytellerPackExtension>();
                extension?.settingsInstance?.ResetState();

                _packs.Add(new PackEntry { Name = pack.LabelCap, Settings = extension?.settingsInstance });
            }

            _selected = _packs.FirstOrFallback();
            _packCount = _packs.Count;
            _packLineSpan = _packCount * Text.SmallFontHeight;
        }

        /// <inheritdoc/>
        public override void DoWindowContents(Rect inRect)
        {
            var listRegion = new Rect(0f, 0f, Mathf.FloorToInt(inRect.width * 0.39f), inRect.height - Text.SmallFontHeight - StandardMargin);
            var globalWeightsRegion = new Rect(0f, inRect.height - Text.SmallFontHeight, listRegion.width, Text.SmallFontHeight);
            var settingsRegion = new Rect(listRegion.width + 10f, 0f, inRect.width - listRegion.width - 10f, inRect.height);

            GUI.BeginGroup(inRect);

            GUI.BeginGroup(listRegion);

            Widgets.DrawMenuSection(listRegion);

            DrawPackList(listRegion.ContractedBy(8f));
            GUI.EndGroup();

            GUI.BeginGroup(globalWeightsRegion);
            DrawGlobalWeightsButton(globalWeightsRegion.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(settingsRegion);

            if (_selected?.Settings == null)
            {
                UiHelper.Label(settingsRegion.AtZero(), "This pack doesn't have any settings.", Color.grey, TextAnchor.MiddleCenter, GameFont.Small);
            }
            else
            {
                _selected.Settings.Draw(settingsRegion.AtZero());
            }

            GUI.EndGroup();

            GUI.EndGroup();
        }

        private void DrawPackList(Rect region)
        {
            float lineSpan = _packCount * Text.SmallFontHeight;
            var view = new Rect(0f, 0f, lineSpan > region.height ? region.width - 16f : region.width, lineSpan);

            GUI.BeginGroup(region);
            _scrollPos = GUI.BeginScrollView(region.AtZero(), _scrollPos, view);

            for (var index = 0; index < _packCount; index++)
            {
                PackEntry pack = _packs[index];

                var lineRegion = new Rect(0f, index * Text.SmallFontHeight, region.width, Text.SmallFontHeight);

                if (!lineRegion.IsVisible(region, _scrollPos))
                {
                    continue;
                }

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRegion);
                }

                if (_selected == pack)
                {
                    Widgets.DrawHighlightSelected(lineRegion);
                }

                Rect checkRegion = LayoutHelper.IconRect(lineRegion.x, lineRegion.y, Text.SmallFontHeight, Text.SmallFontHeight);
                var labelRegion = new Rect(lineRegion.x + Text.SmallFontHeight, lineRegion.y, lineRegion.width - Text.SmallFontHeight, Text.SmallFontHeight);

                UiHelper.Label(labelRegion, pack.Name);

                if (pack.Settings == null)
                {
                    GUI.DrawTexture(checkRegion, Textures.QuestionMark);

                    continue;
                }

                Widgets.CheckboxDraw(checkRegion.x, checkRegion.y, pack.Settings.Enabled, false, checkRegion.height);

                if (Widgets.ButtonInvisible(checkRegion))
                {
                    pack.Settings.Enabled = !pack.Settings.Enabled;
                }

                if (Widgets.ButtonInvisible(labelRegion))
                {
                    _selected = pack;
                }
            }

            GUI.EndScrollView();
            GUI.EndGroup();
        }

        private void DrawGlobalWeightsButton(Rect region)
        {
            if (Widgets.ButtonText(region, "Edit Global Weights"))
            {
                Find.WindowStack.Add(new GlobalWeightDialog());
            }
        }

        private sealed class PackEntry
        {
            public string Name { get; set; }
            public IStorytellerPackSettings Settings { get; set; }
        }
    }
}
