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
using ToolkitUtils.Models.Serialization;
using ToolkitUtils.Models.Tables;
using UnityEngine;

namespace ToolkitUtils.Workers
{
    /// <summary>
    ///     Draws the data within the pawn kind store in a portable way
    ///     without the ability to edit their contents.
    /// </summary>
    public class PawnViewWorker : PawnTableWorker
    {
        /// <inheritdoc cref="PawnTableWorker.DrawHeaders"/>
        protected override void DrawHeaders(Rect region)
        {
            DrawSortableHeaders();
            DrawSortableHeaderIcon();
        }

        /// <inheritdoc cref="PawnTableWorker.DrawKind"/>
        protected override void DrawKind(Rect region, TableSettingsItem<PawnKindItem> item)
        {
            var nameRect = new Rect(NameHeaderTextRect.x, region.y, NameHeaderTextRect.width, RowLineHeight);
            var priceRect = new Rect(PriceHeaderTextRect.x, region.y, PriceHeaderTextRect.width, RowLineHeight);

            UiHelper.Label(nameRect, item.Data.Name);

            if (item.Data.Enabled)
            {
                UiHelper.Label(priceRect, item.Data.Cost.ToString("N0"));
            }
        }

        /// <inheritdoc cref="PawnTableWorker.NotifyResolutionChanged"/>
        public override void NotifyResolutionChanged(Rect region)
        {
            float distributedWidth = Mathf.FloorToInt((region.width - 16f) * 0.3333f);
            NameHeaderRect = new Rect(0f, 0f, distributedWidth, LineHeight);
            NameHeaderTextRect = new Rect(NameHeaderRect.x + 4f, NameHeaderRect.y, NameHeaderRect.width - 8f, NameHeaderRect.height);
            PriceHeaderRect = NameHeaderRect.Shift(Direction8Way.East, 1f);
            PriceHeaderTextRect = new Rect(PriceHeaderRect.x + 4f, PriceHeaderRect.y, PriceHeaderRect.width - 8f, PriceHeaderRect.height);
        }
    }
}
