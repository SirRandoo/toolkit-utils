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
using SirRandoo.ToolkitUtils.IncidentSettings;
using TwitchToolkit.Incidents;

namespace SirRandoo.ToolkitUtils.Models
{
    public class MechanoidRaidIncidentData : RaidIncidentData
    {
        public override bool UseStoryteller => MechanoidRaid.Storyteller;

        public override void DoExtraSetup(IncidentWorker worker, IncidentParms @params, StoreIncident incident)
        {
            base.DoExtraSetup(worker, @params, incident);
            @params.raidArrivalMode = PawnsArrivalModeDefOf.RandomDrop;
            @params.faction = Faction.OfMechanoids;
        }
    }
}
