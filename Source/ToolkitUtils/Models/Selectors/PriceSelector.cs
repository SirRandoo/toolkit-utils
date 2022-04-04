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
using System.Collections.Generic;
using System.Linq;
using CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class PriceSelector<T> : ISelectorBase<T> where T : class, IShopItemBase
    {
        private ComparisonTypes _comparison = ComparisonTypes.Equal;
        private List<FloatMenuOption> _comparisonOptions;
        private int _price;
        private string _priceBuffer = "0";
        private bool _priceBufferValid = true;
        private string _priceText;

        public void Prepare()
        {
            _priceText = Label;

            _comparisonOptions = Data.ComparisonTypes.Select(
                    i => new FloatMenuOption(
                        i.AsOperator(),
                        () =>
                        {
                            _comparison = i;
                            Dirty.Set(true);
                        }
                    )
                )
               .ToList();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.Split(0.75f);
            UiHelper.Label(label, _priceText);

            (Rect button, Rect input) = field.Split(0.3f);

            if (Widgets.ButtonText(button, _comparison.AsOperator()))
            {
                Find.WindowStack.Add(new FloatMenu(_comparisonOptions));
            }

            if (UiHelper.NumberField(input, ref _priceBuffer, ref _price, ref _priceBufferValid))
            {
                Dirty.Set(true);
            }
        }

        public ObservableProperty<bool> Dirty { get; set; }

        public bool IsVisible(TableSettingsItem<T> item)
        {
            switch (_comparison)
            {
                case ComparisonTypes.GreaterEqual:
                    return item.Data.Cost >= _price;
                case ComparisonTypes.Greater:
                    return item.Data.Cost > _price;
                case ComparisonTypes.Equal:
                    return item.Data.Cost == _price;
                case ComparisonTypes.Less:
                    return item.Data.Cost < _price;
                case ComparisonTypes.LessEqual:
                    return item.Data.Cost <= _price;
                default:
                    return false;
            }
        }

        public string Label => "TKUtils.Fields.Price".TranslateSimple();
    }
}
