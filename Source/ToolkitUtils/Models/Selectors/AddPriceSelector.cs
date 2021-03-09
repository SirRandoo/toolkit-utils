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

using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models.Tables;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class AddPriceSelector : SelectorBase<TraitItem>
    {
        private int addPrice;
        private string addPriceBuffer = "0";
        private string addPriceText;
        private ComparisonTypes comparison = ComparisonTypes.Equal;
        private List<FloatMenuOption> comparisonOptions;

        public override void Prepare()
        {
            addPriceText = "TKUtils.Fields.AddPrice".Localize();
            comparisonOptions = ToolkitUtils.Data.ComparisonTypes.Select(
                    i => new FloatMenuOption($"TKUtils.PurgeMenu.{i.ToString()}".Localize(), () => comparison = i)
                )
               .ToList();
        }

        public override void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, addPriceText);

            (Rect button, Rect input) = field.ToForm(0.25f);

            if (Widgets.ButtonText(button, comparison.ToString()))
            {
                Find.WindowStack.Add(new FloatMenu(comparisonOptions));
            }

            if (!SettingsHelper.DrawNumberField(input, ref addPrice, ref addPriceBuffer, out int newCost))
            {
                return;
            }

            addPrice = newCost;
            addPriceBuffer = newCost.ToString();
            Dirty = true;
        }

        public override bool IsVisible(TableItem<TraitItem> item)
        {
            switch (comparison)
            {
                case ComparisonTypes.Greater:
                    return item.Data.CostToAdd > addPrice;
                case ComparisonTypes.Equal:
                    return item.Data.CostToAdd == addPrice;
                case ComparisonTypes.Less:
                    return item.Data.CostToAdd < addPrice;
                case ComparisonTypes.GreaterEqual:
                    return item.Data.CostToAdd >= addPrice;
                case ComparisonTypes.LessEqual:
                    return item.Data.CostToAdd <= addPrice;
                default:
                    return false;
            }
        }
    }
}
