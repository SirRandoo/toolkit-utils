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
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings.Windows
{
    public class BuyItemDialog : Window
    {
        private string genderDescription;
        private string genderLabel;
        private string qualityDescription;
        private string qualityLabel;
        private string researchDescription;
        private string researchLabel;

        private Vector2 scrollPos = Vector2.zero;
        private string stuffDescription;
        private string stuffLabel;

        public BuyItemDialog()
        {
            doCloseButton = true;
        }

        public override void PreOpen()
        {
            base.PreOpen();

            stuffLabel = "TKUtils.Item.Stuff.Label".Localize();
            genderLabel = "TKUtils.Item.Gender.Label".Localize();
            qualityLabel = "TKUtils.Item.Quality.Label".Localize();
            researchLabel = "TKUtils.Item.Research.Label".Localize();
            stuffDescription = "TKUtils.Item.Stuff.Description".Localize();
            genderDescription = "TKUtils.Item.Gender.Description".Localize();
            qualityDescription = "TKUtils.Item.Quality.Description".Localize();
            researchDescription = "TKUtils.Item.Research.Description".Localize();
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, inRect.width - 16f, Text.LineHeight * 19f);
            Widgets.BeginScrollView(inRect, ref scrollPos, viewPort);
            listing.Begin(viewPort);

            (Rect awfulLabel, Rect awfulField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(awfulLabel, "TKUtils.Item.AwfulMultiplier".Localize());
            Widgets.TextFieldNumeric(awfulField, ref Item.AwfulMultiplier, ref Item.AwfulMultiplierBuffer);

            if (SettingsHelper.DrawFieldButton(
                awfulLabel,
                Item.AwfulQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                Item.AwfulQuality = !Item.AwfulQuality;
            }

            (Rect poorLabel, Rect poorField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(poorLabel, "TKUtils.Item.PoorMultiplier".Localize());
            Widgets.TextFieldNumeric(poorField, ref Item.PoorMultiplier, ref Item.PoorMultiplierBuffer);

            if (SettingsHelper.DrawFieldButton(
                poorLabel,
                Item.PoorQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                Item.PoorQuality = !Item.PoorQuality;
            }

            (Rect normalLabel, Rect normalField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(normalLabel, "TKUtils.Item.NormalMultiplier".Localize());
            Widgets.TextFieldNumeric(normalField, ref Item.NormalMultiplier, ref Item.NormalMultiplierBuffer);

            if (SettingsHelper.DrawFieldButton(
                normalLabel,
                Item.NormalQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                Item.NormalQuality = !Item.NormalQuality;
            }

            (Rect goodLabel, Rect goodField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(goodLabel, "TKUtils.Item.GoodMultiplier".Localize());
            Widgets.TextFieldNumeric(goodField, ref Item.GoodMultiplier, ref Item.GoodMultiplierBuffer);

            if (SettingsHelper.DrawFieldButton(
                goodLabel,
                Item.GoodQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                Item.GoodQuality = !Item.GoodQuality;
            }

            (Rect excLabel, Rect excField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(excLabel, "TKUtils.Item.ExcellentMultiplier".Localize());
            Widgets.TextFieldNumeric(excField, ref Item.ExcellentMultiplier, ref Item.ExcellentMultiplierBuffer);


            if (SettingsHelper.DrawFieldButton(
                excLabel,
                Item.ExcellentQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                Item.ExcellentQuality = !Item.ExcellentQuality;
            }

            (Rect mWorkLabel, Rect mWorkField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(mWorkLabel, "TKUtils.Item.MasterworkMultiplier".Localize());
            Widgets.TextFieldNumeric(mWorkField, ref Item.MasterworkMultiplier, ref Item.MasterworkMultiplierBuffer);


            if (SettingsHelper.DrawFieldButton(
                mWorkLabel,
                Item.MasterworkQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                Item.MasterworkQuality = !Item.MasterworkQuality;
            }

            (Rect legLabel, Rect legField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(legLabel, "TKUtils.Item.LegendaryMultiplier".Localize());
            Widgets.TextFieldNumeric(legField, ref Item.LegendaryMultiplier, ref Item.LegendaryMultiplierBuffer);


            if (SettingsHelper.DrawFieldButton(
                legLabel,
                Item.LegendaryQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                Item.LegendaryQuality = !Item.LegendaryQuality;
            }

            listing.CheckboxLabeled(stuffLabel, ref Item.Stuff);
            listing.DrawDescription(stuffDescription);

            listing.CheckboxLabeled(qualityLabel, ref Item.Quality);
            listing.DrawDescription(qualityDescription);

            listing.CheckboxLabeled(genderLabel, ref Item.Gender);
            listing.DrawDescription(genderDescription);

            listing.CheckboxLabeled(researchLabel, ref BuyItemSettings.mustResearchFirst);
            listing.DrawDescription(researchDescription);

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
