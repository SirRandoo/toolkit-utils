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

using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models.Tables;
using UnityEngine;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ItemStateMutator : IMutatorBase<ThingItem>
    {
        private bool state;
        private string stateText;

        public void Prepare()
        {
            stateText = "TKUtils.Fields.State".Localize();
        }

        public void Draw(Rect canvas)
        {
            SettingsHelper.LabeledPaintableCheckbox(canvas, stateText, ref state);
        }

        public void Mutate(TableItem<ThingItem> item)
        {
            switch (state)
            {
                case true when !item.Data.Enabled:
                    item.Data.Item.price = item.Data.Thing.CalculateStorePrice();
                    break;
                case false when item.Data.Enabled:
                    item.Data.Item.price = -10;
                    break;
            }
        }
    }
}
