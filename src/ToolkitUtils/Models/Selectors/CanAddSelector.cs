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
using ToolkitUtils.Models.Serialization;
using ToolkitUtils.Models.Tables;
using ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Models.Selectors
{
    public class CanAddSelector : ISelectorBase<TraitItem>
    {
        private string _canAddText;
        private bool _state = true;

        public void Prepare()
        {
            _canAddText = Label;
        }

        public void Draw(Rect canvas)
        {
            if (UiHelper.LabeledPaintableCheckbox(canvas, _canAddText, ref _state))
            {
                Dirty.Set(true);
            }
        }

        public ObservableProperty<bool> Dirty { get; set; }

        public bool IsVisible([NotNull] TableSettingsItem<TraitItem> item) => item.Data.CanAdd == _state;

        public string Label => "TKUtils.Fields.CanAdd".TranslateSimple();
    }
}
