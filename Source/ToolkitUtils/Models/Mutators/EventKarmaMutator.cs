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
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models.Tables;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models.Mutators;

public class EventKarmaMutator : IMutatorBase<EventItem>
{
    private KarmaType _karmaType = KarmaType.Neutral;
    private string _karmaTypeLabel;

    public int Priority => 1;

    public string Label => "TKUtils.Fields.KarmaType".TranslateSimple();

    public void Prepare()
    {
        _karmaTypeLabel = Label;
    }

    public void Draw(Rect canvas)
    {
        (Rect label, Rect field) = canvas.Split(0.75f);
        UiHelper.Label(label, _karmaTypeLabel);

        if (Widgets.ButtonText(field, _karmaType.ToString()))
        {
            Find.WindowStack.Add(new FloatMenu(Data.KarmaTypes.Values.Select(i => new FloatMenuOption(i.ToString(), () => _karmaType = i)).ToList()));
        }
    }

    public void Mutate(TableSettingsItem<EventItem> item)
    {
        item.Data.KarmaType = _karmaType;
    }
}