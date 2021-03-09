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

using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models.Tables;
using UnityEngine;

namespace SirRandoo.ToolkitUtils.Models
{
    public class StateSelector : SelectorBase<ShopItemBase<ShopDataBase>>
    {
        private bool state = true;
        private string stateText;

        public override void Prepare()
        {
            stateText = "TKUtils.Fields.State".Localize();
        }

        public override void Draw(Rect canvas)
        {
            if (SettingsHelper.LabeledPaintableCheckbox(canvas, stateText, ref state))
            {
                Dirty = true;
            }
        }

        public override bool IsVisible(TableItem<ShopItemBase<ShopDataBase>> item)
        {
            return item.Data.Enabled == state;
        }
    }
}
