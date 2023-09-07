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
using JetBrains.Annotations;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models.Tables;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models.Mutators;

public class WeightMutator : IMutatorBase<ThingItem>
{
    private bool _percentage;
    private string _percentTooltip;
    private string _valueTooltip;
    private float _weight = 1f;
    private string _weightBuffer = "1";
    private string _weightText;

    public int Priority => 1;

    public string Label => "TKUtils.Fields.Weight".TranslateSimple();

    public void Prepare()
    {
        _weightText = Label;
        _valueTooltip = "TKUtils.MutatorTooltips.ValuePrice".TranslateSimple();
        _percentTooltip = "TKUtils.MutatorTooltips.PercentPrice".TranslateSimple();
    }

    public void Mutate(TableSettingsItem<ThingItem> item)
    {
        if (item.Data.ItemData != null)
        {
            item.Data.ItemData.Weight = _percentage ? Mathf.CeilToInt(item.Data.ItemData.Weight * (_weight / 100f)) : _weight;
        }
    }

    public void Draw(Rect canvas)
    {
        (Rect label, Rect field) = canvas.Split(0.75f);
        UiHelper.Label(label, _weightText);
        Widgets.TextFieldNumeric(field, ref _weight, ref _weightBuffer);

        if (UiHelper.FieldButton(field, _percentage ? "%" : "#", _percentage ? _percentTooltip : _valueTooltip))
        {
            _percentage = !_percentage;
        }
    }
}
