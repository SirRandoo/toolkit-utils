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
using JetBrains.Annotations;
using SirRandoo.CommonLib.Helpers;
using ToolkitUtils.Api;
using ToolkitUtils.Interfaces;
using ToolkitUtils.Models.Serialization;
using ToolkitUtils.Models.Tables;
using ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Models.Selectors
{
    public class AddPriceSelector : ISelectorBase<TraitItem>
    {
        private int _addPrice;
        private string _addPriceBuffer = "0";
        private string _addPriceText;
        private bool _bufferValid = true;
        private ComparisonType _comparison = ComparisonType.Equal;
        private readonly List<FloatMenuOption> _comparisonOptions = new List<FloatMenuOption>();

        private ComparisonType Comparison
        {
            get => _comparison;
            set
            {
                _comparison = value;
                Dirty.Set(true);
            }
        }

        public void Prepare()
        {
            _addPriceText = Label;

            foreach (ComparisonType type in ComparisonTypeExtensions.GetValues())
            {
                _comparisonOptions.Add(new FloatMenuOption(type.ToStringFast(), () => Comparison = type));
            }
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.Split(0.75f);
            UiHelper.Label(label, _addPriceText);

            (Rect button, Rect input) = field.Split(0.3f);

            if (Widgets.ButtonText(button, Comparison.ToStringFast()))
            {
                Find.WindowStack.Add(new FloatMenu(_comparisonOptions));
            }

            if (!UiHelper.NumberField(input, out int value, ref _addPriceBuffer, ref _bufferValid))
            {
                return;
            }

            _addPrice = value;
            Dirty.Set(true);
        }

        public ObservableProperty<bool> Dirty { get; set; }

        public bool IsVisible([NotNull] TableSettingsItem<TraitItem> item)
        {
            if (!item.Data.Enabled)
            {
                return false;
            }

            switch (Comparison)
            {
                case ComparisonType.Greater:
                    return item.Data.CostToAdd > _addPrice;
                case ComparisonType.Equal:
                    return item.Data.CostToAdd == _addPrice;
                case ComparisonType.Less:
                    return item.Data.CostToAdd < _addPrice;
                case ComparisonType.GreaterEqual:
                    return item.Data.CostToAdd >= _addPrice;
                case ComparisonType.LesserEqual:
                    return item.Data.CostToAdd <= _addPrice;
                default:
                    return false;
            }
        }

        public string Label => "TKUtils.Fields.AddPrice".TranslateSimple();
    }
}
