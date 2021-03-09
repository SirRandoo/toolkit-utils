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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models.Tables;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class CategorySelector : SelectorBase<ThingItem>
    {
        private string category = "";
        private string categoryText;
        private bool invert;
        private string invertTooltip;

        public override void Prepare()
        {
            categoryText = "TKUtils.Fields.Category".Localize();
            invertTooltip = "TKUtils.SelectorTooltips.Invert".Localize();
        }

        public override void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, categoryText);

            if (SettingsHelper.DrawTextField(field, category, out string input))
            {
                category = input;
                Dirty = true;
            }

            GUI.color = invert ? Color.yellow : Color.white;
            if (SettingsHelper.DrawFieldButton(field, "I", invertTooltip))
            {
                invert = !invert;
                Dirty = true;
            }

            GUI.color = Color.white;
        }

        public override bool IsVisible(TableItem<ThingItem> item)
        {
            if (category.NullOrEmpty())
            {
                return true;
            }

            bool shouldShow = item.Data.Category.EqualsIgnoreCase(category)
                              || item.Data.Category.ToLower().Equals(category.ToLower());

            return invert ? !shouldShow : shouldShow;
        }
    }
}
