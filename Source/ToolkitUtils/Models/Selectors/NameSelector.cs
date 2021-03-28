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
    public class NameSelector<T> : ISelectorBase<T> where T : class, IShopItemBase
    {
        private bool exclude = true;
        private string excludeTooltip;
        private string includeTooltip;
        private string name = "";
        private string nameText;

        public void Prepare()
        {
            nameText = "TKUtils.Fields.Name".Localize();
            excludeTooltip = "TKUtils.SelectorTooltips.ExcludeItem".Localize();
            includeTooltip = "TKUtils.SelectorTooltips.IncludeItem".Localize();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, nameText);

            if (SettingsHelper.DrawTextField(field, name, out string input))
            {
                name = input;
                Dirty.Set(true);
            }

            if (!SettingsHelper.DrawFieldButton(
                field,
                exclude ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex,
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
            if (name.NullOrEmpty())
            {
                return true;
            }

            bool shouldShow = item.Data.Name.EqualsIgnoreCase(name)
                              || item.Data.Name.ToLower().Contains(name.ToLower());

            return exclude ? !shouldShow : shouldShow;
        }
    }
}
