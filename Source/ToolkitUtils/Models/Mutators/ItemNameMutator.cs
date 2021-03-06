﻿// ToolkitUtils
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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ItemNameMutator : IMutatorBase<ThingItem>
    {
        private string name = "";
        private string nameText;

        public int Priority => 1;

        public void Prepare()
        {
            nameText = "TKUtils.Fields.Name".Localize();
        }

        public void Mutate(TableSettingsItem<ThingItem> item)
        {
            if (name.NullOrEmpty())
            {
                return;
            }

            item.Data.ItemData.CustomName = name;
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, nameText);
            name = Widgets.TextField(field, name).ToToolkit();
        }
    }
}
