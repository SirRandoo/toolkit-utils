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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class EventKarmaSelector : ISelectorBase<EventItem>
    {
        private string _karmaLabel;
        private KarmaType _karmaType = KarmaType.Neutral;
        private List<FloatMenuOption> _karmaTypes;

        public ObservableProperty<bool> Dirty { get; set; }

        public void Prepare()
        {
            _karmaLabel = Label;
            _karmaTypes = Data.KarmaTypes.Values.Select(i => new FloatMenuOption(i.ToString(), () => SetKarma(i))).ToList();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.Split(0.75f);
            UiHelper.Label(label, _karmaLabel);

            if (Widgets.ButtonText(field, _karmaType.ToString()))
            {
                Find.WindowStack.Add(new FloatMenu(_karmaTypes));
            }
        }

        public bool IsVisible([NotNull] TableSettingsItem<EventItem> item) => item.Data.KarmaType == _karmaType;

        public string Label => "TKUtils.Fields.KarmaType".TranslateSimple();

        private void SetKarma(KarmaType karma)
        {
            _karmaType = karma;
            Dirty.Set(true);
        }
    }
}
