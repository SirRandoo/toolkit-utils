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
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class RandomDiseaseIncidentData : IWageredIncidentData
    {
        public bool UseStoryteller { get; }
        [NotNull] public Type WorkerClass => typeof(IncidentWorker_DiseaseHuman);

        public IncidentCategoryDef ResolveCategory([NotNull] IncidentWorker worker, StoreIncident incident)
        {
            worker.def = DefDatabase<IncidentDef>.AllDefs
               .Where(i => i.workerClass.IsAssignableFrom(typeof(IncidentWorker_DiseaseHuman)))
               .RandomElement();
            return worker.def.category;
        }

        public void DoExtraSetup(IncidentWorker worker, IncidentParms parms, StoreIncident incident) { }
    }
}
