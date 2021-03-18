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
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class AddPriceSelector : ISelectorBase<TraitItem>
    {
        private int addPrice;
        private string addPriceBuffer = "0";
        private string addPriceText;
        private ComparisonTypes comparison = ComparisonTypes.Equal;
        private List<FloatMenuOption> comparisonOptions;

        public void Prepare()
        {
            addPriceText = "TKUtils.Fields.AddPrice".Localize();
            comparisonOptions = Data.ComparisonTypes.Select(
                    i => new FloatMenuOption(
                        i.AsOperator(),
                        () =>
                        {
                            comparison = i;
                            Dirty.Set(true);
                        }
                    )
                )
               .ToList();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, addPriceText);

            (Rect button, Rect input) = field.ToForm(0.3f);

            if (Widgets.ButtonText(button, comparison.AsOperator()))
            {
                Find.WindowStack.Add(new FloatMenu(comparisonOptions));
            }

            if (!SettingsHelper.DrawNumberField(input, ref addPrice, ref addPriceBuffer, out int newCost))
            {
                return;
            }

            addPrice = newCost;
            addPriceBuffer = newCost.ToString();
            Dirty.Set(true);
        }

        public ObservableProperty<bool> Dirty { get; set; }

        public bool IsVisible([NotNull] TableSettingsItem<TraitItem> item)
        {
            if (!item.Data.Enabled)
            {
                return false;
            }

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
