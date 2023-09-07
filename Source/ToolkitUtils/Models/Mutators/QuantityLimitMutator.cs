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

public class QuantityLimitMutator : IMutatorBase<ThingItem>
{
    private int _limit = 1;
    private string _limitBuffer = "1";
    private string _quantityLimitText;
    private bool _toLimit;
    private string _toLimitTooltip;
    private string _toValueTooltip;

    public int Priority => 1;

    public string Label => "TKUtils.Fields.QuantityLimit".TranslateSimple();

    public void Prepare()
    {
        _quantityLimitText = Label;
        _toLimitTooltip = "TKUtils.MutatorTooltips.StackMode".TranslateSimple();
        _toValueTooltip = "TKUtils.MutatorTooltips.ValueMode".TranslateSimple();
    }

    public void Mutate(TableSettingsItem<ThingItem> item)
    {
        if (item.Data.ItemData != null)
        {
            item.Data.ItemData.QuantityLimit = _toLimit ? item.Data.Thing?.stackLimit ?? 1 : _limit;
        }
    }

    public void Draw(Rect canvas)
    {
        (Rect label, Rect field) = canvas.Split(0.75f);
        UiHelper.Label(label, _quantityLimitText);
        Widgets.TextFieldNumeric(field, ref _limit, ref _limitBuffer, 1f);

        if (UiHelper.FieldButton(field, _toLimit ? "S" : "#", _toLimit ? _toLimitTooltip : _toValueTooltip))
        {
            _toLimit = !_toLimit;
        }
    }
}