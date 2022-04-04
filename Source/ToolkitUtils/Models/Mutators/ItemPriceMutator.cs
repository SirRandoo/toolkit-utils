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
using CommonLib.Helpers;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ItemPriceMutator : IMutatorBase<ThingItem>
    {
        private bool _percentage;
        private string _percentTooltip;
        private int _price = 1;
        private string _priceBuffer = "1";
        private string _priceText;
        private string _valueTooltip;

        public int Priority => 1;

        public string Label => "TKUtils.Fields.Price".TranslateSimple();

        public void Prepare()
        {
            _priceText = Label;
            _valueTooltip = "TKUtils.MutatorTooltips.ValuePrice".TranslateSimple();
            _percentTooltip = "TKUtils.MutatorTooltips.PercentPrice".TranslateSimple();
        }

        public void Mutate([NotNull] TableSettingsItem<ThingItem> item)
        {
            item.Data.Item!.price = _percentage ? Mathf.CeilToInt(item.Data.Item.price * (_price / 100f)) : _price;
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.Split(0.75f);
            UiHelper.Label(label, _priceText);
            Widgets.TextFieldNumeric(field, ref _price, ref _priceBuffer, _percentage ? -100f : 1f);

            if (UiHelper.FieldButton(field, _percentage ? '%' : '#', _percentage ? _percentTooltip : _valueTooltip))
            {
                _percentage = !_percentage;
            }
        }
    }
}
