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
            qualityLabel = "TKUtils.Item.Quality.Label".Localize();
            researchLabel = "TKUtils.Item.Research.Label".Localize();
            stuffDescription = "TKUtils.Item.Stuff.Description".Localize();
            qualityDescription = "TKUtils.Item.Quality.Description".Localize();
            researchDescription = "TKUtils.Item.Research.Description".Localize();
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, inRect.width - 16f, Text.LineHeight * 15f);
            listing.BeginScrollView(inRect, ref scrollPos, ref viewPort);

            (Rect awfulLabel, Rect awfulField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(awfulLabel, "TKUtils.Item.AwfulMultiplier".Localize());
            Widgets.TextFieldNumeric(awfulField, ref Item.AwfulMultiplier, ref Item.AwfulMultiplierBuffer);

            (Rect poorLabel, Rect poorField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(poorLabel, "TKUtils.Item.PoorMultiplier".Localize());
            Widgets.TextFieldNumeric(poorField, ref Item.PoorMultiplier, ref Item.PoorMultiplierBuffer);

            (Rect normalLabel, Rect normalField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(normalLabel, "TKUtils.Item.NormalMultiplier".Localize());
            Widgets.TextFieldNumeric(normalField, ref Item.NormalMultiplier, ref Item.NormalMultiplierBuffer);

            (Rect goodLabel, Rect goodField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(goodLabel, "TKUtils.Item.GoodMultiplier".Localize());
            Widgets.TextFieldNumeric(goodField, ref Item.GoodMultiplier, ref Item.GoodMultiplierBuffer);

            (Rect excLabel, Rect excField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(excLabel, "TKUtils.Item.ExcellentMultiplier".Localize());
            Widgets.TextFieldNumeric(excField, ref Item.ExcellentMultiplier, ref Item.ExcellentMultiplierBuffer);

            (Rect mWorkLabel, Rect mWorkField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(mWorkLabel, "TKUtils.Item.MasterworkMultiplier".Localize());
            Widgets.TextFieldNumeric(mWorkField, ref Item.MasterworkMultiplier, ref Item.MasterworkMultiplierBuffer);

            (Rect legLabel, Rect legField) = listing.GetRectAsForm(0.7f);
            SettingsHelper.DrawLabel(legLabel, "TKUtils.Item.LegendaryMultiplier".Localize());
            Widgets.TextFieldNumeric(legField, ref Item.LegendaryMultiplier, ref Item.LegendaryMultiplierBuffer);

            listing.GapLine();

            listing.CheckboxLabeled(stuffLabel, ref Item.Stuff);
            listing.DrawDescription(stuffDescription);

            listing.CheckboxLabeled(qualityLabel, ref Item.Quality);
            listing.DrawDescription(qualityDescription);

            listing.CheckboxLabeled(researchLabel, ref BuyItemSettings.mustResearchFirst);
            listing.DrawDescription(researchDescription);

            listing.EndScrollView(ref viewPort);
        }
    }
}
