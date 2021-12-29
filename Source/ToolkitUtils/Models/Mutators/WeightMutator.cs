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
    public class WeightMutator : IMutatorBase<ThingItem>
    {
        private bool percentage;
        private string percentTooltip;
        private string valueTooltip;
        private float weight = 1f;
        private string weightBuffer = "1";
        private string weightText;

        public int Priority => 1;

        public string Label => "TKUtils.Fields.Weight".TranslateSimple();

        public void Prepare()
        {
            weightText = "TKUtils.Fields.Weight".TranslateSimple();
            valueTooltip = "TKUtils.MutatorTooltips.ValuePrice".TranslateSimple();
            percentTooltip = "TKUtils.MutatorTooltips.PercentPrice".TranslateSimple();
        }

        public void Mutate([NotNull] TableSettingsItem<ThingItem> item)
        {
            if (item.Data.ItemData != null)
            {
                item.Data.ItemData.Weight = percentage ? Mathf.CeilToInt(item.Data.ItemData.Weight * (weight / 100f)) : weight;
            }
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, weightText);
            Widgets.TextFieldNumeric(field, ref weight, ref weightBuffer);

            if (SettingsHelper.DrawFieldButton(field, percentage ? "%" : "#", percentage ? percentTooltip : valueTooltip))
            {
                percentage = !percentage;
            }
        }
    }
}
