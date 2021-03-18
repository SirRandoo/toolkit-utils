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

using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using TorannMagic;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.TMagic
{
    public class ClassSelector : ISelectorBase<TraitItem>
    {
        private string classText;
        private bool state = true;

        public ObservableProperty<bool> Dirty { get; set; }

        public void Prepare()
        {
            classText = "TKUtils.Fields.Class".Localize();
        }

        public void Draw(Rect canvas)
        {
            if (SettingsHelper.LabeledPaintableCheckbox(canvas, classText, ref state))
            {
                Dirty.Set(true);
            }
        }

        public bool IsVisible([NotNull] TableSettingsItem<TraitItem> item)
        {
            TraitDef traitDef = item.Data.TraitDef;

            if (traitDef == null)
            {
                return false;
            }

            if (traitDef.Equals(TorannMagicDefOf.DeathKnight))
            {
                return state;
            }

            return !TM_Data.AllClassTraits.Any(i => i.Equals(traitDef)) || state;
        }
    }
}
