// MIT License
//
// Copyright (c) 2021 SirRandoo
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

using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class EventViewWorker : EventTableWorker
    {
        public override void DrawHeaders(Rect canvas)
        {
            DrawSortableHeaders();
            DrawSortableHeaderIcon();
        }

        protected override void DrawEvent(Rect canvas, TableSettingsItem<EventItem> ev)
        {
            var nameRect = new Rect(NameHeaderTextRect.x, canvas.y, NameHeaderTextRect.width, RowLineHeight);
            var priceRect = new Rect(PriceHeaderTextRect.x, canvas.y, PriceHeaderTextRect.width, RowLineHeight);
            var karmaRect = new Rect(KarmaHeaderTextRect.x, canvas.y, KarmaHeaderTextRect.width, RowLineHeight);

            SettingsHelper.DrawLabel(nameRect, ev.Data.Name);

            if (ev.Data.Enabled)
            {
                SettingsHelper.DrawLabel(priceRect, ev.Data.Cost.ToString("N0"));
            }

            SettingsHelper.DrawLabel(karmaRect, ev.Data.KarmaType.ToString());
        }

        public override void NotifyResolutionChanged(Rect canvas)
        {
            float consumedWidth = canvas.width - 10f; // Icon buttons
            float labelWidth = Mathf.FloorToInt(consumedWidth * 0.4f);
            float remainingWidth = consumedWidth - labelWidth;
            float distributedWidth = Mathf.FloorToInt(remainingWidth / 2f);
            NameHeaderRect = new Rect(0f, 0f, labelWidth, LineHeight);
            NameHeaderTextRect = new Rect(NameHeaderRect.x + 4f, NameHeaderRect.y, NameHeaderRect.width - 8f, NameHeaderRect.height);
            PriceHeaderRect = new Rect(NameHeaderRect.x + NameHeaderRect.width + 1f, 0f, distributedWidth, LineHeight);
            PriceHeaderTextRect = new Rect(PriceHeaderRect.x + 4f, PriceHeaderRect.y, PriceHeaderRect.width - 8f, PriceHeaderRect.height);
            KarmaHeaderRect = new Rect(PriceHeaderRect.x + PriceHeaderRect.width + 1f, 0f, distributedWidth, LineHeight);
            KarmaHeaderTextRect = new Rect(KarmaHeaderRect.x + 4f, KarmaHeaderRect.y, KarmaHeaderRect.width - 8f, KarmaHeaderRect.height);
        }
    }
}
