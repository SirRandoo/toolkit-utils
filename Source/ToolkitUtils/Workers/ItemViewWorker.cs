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

using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class ItemViewWorker : ItemTableWorker
    {
        public override void DrawHeaders(Rect canvas)
        {
            DrawSortableHeaders();
            DrawSortableHeaderIcon();
        }

        protected override void DrawItem(Rect canvas, TableSettingsItem<ThingItem> item)
        {
            bool hasIcon = Widgets.CanDrawIconFor(item.Data.Thing);

            var infoRect = new Rect(hasIcon ? NameHeaderRect.x : NameHeaderTextRect.x, canvas.y, hasIcon ? NameHeaderRect.width : NameHeaderTextRect.width, canvas.height);
            var priceRect = new Rect(PriceHeaderRect.x, canvas.y, PriceHeaderRect.width, RowLineHeight);
            var categoryRect = new Rect(CategoryHeaderRect.x, canvas.y, CategoryHeaderRect.width, RowLineHeight);

            if (item.Data.Thing != null && hasIcon)
            {
                SettingsHelper.DrawThing(infoRect, item.Data.Thing, item.Data.Name, !item.EditingName);
            }

            if (item.Data.Cost > 0)
            {
                SettingsHelper.DrawLabel(priceRect, item.Data.Cost.ToString("N0"));
            }

            SettingsHelper.DrawLabel(categoryRect, item.Data.Category);
        }

        public override void NotifyResolutionChanged(Rect canvas)
        {
            float distributedWidth = Mathf.FloorToInt((canvas.width - 16f) * 0.333f);
            NameHeaderRect = new Rect(0f, 0f, distributedWidth, LineHeight);
            NameHeaderTextRect = new Rect(NameHeaderRect.x + 4f, NameHeaderRect.y, NameHeaderRect.width - 8f, NameHeaderRect.height);
            PriceHeaderRect = NameHeaderRect.ShiftRight(1f);
            PriceHeaderTextRect = new Rect(PriceHeaderRect.x + 4f, PriceHeaderRect.y, PriceHeaderRect.width - 8f, PriceHeaderRect.height);
            CategoryHeaderRect = PriceHeaderRect.ShiftRight(1f);
            CategoryHeaderTextRect = new Rect(CategoryHeaderRect.x + 4f, CategoryHeaderRect.y, CategoryHeaderRect.width - 8f, CategoryHeaderRect.height);
        }
    }
}
