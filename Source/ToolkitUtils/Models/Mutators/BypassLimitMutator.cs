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
using UnityEngine;

namespace SirRandoo.ToolkitUtils.Models
{
    public class BypassLimitMutator : IMutatorBase<TraitItem>
    {
        private string bypassLimitText;
        private bool state;

        public void Prepare()
        {
            bypassLimitText = "TKUtils.Fields.BypassTraitLimit".Localize();
        }

        public void Mutate(TableSettingsItem<TraitItem> item)
        {
            item.Data.TraitData.CanBypassLimit = state;
        }

        public void Draw(Rect canvas)
        {
            SettingsHelper.LabeledPaintableCheckbox(canvas, bypassLimitText, ref state);
        }
    }
}
