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
    public class WeightSelector : ISelectorBase<ThingItem>
    {
        private ComparisonTypes comparison = ComparisonTypes.Equal;
        private List<FloatMenuOption> comparisonOptions;
        private int weight;
        private string weightBuffer = "0";
        private string weightText;

        public void Prepare()
        {
            weightText = "TKUtils.Fields.Weight".Localize();
            comparisonOptions = Data.ComparisonTypes.Select(
                    i => new FloatMenuOption($"TKUtils.PurgeMenu.{i.ToString()}".Localize(), () => comparison = i)
                )
               .ToList();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, weightText);

            (Rect button, Rect input) = field.ToForm(0.25f);

            if (Widgets.ButtonText(button, comparison.ToString()))
            {
                Find.WindowStack.Add(new FloatMenu(comparisonOptions));
            }

            if (!SettingsHelper.DrawNumberField(input, ref weight, ref weightBuffer, out int newCost))
            {
                return;
            }

            weight = newCost;
            weightBuffer = newCost.ToString();
            Dirty = true;
        }

        public bool Dirty { get; set; }

        public bool IsVisible(TableItem<ThingItem> item)
        {
            switch (comparison)
            {
                case ComparisonTypes.Greater:
                    return item.Data.ItemData.Weight > weight;
                case ComparisonTypes.Equal:
                    return Math.Abs(item.Data.ItemData.Weight - weight) < 0.0003;
                case ComparisonTypes.Less:
                    return item.Data.ItemData.Weight < weight;
                case ComparisonTypes.GreaterEqual:
                    return item.Data.ItemData.Weight >= weight;
                case ComparisonTypes.LessEqual:
                    return item.Data.ItemData.Weight <= weight;
                default:
                    return false;
            }
        }
    }
}
