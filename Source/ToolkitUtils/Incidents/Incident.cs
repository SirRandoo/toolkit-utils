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
using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class Incident : IncidentHelper
    {
        private static readonly Dictionary<string, IncidentData> Data;
        private IncidentParms parms;
        private IncidentWorker worker;

        static Incident()
        {
            Data = new Dictionary<string, IncidentData>
            {
                {
                    "TraderCaravanArrival",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_TraderCaravanArrival),
                        ExtraSetup = (worker, parms, arg3) =>
                            worker.def = RimWorld.IncidentDefOf.TraderCaravanArrival
                    }
                },
                {
                    "OrbitalTraderArrival",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_OrbitalTraderArrival),
                        ExtraSetup = (worker, parms, arg3) =>
                            worker.def = RimWorld.IncidentDefOf.OrbitalTraderArrival
                    }
                }
            };
        }

        public override bool IsPossible()
        {
            if (!Data.TryGetValue(storeIncident.defName, out IncidentData data))
            {
                return false;
            }

            worker = Activator.CreateInstance(data.WorkerClass) as IncidentWorker;
            Map map = Find.RandomPlayerHomeMap;

            if (map == null)
            {
                return false;
            }

            parms = StorytellerUtility.DefaultParmsNow(data.Category, map);
            parms.forced = true;

            data.ExtraSetup?.Invoke(worker, parms, storeIncident);

            return worker?.CanFireNow(parms) == true;
        }

        public override void TryExecute()
        {
            worker.TryExecute(parms);
        }
    }
}
