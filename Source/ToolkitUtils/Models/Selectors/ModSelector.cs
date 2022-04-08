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
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ModSelector<T> : ISelectorBase<T> where T : class, IShopItemBase
    {
        private bool _exclude = true;
        private string _excludeTooltip;
        private string _includeTooltip;
        private string _mod = "";
        private string _modText;

        public void Prepare()
        {
            _modText = Label;
            _excludeTooltip = "TKUtils.SelectorTooltips.ExcludeItem".TranslateSimple();
            _includeTooltip = "TKUtils.SelectorTooltips.IncludeItem".TranslateSimple();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.Split(0.75f);
            UiHelper.Label(label, _modText);

            if (UiHelper.TextField(field, _mod, out string input))
            {
                _mod = input;
                Dirty.Set(true);
            }

            if (!UiHelper.FieldButton(field, _exclude ? "!=" : "=", _exclude ? _includeTooltip : _excludeTooltip))
            {
                return;
            }

            _exclude = !_exclude;
            Dirty.Set(true);
        }

        public ObservableProperty<bool> Dirty { get; set; }

        public bool IsVisible(TableSettingsItem<T> item)
        {
            if (_mod.NullOrEmpty() || item.Data?.Data?.Mod == null)
            {
                return false;
            }

            bool shouldShow = item.Data.Data.Mod.EqualsIgnoreCase(_mod) || item.Data.Data.Mod.ToLower().Contains(_mod.ToLower());

            return _exclude ? !shouldShow : shouldShow;
        }

        public string Label => "TKUtils.Fields.Mod".TranslateSimple();
    }
}
