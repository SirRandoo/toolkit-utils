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

using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.CommandSettings
{
    public class PawnGear : ICommandSettings
    {
        public void Draw(Rect region)
        {
            var listing = new Listing_Standard();

            listing.Begin(region);

            listing.CheckboxLabeled("TKUtils.PawnGear.Temperature.Label".TranslateSimple(), ref TkSettings.TempInGear);
            listing.DrawDescription("TKUtils.PawnGear.Temperature.Description".TranslateSimple());

            listing.CheckboxLabeled("TKUtils.PawnGear.Apparel.Label".TranslateSimple(), ref TkSettings.ShowApparel);
            listing.DrawDescription("TKUtils.PawnGear.Apparel.Description".TranslateSimple());

            listing.CheckboxLabeled("TKUtils.PawnGear.Armor.Label".TranslateSimple(), ref TkSettings.ShowArmor);
            listing.DrawDescription("TKUtils.PawnGear.Armor.Description".TranslateSimple());

            listing.CheckboxLabeled("TKUtils.PawnGear.Weapon.Label".TranslateSimple(), ref TkSettings.ShowWeapon);
            listing.DrawDescription("TKUtils.PawnGear.Weapon.Description".TranslateSimple());

            listing.End();
        }

        public void Save()
        {
            TkUtils.Instance.WriteSettings();
        }
    }
}
