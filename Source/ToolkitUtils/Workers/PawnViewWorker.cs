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

namespace SirRandoo.ToolkitUtils.Workers
{
    public class PawnViewWorker : PawnTableWorker
    {
        public override void DrawHeaders(Rect canvas)
        {
            DrawSortableHeaders();
            DrawSortableHeaderIcon();
        }

        protected override void DrawKind(Rect canvas, TableSettingsItem<PawnKindItem> item)
        {
            var nameRect = new Rect(NameHeaderTextRect.x, canvas.y, NameHeaderTextRect.width, RowLineHeight);
            var priceRect = new Rect(PriceHeaderTextRect.x, canvas.y, PriceHeaderTextRect.width, RowLineHeight);

            SettingsHelper.DrawLabel(nameRect, item.Data.Name);

            if (item.Data.Enabled)
            {
                SettingsHelper.DrawLabel(priceRect, item.Data.Cost.ToString("N0"));
            }
        }

        public override void NotifyResolutionChanged(Rect canvas)
        {
            float distributedWidth = Mathf.FloorToInt((canvas.width - 16f) * 0.3333f);
            NameHeaderRect = new Rect(0f, 0f, distributedWidth, LineHeight);
            NameHeaderTextRect = new Rect(NameHeaderRect.x + 4f, NameHeaderRect.y, NameHeaderRect.width - 8f, NameHeaderRect.height);
            PriceHeaderRect = NameHeaderRect.ShiftRight(1f);
            PriceHeaderTextRect = new Rect(PriceHeaderRect.x + 4f, PriceHeaderRect.y, PriceHeaderRect.width - 8f, PriceHeaderRect.height);
        }
    }
}
