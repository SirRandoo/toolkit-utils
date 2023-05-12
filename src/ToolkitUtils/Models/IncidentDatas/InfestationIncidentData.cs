﻿// ToolkitUtils
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
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.IncidentSettings;
using ToolkitUtils.Interfaces;
using TwitchToolkit.Incidents;

namespace ToolkitUtils.Models.IncidentDatas
{
    public class InfestationIncidentData : IWageredIncidentData
    {
        public bool UseStoryteller => Infestation.Storyteller;
        [NotNull] public Type WorkerClass => typeof(IncidentWorker_Infestation);

        public IncidentCategoryDef ResolveCategory(IncidentWorker worker, StoreIncident incident) => IncidentCategoryDefOf.ThreatBig;

        public void DoExtraSetup([NotNull] IncidentWorker worker, IncidentParms @params, StoreIncident incident)
        {
            worker.def = IncidentDef.Named("Infestation");
        }
    }
}
