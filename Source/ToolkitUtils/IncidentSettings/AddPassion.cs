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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.IncidentSettings.Windows;
using SirRandoo.ToolkitUtils.Interfaces;
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

        private string decreaseChanceBuffer;
        private string failChanceBuffer;
        private string hopChanceBuffer;

        public int LineSpan => 4;

        public void Draw(Rect canvas, float preferredHeight)
        {
            var listing = new Listing_Standard();

            listing.Begin(canvas);

            listing.CheckboxLabeled(
                "TKUtils.Passion.Randomness.Label".Localize(),
                ref Randomness,
                "TKUtils.Passion.Randomness.Description".Localize()
            );

            (Rect failLabel, Rect failField) = listing.GetRect(preferredHeight).ToForm();
            Widgets.Label(failLabel, "TKUtils.Passion.FailChance.Label".Localize());
            Widgets.TextFieldNumeric(failField, ref ChanceToFail, ref failChanceBuffer, max: 100f);
            failLabel.TipRegion("TKUtils.Passion.FailChance.Description".Localize());

            (Rect hopLabel, Rect hopField) = listing.GetRect(preferredHeight).ToForm();
            Widgets.Label(hopLabel, "TKUtils.Passion.HopChance.Label".Localize());
            Widgets.TextFieldNumeric(hopField, ref ChanceToHop, ref hopChanceBuffer, max: 100f);
            hopLabel.TipRegion("TKUtils.Passion.HopChance.Description".Localize());

            (Rect decreaseLabel, Rect decreaseField) = listing.GetRect(preferredHeight).ToForm();
            Widgets.Label(decreaseLabel, "TKUtils.Passion.DecreaseChance.Label".Localize());
            Widgets.TextFieldNumeric(decreaseField, ref ChanceToDecrease, ref decreaseChanceBuffer, max: 100f);
            decreaseLabel.TipRegion("TKUtils.Passion.DecreaseChance.Description".Localize());

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
            Find.WindowStack.Add(new AddPassionDialog());
        }
    }
}
