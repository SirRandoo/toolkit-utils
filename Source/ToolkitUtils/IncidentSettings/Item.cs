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

using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.IncidentSettings.Windows;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.Incidents;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings
{
    [UsedImplicitly]
    public class Item : IncidentHelperVariablesSettings, IEventSettings
    {
        public static bool Stuff = true;
        public static bool Quality = true;

        public int LineSpan => 3;

        public void Draw(Rect canvas, float preferredHeight)
        {
            var listing = new Listing_Standard();
            listing.Begin(canvas);

            listing.CheckboxLabeled(
                "TKUtils.Item.Stuff.Label".Localize(),
                ref Stuff,
                "TKUtils.Item.Stuff.Description".Localize()
            );
            listing.CheckboxLabeled(
                "TKUtils.Item.Quality.Label".Localize(),
                ref Quality,
                "TKUtils.Item.Quality.Description".Localize()
            );
            listing.CheckboxLabeled(
                "TKUtils.Item.Research.Label".Localize(),
                ref BuyItemSettings.mustResearchFirst,
                "TKUtils.Item.Research.Description".Localize()
            );

            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Stuff, "buyItemMaterial", true);
            Scribe_Values.Look(ref Quality, "buyItemQuality", true);
            Scribe_Values.Look(ref BuyItemSettings.mustResearchFirst, "BuyItemSettings.mustResearchFirst", true);
        }

        public override void EditSettings()
        {
            Find.WindowStack.Add(new BuyItemDialog());
        }
    }
}
