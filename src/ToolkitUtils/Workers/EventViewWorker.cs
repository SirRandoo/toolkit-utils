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

using SirRandoo.CommonLib.Helpers;
using ToolkitUtils.Models;
using ToolkitUtils.Models.Tables;
using UnityEngine;

namespace ToolkitUtils.Workers
{
    /// <summary>
    ///     A class for displaying <see cref="EventItem"/> data, without the
    ///     ability to modify the contents of the table, in a portable way.
    /// </summary>
    public class EventViewWorker : EventTableWorker
    {
        /// <inheritdoc cref="EventTableWorker.DrawHeaders"/>
        protected override void DrawHeaders(Rect region)
        {
            DrawSortableHeaders();
            DrawSortableHeaderIcon();
        }

        /// <inheritdoc cref="EventTableWorker.DrawEvent"/>
        protected override void DrawEvent(Rect canvas, TableSettingsItem<EventItem> ev)
        {
            var nameRect = new Rect(NameHeaderTextRect.x, canvas.y, NameHeaderTextRect.width, RowLineHeight);
            var priceRect = new Rect(PriceHeaderTextRect.x, canvas.y, PriceHeaderTextRect.width, RowLineHeight);
            var karmaRect = new Rect(KarmaHeaderTextRect.x, canvas.y, KarmaHeaderTextRect.width, RowLineHeight);

            UiHelper.Label(nameRect, ev.Data.Name);

            if (ev.Data.Enabled)
            {
                UiHelper.Label(priceRect, ev.Data.Cost.ToString("N0"));
            }

            UiHelper.Label(karmaRect, ev.Data.KarmaType.ToString());
        }

        /// <inheritdoc cref="EventTableWorker.NotifyResolutionChanged"/>
        public override void NotifyResolutionChanged(Rect region)
        {
            float consumedWidth = region.width - 10f; // Icon buttons
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
