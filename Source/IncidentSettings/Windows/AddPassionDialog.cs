﻿using System;
using SirRandoo.ToolkitUtils.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings.Windows
{
    public class AddPassionDialog : Window
    {
        private string decreaseChanceBuffer;
        private string decreaseChanceDescription;
        private string decreaseChanceLabel;
        private string failChanceBuffer;
        private string failChanceDescription;
        private string failChanceLabel;
        private string hopChanceBuffer;
        private string hopChanceDescription;
        private string hopChanceLabel;
        private string randomnessDescription;

        private string randomnessLabel;

        public AddPassionDialog()
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
            decreaseChanceLabel = "TKUtils.Passion.DecreaseChance.Label".Localize();
            decreaseChanceDescription = "TKUtils.Passion.DecreaseChance.Description".Localize();
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.CheckboxLabeled(randomnessLabel, ref AddPassion.Randomness);
            listing.DrawDescription(randomnessDescription);

            (Rect failLabel, Rect failField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(failLabel, failChanceLabel);
            Widgets.TextFieldNumeric(failField, ref AddPassion.ChanceToFail, ref failChanceBuffer, max: 100f);
            listing.DrawDescription(failChanceDescription);

            (Rect hopLabel, Rect hopField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(hopLabel, hopChanceLabel);
            Widgets.TextFieldNumeric(hopField, ref AddPassion.ChanceToHop, ref hopChanceBuffer, max: 100f);
            listing.DrawDescription(hopChanceDescription);

            (Rect decreaseLabel, Rect decreaseField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(decreaseLabel, decreaseChanceLabel);
            Widgets.TextFieldNumeric(
                decreaseField,
                ref AddPassion.ChanceToDecrease,
                ref decreaseChanceBuffer,
                max: 100f
            );
            listing.DrawDescription(decreaseChanceDescription);

            listing.End();
        }
    }
}
