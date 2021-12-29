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

using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class StuffMutator : IMutatorBase<ThingItem>
    {
        private bool state;
        private string stuffText;

        public int Priority => 1;

        public string Label => "TKUtils.Fields.CamBeStuff".TranslateSimple();

        public void Prepare()
        {
            stuffText = "TKUtils.Fields.CanBeStuff".TranslateSimple();
        }

        public void Mutate([NotNull] TableSettingsItem<ThingItem> item)
        {
            if (item.Data.Thing?.IsStuff != true)
            {
                return;
            }

            if (item.Data.ItemData != null)
            {
                item.Data.ItemData.IsStuffAllowed = state;
            }
        }

        public void Draw(Rect canvas)
        {
            SettingsHelper.LabeledPaintableCheckbox(canvas, stuffText, ref state);
        }
    }
}
