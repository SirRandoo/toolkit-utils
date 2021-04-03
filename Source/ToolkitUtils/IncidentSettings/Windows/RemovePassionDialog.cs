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
using SirRandoo.ToolkitUtils.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings.Windows
{
    public class RemovePassionDialog : Window
    {
        private string failChanceBuffer;
        private string failChanceDescription;
        private string failChanceLabel;
        private string hopChanceBuffer;
        private string hopChanceDescription;
        private string hopChanceLabel;
        private string increaseChanceBuffer;
        private string increaseChanceDescription;
        private string increaseChanceLabel;
        private string randomnessDescription;

        private string randomnessLabel;

        public RemovePassionDialog()
        {
            doCloseButton = true;
        }

        public override void PreOpen()
        {
            base.PreOpen();

            randomnessLabel = "TKUtils.Passion.Randomness.Label".Localize();
            randomnessDescription = "TKUtils.Passion.Randomness.Description".Localize();
            failChanceLabel = "TKUtils.Passion.FailChance.Label".Localize();
            failChanceDescription = "TKUtils.Passion.FailChance.Description".Localize();
            hopChanceLabel = "TKUtils.Passion.HopChance.Label".Localize();
            hopChanceDescription = "TKUtils.Passion.HopChance.Description".Localize();
            increaseChanceLabel = "TKUtils.Passion.DecreaseChance.Label".Localize();
            increaseChanceDescription = "TKUtils.Passion.IncreaseChance.Description".Localize();
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.CheckboxLabeled(randomnessLabel, ref RemovePassion.Randomness);
            listing.DrawDescription(randomnessDescription);

            (Rect failLabel, Rect failField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(failLabel, failChanceLabel);
            Widgets.TextFieldNumeric(failField, ref RemovePassion.ChanceToFail, ref failChanceBuffer, max: 100f);
            listing.DrawDescription(failChanceDescription);

            (Rect hopLabel, Rect hopField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(hopLabel, hopChanceLabel);
            Widgets.TextFieldNumeric(hopField, ref RemovePassion.ChanceToHop, ref hopChanceBuffer, max: 100f);
            listing.DrawDescription(hopChanceDescription);

            (Rect increaseLabel, Rect increaseField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(increaseLabel, increaseChanceLabel);
            Widgets.TextFieldNumeric(
                increaseField,
                ref RemovePassion.ChanceToIncrease,
                ref increaseChanceBuffer,
                max: 100f
            );
            listing.DrawDescription(increaseChanceDescription);

            listing.End();
        }
    }
}
