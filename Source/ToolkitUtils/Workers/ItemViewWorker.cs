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

using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    /// <summary>
    ///     A class for drawing <see cref="ThingItem"/>s in a portable way
    ///     without any means of editing the contents within through the
    ///     table's UI.
    /// </summary>
    public class ItemViewWorker : ItemTableWorker
    {
        /// <inheritdoc cref="ItemTableWorker.DrawHeaders"/>
        protected override void DrawHeaders(Rect region)
        {
            DrawSortableHeaders();
            DrawSortableHeaderIcon();
        }

        /// <inheritdoc cref="ItemTableWorker.DrawItem"/>
        protected override void DrawItem(Rect region, TableSettingsItem<ThingItem> item)
        {
            bool hasIcon = Widgets.CanDrawIconFor(item.Data.Thing);

            var infoRect = new Rect(hasIcon ? NameHeaderRect.x : NameHeaderTextRect.x, region.y, hasIcon ? NameHeaderRect.width : NameHeaderTextRect.width, region.height);
            var priceRect = new Rect(PriceHeaderRect.x, region.y, PriceHeaderRect.width, RowLineHeight);
            var categoryRect = new Rect(CategoryHeaderRect.x, region.y, CategoryHeaderRect.width, RowLineHeight);

            if (item.Data.Thing != null && hasIcon)
            {
                UiHelper.DrawThing(infoRect, item.Data.Thing, item.Data.Name, !item.EditingName);
            }

            if (item.Data.Cost > 0)
            {
                UiHelper.Label(priceRect, item.Data.Cost.ToString("N0"));
            }

            UiHelper.Label(categoryRect, item.Data.Category);
        }

        /// <inheritdoc cref="ItemTableWorker.NotifyResolutionChanged"/>
        public override void NotifyResolutionChanged(Rect region)
        {
            float distributedWidth = Mathf.FloorToInt((region.width - 16f) * 0.333f);
            NameHeaderRect = new Rect(0f, 0f, distributedWidth, LineHeight);
            NameHeaderTextRect = new Rect(NameHeaderRect.x + 4f, NameHeaderRect.y, NameHeaderRect.width - 8f, NameHeaderRect.height);
            PriceHeaderRect = NameHeaderRect.Shift(Direction8Way.East, 1f);
            PriceHeaderTextRect = new Rect(PriceHeaderRect.x + 4f, PriceHeaderRect.y, PriceHeaderRect.width - 8f, PriceHeaderRect.height);
            CategoryHeaderRect = PriceHeaderRect.Shift(Direction8Way.East, 1f);
            CategoryHeaderTextRect = new Rect(CategoryHeaderRect.x + 4f, CategoryHeaderRect.y, CategoryHeaderRect.width - 8f, CategoryHeaderRect.height);
        }
    }
}
