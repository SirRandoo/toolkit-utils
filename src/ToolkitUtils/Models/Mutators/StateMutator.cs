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
using SirRandoo.CommonLib.Helpers;
using ToolkitUtils.Interfaces;
using ToolkitUtils.Models.Tables;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Models.Mutators
{
    public class StateMutator<T> : IMutatorBase<T> where T : class, IShopItemBase
    {
        private bool _state;
        private string _stateText;

        public int Priority => 1;

        public string Label => "TKUtils.Fields.State".TranslateSimple();

        public void Prepare()
        {
            _stateText = Label;
        }

        public void Mutate([NotNull] TableSettingsItem<T> item)
        {
            item.Data.Enabled = _state;
        }

        public void Draw(Rect canvas)
        {
            UiHelper.LabeledPaintableCheckbox(canvas, _stateText, ref _state);
        }
    }
}
