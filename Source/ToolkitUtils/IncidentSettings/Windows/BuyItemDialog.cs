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
            listing.Begin(inRect);

            listing.CheckboxLabeled(stuffLabel, ref Item.Stuff);
            listing.DrawDescription(stuffDescription);

            listing.CheckboxLabeled(qualityLabel, ref Item.Quality);
            listing.DrawDescription(qualityDescription);

            listing.CheckboxLabeled(researchLabel, ref BuyItemSettings.mustResearchFirst);
            listing.DrawDescription(researchDescription);

            listing.End();
        }
    }
}
