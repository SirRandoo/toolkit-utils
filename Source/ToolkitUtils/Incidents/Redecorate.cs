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

using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    public class Redecorate : IncidentVariablesBase
    {
        public override bool CanHappen(string msg, Viewer viewer)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute()
        {
            var pawn = new Pawn();
            Room room = pawn.ownership.OwnedBed.GetRoom();
            foreach (IntVec3 cell in room.BorderCells)
            {
                var building = room.Map.thingGrid.ThingAt<Building>(cell);

                // GenConstruct.PlaceBlueprintForBuild_NewTemp()
            }
        }
    }
}
