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

using System.Text;
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class TraitViewWorker : TraitTableWorker
    {
        public override void DrawHeaders(Rect canvas)
        {
            DrawSortableHeaders();
            DrawSortableHeaderIcon();
        }

        protected override void DrawTrait(Rect canvas, TableSettingsItem<TraitItem> trait)
        {
            var nameMouseOverRect = new Rect(NameHeaderRect.x, canvas.y, NameHeaderRect.width, RowLineHeight);
            var nameRect = new Rect(NameHeaderTextRect.x, canvas.y, NameHeaderTextRect.width, RowLineHeight);
            var addPriceRect = new Rect(AddPriceHeaderTextRect.x, canvas.y, AddPriceHeaderTextRect.width, RowLineHeight);
            var removePriceRect = new Rect(RemovePriceHeaderTextRect.x, canvas.y, RemovePriceHeaderTextRect.width, RowLineHeight);

            UiHelper.Label(nameRect, trait.Data.Name);

            if (!trait.EditingName)
            {
                Widgets.DrawHighlightIfMouseover(nameMouseOverRect);

                var builder = new StringBuilder();
                builder.AppendLine(trait.Data.Description);
                builder.AppendLine();

                foreach (string i in trait.Data.Stats)
                {
                    builder.AppendLine($"- {i}");
                }

                TooltipHandler.TipRegion(nameMouseOverRect, builder.ToString());
            }

            if (trait.Data.CanAdd)
            {
                UiHelper.Label(addPriceRect, trait.Data.CostToAdd.ToString("N0"));
            }

            if (trait.Data.CanRemove)
            {
                UiHelper.Label(removePriceRect, trait.Data.CostToRemove.ToString("N0"));
            }
        }

        public override void NotifyResolutionChanged(Rect canvas)
        {
            float distributedWidth = Mathf.FloorToInt((canvas.width - 16f) * 0.3333f);
            NameHeaderRect = new Rect(0f, 0f, distributedWidth, LineHeight);
            NameHeaderTextRect = new Rect(NameHeaderRect.x + 4f, NameHeaderRect.y, NameHeaderRect.width - 8f, NameHeaderRect.height);
            AddPriceHeaderRect = NameHeaderRect.Shift(Direction8Way.East, 1f);
            AddPriceHeaderTextRect = new Rect(AddPriceHeaderRect.x + 4f, AddPriceHeaderRect.y, AddPriceHeaderRect.width - 8f, AddPriceHeaderRect.height);
            RemovePriceHeaderRect = AddPriceHeaderRect.Shift(Direction8Way.East, 1f);
            RemovePriceHeaderTextRect = new Rect(RemovePriceHeaderRect.x + 4f, RemovePriceHeaderRect.y, RemovePriceHeaderRect.width - 8f, RemovePriceHeaderRect.height);
        }
    }
}
