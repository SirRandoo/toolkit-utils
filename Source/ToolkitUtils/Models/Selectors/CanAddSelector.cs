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
    public class CanAddSelector : SelectorBase<TraitItem>
    {
        private string canAddText;
        private bool state = true;

        public override void Prepare()
        {
            canAddText = "TKUtils.Fields.CanAdd".Localize();
        }

        public override void Draw(Rect canvas)
        {
            if (SettingsHelper.LabeledPaintableCheckbox(canvas, canAddText, ref state))
            {
                Dirty = true;
            }
        }

        public override bool IsVisible(TableItem<TraitItem> item)
        {
            return item.Data.CanAdd == state;
        }
    }
}
