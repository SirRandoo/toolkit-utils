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
    public class CategorySelector : ISelectorBase<ThingItem>
    {
        private string _categoryText;
        private string _excludeTooltip;
        private string _includeTooltip;
        private protected string Category = "";
        private protected bool Exclude = true;

        public void Prepare()
        {
            _categoryText = Label;
            _excludeTooltip = "TKUtils.SelectorTooltips.ExcludeItem".TranslateSimple();
            _includeTooltip = "TKUtils.SelectorTooltips.IncludeItem".TranslateSimple();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.Split(0.75f);
            UiHelper.Label(label, _categoryText);

            if (UiHelper.TextField(field, Category, out string input))
            {
                Category = input;
                Dirty.Set(true);
            }

            if (!UiHelper.FieldButton(field, Exclude ? '!' : '=', Exclude ? _includeTooltip : _excludeTooltip))
            {
                return;
            }

            Exclude = !Exclude;
            Dirty.Set(true);
        }

        public ObservableProperty<bool> Dirty { get; set; }

        public virtual bool IsVisible(TableSettingsItem<ThingItem> item)
        {
            if (Category.NullOrEmpty())
            {
                return false;
            }

            bool shouldShow = item.Data.Category.EqualsIgnoreCase(Category) || item.Data.Category.ToLower().Equals(Category.ToLower());

            return Exclude ? !shouldShow : shouldShow;
        }

        public virtual string Label => "TKUtils.Fields.Category".TranslateSimple();
    }
}
