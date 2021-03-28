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
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class RemovePriceMutator : IMutatorBase<TraitItem>
    {
        private bool percentage;
        private string percentTooltip;
        private int removePrice = 1;
        private string removePriceBuffer = "1";
        private string removePriceText;
        private string valueTooltip;

        public void Prepare()
        {
            removePriceText = "TKUtils.Fields.RemovePrice".Localize();
            valueTooltip = "TKUtils.MutatorTooltips.ValuePrice".Localize();
            percentTooltip = "TKUtils.MutatorTooltips.PercentPrice".Localize();
        }

        public void Mutate([NotNull] TableSettingsItem<TraitItem> item)
        {
            item.Data.CostToRemove =
                percentage ? Mathf.CeilToInt(item.Data.CostToRemove * (removePrice / 100f)) : removePrice;
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, removePriceText);
            Widgets.TextFieldNumeric(field, ref removePrice, ref removePriceBuffer, percentage ? -100f : 1f);

            if (SettingsHelper.DrawFieldButton(
                field,
                percentage ? "%" : "#",
                percentage ? percentTooltip : valueTooltip
            ))
            {
                percentage = !percentage;
            }
        }
    }
}
