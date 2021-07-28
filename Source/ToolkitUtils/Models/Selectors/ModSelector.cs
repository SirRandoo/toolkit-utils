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
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ModSelector<T> : ISelectorBase<T> where T : class, IShopItemBase
    {
        private bool exclude = true;
        private string excludeTooltip;
        private string includeTooltip;
        private string mod = "";
        private string modText;

        public void Prepare()
        {
            modText = "TKUtils.Fields.Mod".Localize();
            excludeTooltip = "TKUtils.SelectorTooltips.ExcludeItem".Localize();
            includeTooltip = "TKUtils.SelectorTooltips.IncludeItem".Localize();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, modText);

            if (SettingsHelper.DrawTextField(field, mod, out string input))
            {
                mod = input;
                Dirty.Set(true);
            }

            if (!SettingsHelper.DrawFieldButton(
                field,
                exclude ? ResponseHelper.NotEqualGlyph : "=",
                exclude ? includeTooltip : excludeTooltip
            ))
            {
                return;
            }

            exclude = !exclude;
            Dirty.Set(true);
        }

        public ObservableProperty<bool> Dirty { get; set; }

        public bool IsVisible(TableSettingsItem<T> item)
        {
            if (mod.NullOrEmpty())
            {
                return false;
            }

            bool shouldShow = item.Data.Data.Mod.EqualsIgnoreCase(mod)
                              || item.Data.Data.Mod.ToLower().Contains(mod.ToLower());

            return exclude ? !shouldShow : shouldShow;
        }
    }
}
