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
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class QuantityLimitMutator : IMutatorBase<ThingItem>
    {
        private int limit = 1;
        private string limitBuffer = "1";
        private string quantityLimitText;
        private bool toLimit;
        private string toLimitTooltip;
        private string toValueTooltip;

        public void Prepare()
        {
            quantityLimitText = "TKUtils.Fields.QuantityLimit".Localize();
            toLimitTooltip = "TKUtils.SelectorTooltips.StackMode".Localize();
            toValueTooltip = "TKUtils.SelectorTooltips.FixedMode".Localize();
        }

        public void Mutate([NotNull] TableSettingsItem<ThingItem> item)
        {
            item.Data.ItemData.QuantityLimit = toLimit ? item.Data.Thing?.stackLimit ?? 1 : limit;
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, quantityLimitText);
            Widgets.TextFieldNumeric(field, ref limit, ref limitBuffer, 1f);

            if (SettingsHelper.DrawFieldButton(
                field,
                toLimit ? Textures.Stack : Textures.Edit,
                toLimit ? toLimitTooltip : toValueTooltip
            ))
            {
                toLimit = !toLimit;
            }
        }
    }
}
