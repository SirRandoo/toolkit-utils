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
using SirRandoo.ToolkitUtils.IncidentSettings.Windows;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class AddPassion : IncidentHelperVariablesSettings
    {
        public static bool Randomness = true;
        public static int ChanceToFail = 20;
        public static int ChanceToHop = 10;
        public static int ChanceToDecrease = 5;

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
