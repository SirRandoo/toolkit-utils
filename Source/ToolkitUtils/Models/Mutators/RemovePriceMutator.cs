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

public class RemovePriceMutator : IMutatorBase<TraitItem>
{
    private bool _percentage;
    private string _percentTooltip;
    private int _removePrice = 1;
    private string _removePriceBuffer = "1";
    private string _removePriceText;
    private string _valueTooltip;

    public int Priority => 1;

    public string Label => "TKUtils.Fields.RemovePrice".TranslateSimple();

    public void Prepare()
    {
        _removePriceText = Label;
        _valueTooltip = "TKUtils.MutatorTooltips.ValuePrice".TranslateSimple();
        _percentTooltip = "TKUtils.MutatorTooltips.PercentPrice".TranslateSimple();
    }

    public void Mutate(TableSettingsItem<TraitItem> item)
    {
        item.Data.CostToRemove = _percentage ? Mathf.CeilToInt(item.Data.CostToRemove * (_removePrice / 100f)) : _removePrice;
    }

    public void Draw(Rect canvas)
    {
        (Rect label, Rect field) = canvas.Split(0.75f);
        UiHelper.Label(label, _removePriceText);
        Widgets.TextFieldNumeric(field, ref _removePrice, ref _removePriceBuffer, _percentage ? -100f : 1f);

        if (UiHelper.FieldButton(field, _percentage ? "%" : "#", _percentage ? _percentTooltip : _valueTooltip))
        {
            _percentage = !_percentage;
        }
    }
}