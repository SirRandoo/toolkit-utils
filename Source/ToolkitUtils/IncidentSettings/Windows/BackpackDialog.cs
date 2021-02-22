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
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings.Windows
{
    public class BackpackDialog : Window
    {
        private string autoEquipDescription;
        private string autoEquipLabel;

        public BackpackDialog()
        {
            doCloseButton = true;
        }

        public override void PreOpen()
        {
            base.PreOpen();

            autoEquipLabel = "TKUtils.Backpack.AutoEquip.Label".Localize();
            autoEquipDescription = "TKUtils.Backpack.AutoEquip.Description".Localize();
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();

            listing.Begin(inRect);

            listing.CheckboxLabeled(autoEquipLabel, ref Backpack.AutoEquip);
            listing.DrawDescription(autoEquipDescription);

            listing.End();
        }
    }
}
