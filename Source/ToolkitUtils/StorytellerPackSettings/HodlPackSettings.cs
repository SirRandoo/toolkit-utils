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
using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.CommonLib.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.StorytellerPackSettings
{
    [UsedImplicitly]
    public class HodlPackSettings : PackSettingsBase
    {
        private List<Entry> _categoryEntries;
        private string _mtbBuffer;
        private bool _mtbBufferValid;
        private Vector2 _scrollPos;
        private int _totalCategoryWeight;
        private int _totalKarmaWeight;
        private List<Entry> _typeEntries;
        private int _weightLineSpan;

        /// <inheritdoc/>
        public override bool Enabled
        {
            get => ToolkitSettings.HodlBotEnabled;
            set => ToolkitSettings.HodlBotEnabled = value;
        }

        /// <inheritdoc/>
        [NotNull]
        public override string Tooltip =>
            "Random by category, or type. Hodlbot chooses events from a random category or type. The chance of one of these categories/types being picked is based the pack's weights.";

        /// <inheritdoc/>
        public override void ResetState()
        {
            _mtbBufferValid = true;
            _scrollPos = Vector2.zero;
            _mtbBuffer = ToolkitSettings.HodlBotMTBDays.ToString("N2");

            if (_categoryEntries == null)
            {
                _categoryEntries = new List<Entry>();

                foreach ((string key, float value) in ToolkitSettings.VoteCategoryWeights)
                {
                    _categoryEntries.Add(new Entry { Name = key, Weight = Mathf.FloorToInt(value), Buffer = value.ToString("N2"), BufferValid = true });
                }

                _totalCategoryWeight = RecalculateTotalCategoryWeight();
                _weightLineSpan = (_typeEntries?.Count ?? 0) + _categoryEntries.Count;
            }

            if (_typeEntries == null)
            {
                _typeEntries = new List<Entry>();

                foreach ((string key, float value) in ToolkitSettings.VoteTypeWeights)
                {
                    _typeEntries.Add(new Entry { Name = key, Weight = Mathf.FloorToInt(value), Buffer = value.ToString("N2"), BufferValid = true });
                }

                _totalKarmaWeight = RecalculateTotalKarmaWeight();
                _weightLineSpan = _typeEntries.Count + _categoryEntries.Count;
            }
        }

        /// <inheritdoc/>
        public override void Draw(Rect region)
        {
            var headerRegion = new Rect(0f, 0f, region.width, Text.SmallFontHeight * 5f);
            var settingsRegion = new Rect(0f, headerRegion.height, region.width, region.height - headerRegion.height);

            GUI.BeginGroup(region);

            GUI.BeginGroup(headerRegion);

            UiHelper.Label(
                headerRegion,
                "HodlBot chooses events from a random category or type. The chance of one of these categories/types being picked is based on the weights below. Setting something to 0% will disable.",
                Color.gray,
                TextAnchor.MiddleCenter,
                GameFont.Small
            );

            GUI.EndGroup();

            GUI.BeginGroup(settingsRegion);

            DrawSettings(settingsRegion.AtZero());

            GUI.EndGroup();

            GUI.EndGroup();
        }

        private void DrawSettings(Rect region)
        {
            var listing = new Listing_Standard();
            var view = new Rect(0f, 0f, region.width - 16f, Text.SmallFontHeight * (_weightLineSpan + 1));

            _scrollPos = GUI.BeginScrollView(region, _scrollPos, view);
            listing.Begin(region);

            (Rect mtbLabel, Rect mtbField) = listing.Split(0.8f);
            UiHelper.Label(mtbLabel, "Average days between events");

            if (UiHelper.NumberField(mtbField, out float newMtb, ref _mtbBuffer, ref _mtbBufferValid, 0.5f, 10f))
            {
                ToolkitSettings.HodlBotMTBDays = newMtb;
            }

            listing.GapLine();

            foreach (Entry entry in _categoryEntries)
            {
                Rect lineRegion = listing.GetRect(Text.SmallFontHeight);

                if (!lineRegion.IsVisible(region, _scrollPos))
                {
                    continue;
                }

                string buffer = entry.Buffer;
                bool bufferValid = entry.BufferValid;
                var relativeWeight = (float)Math.Round((float)entry.Weight / _totalCategoryWeight * 100f, 2);

                (Rect labelRegion, Rect fieldRegion) = lineRegion.Split(0.8f);
                Widgets.LabelFit(labelRegion, $"{entry.Name} {relativeWeight:P}");

                if (UiHelper.NumberField(fieldRegion, out int newWeight, ref buffer, ref bufferValid))
                {
                    ToolkitSettings.VoteCategoryWeights[entry.Name] = entry.Weight = newWeight;
                    _totalCategoryWeight = RecalculateTotalCategoryWeight();
                }

                entry.Buffer = buffer;
                entry.BufferValid = bufferValid;
            }

            foreach (Entry entry in _typeEntries)
            {
                Rect lineRegion = listing.GetRect(Text.SmallFontHeight);

                if (!lineRegion.IsVisible(region, _scrollPos))
                {
                    continue;
                }

                string buffer = entry.Buffer;
                bool bufferValid = entry.BufferValid;
                var relativeWeight = (float)Math.Round((float)entry.Weight / _totalKarmaWeight * 100f, 2);

                (Rect labelRegion, Rect fieldRegion) = lineRegion.Split(0.8f);
                Widgets.LabelFit(labelRegion, $"{entry.Name} {relativeWeight:P}");

                if (UiHelper.NumberField(fieldRegion, out int newWeight, ref buffer, ref bufferValid))
                {
                    ToolkitSettings.VoteTypeWeights[entry.Name] = entry.Weight = newWeight;
                    _totalKarmaWeight = RecalculateTotalKarmaWeight();
                }

                entry.Buffer = buffer;
                entry.BufferValid = bufferValid;
            }

            listing.End();
            GUI.EndScrollView();
        }

        private int RecalculateTotalCategoryWeight()
        {
            var value = 0;

            foreach (Entry entry in _categoryEntries)
            {
                value += Mathf.FloorToInt(entry.Weight);
            }

            return value;
        }

        private int RecalculateTotalKarmaWeight()
        {
            var value = 0;

            foreach (Entry entry in _typeEntries)
            {
                value += Mathf.FloorToInt(entry.Weight);
            }

            return value;
        }

        private sealed class Entry
        {
            public string Name { get; set; }
            public int Weight { get; set; }
            public string Buffer { get; set; }
            public bool BufferValid { get; set; }
        }
    }
}
