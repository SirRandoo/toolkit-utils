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
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.StorytellerPackSettings
{
    [UsedImplicitly]
    public class TorytalkerPackSettings : PackSettingsBase
    {
        private string _mtbBuffer;
        private bool _mtbBufferValid;

        /// <inheritdoc/>
        public override bool Enabled
        {
            get => ToolkitSettings.ToryTalkerEnabled;
            set => ToolkitSettings.ToryTalkerEnabled = value;
        }

        /// <inheritdoc/>
        [NotNull]
        public override string Tooltip =>
            "The classic, and most balanced Toolkit experience. ToryTalker uses the global weights, and it's own weighting system based on events that have happened recently.";

        /// <inheritdoc/>
        public override void ResetState()
        {
            _mtbBufferValid = true;
            _mtbBuffer = ToolkitSettings.ToryTalkerMTBDays.ToString("N2");
        }

        /// <inheritdoc/>
        public override void Draw(Rect region)
        {
            var headerRegion = new Rect(0f, 0f, region.width, Text.SmallFontHeight * 5f);
            var contentRegion = new Rect(0f, headerRegion.height, region.width, region.height - headerRegion.height);

            GUI.BeginGroup(region);

            GUI.BeginGroup(headerRegion);
            GUI.color = Color.grey;
            UiHelper.Label(headerRegion, "Tory Talker uses the global weights system, and its own weighting system based on events that have happened recently.");
            GUI.color = Color.white;
            GUI.EndGroup();

            GUI.BeginGroup(contentRegion);

            (Rect labelRegion, Rect fieldRegion) = new Rect(0f, 0f, region.width, Text.SmallFontHeight).Split(0.8f);
            UiHelper.Label(labelRegion, "Average days between events");

            if (UiHelper.NumberField(fieldRegion, out float newDays, ref _mtbBuffer, ref _mtbBufferValid, 0.5f, 10f))
            {
                ToolkitSettings.ToryTalkerMTBDays = newDays;
            }

            GUI.EndGroup();

            GUI.EndGroup();
        }
    }
}
