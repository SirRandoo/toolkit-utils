// MIT License
// 
// Copyright (c) 2021 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class EventKarmaSelector : ISelectorBase<EventItem>
    {
        private string karmaLabel;
        private KarmaType karmaType = KarmaType.Neutral;

        public ObservableProperty<bool> Dirty { get; set; }

        public void Prepare()
        {
            karmaLabel = "TKUtils.Fields.KarmaType".Localize();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, karmaLabel);

            if (Widgets.ButtonText(field, karmaType.ToString()))
            {
                Find.WindowStack.Add(
                    new FloatMenu(
                        Data.KarmaTypes.Select(i => new FloatMenuOption(i.ToString(), () => karmaType = i)).ToList()
                    )
                );
            }
        }

        public bool IsVisible(TableSettingsItem<EventItem> item)
        {
            return item.Data.KarmaType == karmaType;
        }
    }
}
