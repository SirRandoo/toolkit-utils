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
using TwitchToolkit.Incidents;
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
                    "WildManWandersIn",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_WildManWandersIn),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("WildManWandersIn")
                    }
                },
                {
                    "WandererJoins",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_WandererJoin),
                        ExtraSetup = (worker, parms, arg3) => worker.def = RimWorld.IncidentDefOf.WandererJoin
                    }
                },
                {
                    "VisitorGroup",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_VisitorGroup),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("VisitorGroup")
                    }
                },
                {
                    "TravelerGroup",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_TravelerGroup),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("TravelerGroup")
                    }
                },
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
                    "ThrumboPasses",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_ThrumboPasses),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("ThrumboPasses")
                    }
                },
                {
                    "ShipPartPsychic",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_CrashedShipPart),
                        ExtraSetup = (worker, parms, arg3) =>
                            worker.def = IncidentDef.Named("PsychicEmanatorShipPartCrash")
                    }
                },
                {
                    "ShipPartPoison",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_CrashedShipPart),
                        ExtraSetup = (worker, parms, arg3) =>
                            worker.def = IncidentDef.Named("DefoliatorShipPartCrash")
                    }
                },
                {
                    "ShipChunkDrop",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc, WorkerClass = typeof(IncidentWorker_ShipChunkDrop)
                    }
                },
                {
                    "ResourcePodCrash",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_ResourcePodCrash),
                        ExtraSetup = (worker, parms, arg3) => worker.def = RimWorld.IncidentDefOf.ShipChunkDrop
                    }
                },
                {
                    "PsychicSoothe",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_PsychicSoothe),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("PsychicSoothe")
                    }
                },
                {
                    "PsychicDrone",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_PsychicDrone),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("PsychicDrone")
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
                },
                {
                    "Meteorite",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_MeteoriteImpact),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("MeteoriteImpact")
                    }
                },
                {
                    "ManInBlack",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_WandererJoin),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("StrangerInBlackJoin")
                    }
                },
                {
                    "MadAnimal",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_AnimalInsanitySingle),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("AnimalInsanitySingle")
                    }
                },
                {
                    "HerdMigration",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_HerdMigration),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("HerdMigration")
                    }
                },
                {
                    "FarmAnimalsWanderIn",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_FarmAnimalsWanderIn),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("FarmAnimalsWanderIn")
                    }
                },
                {
                    "Blight",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_CropBlight),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("CropBlight")
                    }
                },
                {
                    "AmbrosiaSprout",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_AmbrosiaSprout),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("AmbrosiaSprout")
                    }
                },
                {
                    "Aurora",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_Aurora),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("Aurora")
                    }
                },
                {
                    "HeatWave",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_MakeGameCondition),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("HeatWave")
                    }
                },
                {
                    "ColdSnap",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_MakeGameCondition),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("ColdSnap")
                    }
                },
                {
                    "Eclipse",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_MakeGameCondition),
                        ExtraSetup = (worker, parms, arg3) => worker.def = RimWorld.IncidentDefOf.Eclipse
                    }
                },
                {
                    "ShortCircuit",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_ShortCircuit),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("ShortCircuit")
                    }
                },
                {
                    "SolarFlare",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_MakeGameCondition),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("SolarFlare")
                    }
                },
                {
                    "ToxicFallout",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_MakeGameCondition),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("ToxicFallout")
                    }
                },
                {
                    "VolcanicWinter",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_MakeGameCondition),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("VolcanicWinter")
                    }
                },
                {
                    "AnimalTame",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_SelfTame),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("SelfTame")
                    }
                },
                {
                    "Alphabeavers",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.Misc,
                        WorkerClass = typeof(IncidentWorker_Alphabeavers),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("Alphabeavers")
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
