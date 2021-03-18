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
using MoonSharp.Interpreter;
using TwitchToolkit.Incidents;

namespace SirRandoo.ToolkitUtils.Proxies
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class StoreIncidentProxy
    {
        private readonly StoreIncident storeIncident;
        private readonly StoreIncidentVariables storeIncidentVariables;

        [MoonSharpHidden]
        public StoreIncidentProxy(StoreIncident storeIncident)
        {
            this.storeIncident = storeIncident;
            storeIncidentVariables = storeIncident as StoreIncidentVariables;
        }

        public int Cost => storeIncident.cost;
        public int EventCap => storeIncident.eventCap;
        [NotNull] public string KarmaType => storeIncident.karmaType.ToString();
        public string Abbreviation => storeIncident.abbreviation;
        public string Abr => storeIncident.abbreviation;

        public bool IsVariables => storeIncidentVariables != null;
        public int MinPointsToFire => storeIncidentVariables?.minPointsToFire ?? -1;
        public int MaxWager => storeIncidentVariables?.maxWager ?? -1;
    }
}
