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
using JetBrains.Annotations;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Incidents;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class AddPassion : IncidentHelperVariablesSettings, IEventSettings
    {
        public static bool Randomness = true;
        public static int ChanceToFail = 20;
        public static int ChanceToHop = 10;
        public static int ChanceToDecrease = 5;

        private string _decreaseChanceBuffer;
        private string _failChanceBuffer;
        private string _hopChanceBuffer;

        public int LineSpan => 4;

        public void Draw(Rect canvas, float preferredHeight)
        {
            var listing = new Listing_Standard();

            listing.Begin(canvas);

            listing.CheckboxLabeled("TKUtils.Passion.Randomness.Label".TranslateSimple(), ref Randomness, "TKUtils.Passion.Randomness.Description".TranslateSimple());

            (Rect failLabel, Rect failField) = listing.GetRect(preferredHeight).Split();
            UiHelper.Label(failLabel, "TKUtils.Passion.FailChance.Label".TranslateSimple());
            Widgets.TextFieldNumeric(failField, ref ChanceToFail, ref _failChanceBuffer, max: 100f);
            failLabel.TipRegion("TKUtils.Passion.FailChance.Description".TranslateSimple());

            (Rect hopLabel, Rect hopField) = listing.GetRect(preferredHeight).Split();
            UiHelper.Label(hopLabel, "TKUtils.Passion.HopChance.Label".TranslateSimple());
            Widgets.TextFieldNumeric(hopField, ref ChanceToHop, ref _hopChanceBuffer, max: 100f);
            hopLabel.TipRegion("TKUtils.Passion.HopChance.Description".TranslateSimple());

            (Rect decreaseLabel, Rect decreaseField) = listing.GetRect(preferredHeight).Split();
            UiHelper.Label(decreaseLabel, "TKUtils.Passion.DecreaseChance.Label".TranslateSimple());
            Widgets.TextFieldNumeric(decreaseField, ref ChanceToDecrease, ref _decreaseChanceBuffer, max: 100f);
            decreaseLabel.TipRegion("TKUtils.Passion.DecreaseChance.Description".TranslateSimple());

            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Randomness, "addPassionRandomness", true);
            Scribe_Values.Look(ref ChanceToFail, "addPassionFailChance", 20);
            Scribe_Values.Look(ref ChanceToHop, "addPassionHopChance", 10);
            Scribe_Values.Look(ref ChanceToDecrease, "addPassionDecreaseChance", 5);
        }

        public override void EditSettings()
        {
            Find.WindowStack.Add(new EventSettingsDialog(this));
        }
    }
}
