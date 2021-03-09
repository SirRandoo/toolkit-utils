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
    public class ModSelector<T> : ISelectorBase<T> where T : IShopItemBase
    {
        private bool invert;
        private string invertTooltip;
        private string mod = "";
        private string modText;

        public void Prepare()
        {
            modText = "TKUtils.Fields.Mod".Localize();
            invertTooltip = "TKUtils.SelectorTooltips.Invert".Localize();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, modText);

            if (SettingsHelper.DrawTextField(field, mod, out string input))
            {
                mod = input;
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

        public bool Dirty { get; set; }

        public bool IsVisible(TableItem<T> item)
        {
            if (mod.NullOrEmpty())
            {
                return true;
            }

            bool shouldShow = item.Data.Data.Mod.EqualsIgnoreCase(mod)
                              || item.Data.Data.Mod.ToLower().Contains(mod.ToLower());

            return invert ? !shouldShow : shouldShow;
        }
    }
}
