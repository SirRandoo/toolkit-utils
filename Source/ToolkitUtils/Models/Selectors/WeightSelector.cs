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
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models.Tables;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.Constraints;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models.Selectors;

public class WeightSelector : ISelectorBase<ThingItem>
{
    private ComparisonTypes _comparison = ComparisonTypes.Equal;
    private List<FloatMenuOption> _comparisonOptions;
    private int _weight;
    private string _weightBuffer = "0";
    private string _weightText;
    private bool _weightValid = true;

    public void Prepare()
    {
        _weightText = "TKUtils.Fields.Weight".TranslateSimple();

        _comparisonOptions = Data.ComparisonTypes.Values.Select(
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
        UiHelper.Label(label, _weightText);

        (Rect button, Rect input) = field.Split(0.3f);

        if (Widgets.ButtonText(button, _comparison.AsOperator()))
        {
            Find.WindowStack.Add(new FloatMenu(_comparisonOptions));
        }

        if (!UiHelper.NumberField(input, out int value, ref _weightBuffer, ref _weightValid))
        {
            return;
        }

        _weight = value;
        Dirty.Set(true);
    }

    public ObservableProperty<bool> Dirty { get; set; }

    public bool IsVisible(TableSettingsItem<ThingItem> item)
    {
        switch (_comparison)
        {
            case ComparisonTypes.Greater:
                return item.Data.ItemData?.Weight > _weight;
            case ComparisonTypes.Equal:
                return Math.Abs(item.Data.ItemData?.Weight ?? 1f - _weight) < 0.0003;
            case ComparisonTypes.Less:
                return item.Data.ItemData?.Weight < _weight;
            case ComparisonTypes.GreaterEqual:
                return item.Data.ItemData?.Weight >= _weight;
            case ComparisonTypes.LessEqual:
                return item.Data.ItemData?.Weight <= _weight;
            default:
                return false;
        }
    }

    public string Label => "TKUtils.Fields.Weight".TranslateSimple();
}