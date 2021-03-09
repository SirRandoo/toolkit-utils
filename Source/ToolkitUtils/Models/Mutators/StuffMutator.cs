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
    public class StuffMutator : MutatorBase<ThingItem>
    {
        private bool state;
        private string stuffText;

        public override void Prepare()
        {
            stuffText = "TKUtils.Fields.CanBeStuff".Localize();
        }

        public override void Mutate(TableItem<ThingItem> item)
        {
            if (!item.Data.Thing?.IsStuff ?? true)
            {
                return;
            }

            item.Data.Data.IsStuffAllowed = state;
        }

        public override void Draw(Rect canvas)
        {
            SettingsHelper.LabeledPaintableCheckbox(canvas, stuffText, ref state);
        }
    }
}
