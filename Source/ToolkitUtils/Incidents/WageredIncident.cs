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
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers;
using TwitchToolkit.Incidents;
using Verse;
using IncidentWorker_RaidEnemy = RimWorld.IncidentWorker_RaidEnemy;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class WageredIncident : IncidentVariablesBase
    {
        private static readonly Dictionary<string, IncidentData> Data;
        private static readonly List<string> Predators;
        private IncidentParms parms;
        private int wager;
        private IncidentWorker worker;

        static WageredIncident()
        {
            Predators = new List<string>
            {
                "Bear_Grizzly",
                "Bear_Polar",
                "Rhinoceros",
                "Elephant",
                "Megasloth",
                "Thrumbo"
            };
            Data = new Dictionary<string, IncidentData>
            {
                {
                    "Raid",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.ThreatSmall,
                        WorkerClass = typeof(IncidentWorker_RaidEnemy),
                        ExtraSetup = PrepareRaid
                    }
                },
                {
                    "DropRaid",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.ThreatSmall,
                        WorkerClass = typeof(IncidentWorker_RaidEnemy),
                        ExtraSetup = PrepareRaid
                    }
                },
                {
                    "SapperRaid",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.ThreatSmall,
                        WorkerClass = typeof(IncidentWorker_RaidEnemy),
                        ExtraSetup = PrepareRaid
                    }
                },
                {
                    "SiegeRaid",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.ThreatSmall,
                        WorkerClass = typeof(IncidentWorker_RaidEnemy),
                        ExtraSetup = PrepareRaid
                    }
                },
                {
                    "MechanoidRaid",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.ThreatSmall,
                        WorkerClass = typeof(IncidentWorker_RaidEnemy),
                        ExtraSetup = PrepareRaid
                    }
                },
                {
                    "Infestation",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.ThreatBig,
                        WorkerClass = typeof(IncidentWorker_Infestation),
                        ExtraSetup = (worker, parms, arg3) => worker.def = IncidentDef.Named("Infestation")
                    }
                },
                {
                    "ManhunterPack",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.ThreatSmall,
                        WorkerClass = typeof(IncidentWorker_ManhunterPack),
                        ExtraSetup = (worker, parms, arg3) => worker.def = RimWorld.IncidentDefOf.RaidEnemy
                    }
                },
                {
                    "Predators",
                    new IncidentData
                    {
                        Category = IncidentCategoryDefOf.ThreatSmall,
                        WorkerClass = typeof(AnimalSpawnWorker),
                        ExtraSetup = PreparePredators
                    }
                },
                {
                    "RandomDisease",
                    new IncidentData
                    {
                        CategoryResolver = GetDiseaseCategory, WorkerClass = typeof(IncidentWorker_DiseaseHuman)
                    }
                }
            };
        }

        private static IncidentCategoryDef GetDiseaseCategory([NotNull] IncidentWorker worker, StoreIncident incident)
        {
            IncidentDef disease = DefDatabase<IncidentDef>.AllDefs
               .Where(i => i.workerClass.IsAssignableFrom(typeof(IncidentWorker_DiseaseHuman)))
               .RandomElement();

            worker.def = disease;
            return disease.category;
        }

        private static void PreparePredators(IncidentWorker worker, IncidentParms parms, StoreIncident incident)
        {
            if (!(worker is AnimalSpawnWorker spawnWorker))
            {
                return;
            }

            if (!Predators.TryRandomElement(out string predator))
            {
                predator = "Thrumbo";
            }

            ThingDef thing = ThingDef.Named(predator);
            var power = 0.0f;

            if (thing?.race != null)
            {
                power += thing.tools.Sum(t => t.power);
                power /= thing.tools.Count;
            }

            spawnWorker.Quantity = power > 18f ? 2 : 3;
            spawnWorker.SpawnManhunter = true;
            spawnWorker.AnimalDef = PawnKindDef.Named(predator);
            spawnWorker.def = IncidentDef.Named("HerdMigration");
            spawnWorker.Label = "TwitchStoriesLetterLabelPredators".Localize();
            spawnWorker.Text = "ManhunterPackArrived".Localize(spawnWorker.AnimalDef.GetLabelPlural());
        }

        private static void PrepareRaid(
            [NotNull] IncidentWorker worker,
            [NotNull] IncidentParms parms,
            [NotNull] StoreIncident incident
        )
        {
            parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            worker.def = RimWorld.IncidentDefOf.RaidEnemy;
            parms.faction = Find.FactionManager.RandomEnemyFaction(
                minTechLevel: TechLevel.Industrial,
                allowNonHumanlike: false
            );
            parms.forced = true;

            switch (incident.defName)
            {
                case "Raid":
                    parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                    break;
                case "DropRaid":
                    parms.raidArrivalMode = PawnsArrivalModeDefOf.CenterDrop;
                    break;
                case "SapperRaid":
                    parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                    parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("ImmediateAttackSappers");
                    break;
                case "MechanoidRaid":
                    parms.raidArrivalMode = PawnsArrivalModeDefOf.RandomDrop;
                    parms.faction = Faction.OfMechanoids;
                    break;
                case "SiegeRaid":
                    parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("Siege");
                    parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                    break;
            }
        }

        public override bool CanHappen(string msg, Viewer viewer)
        {
            if (!Data.TryGetValue(storeIncident.defName, out IncidentData data))
            {
                return false;
            }

            string rawPoints = CommandFilter.Parse(msg).Skip(2).FirstOrDefault();

            if (rawPoints.NullOrEmpty()
                || !VariablesHelpers.PointsWagerIsValid(rawPoints, viewer, ref wager, ref storeIncident))
            {
                return false;
            }

            Map map = Find.RandomPlayerHomeMap;
            worker = Activator.CreateInstance(data.WorkerClass) as IncidentWorker;

            if (worker == null || map == null)
            {
                return false;
            }

            parms = StorytellerUtility.DefaultParmsNow(
                data.CategoryResolver != null ? data.CategoryResolver.Invoke(worker, storeIncident) : data.Category,
                map
            );

            parms.points = IncidentHelper_PointsHelper.RollProportionalGamePoints(storeIncident, wager, parms.points);
            parms.forced = true;
            data.ExtraSetup?.Invoke(worker, parms, storeIncident);

            return worker.CanFireNow(parms);
        }

        public override void Execute()
        {
            if (worker.TryExecute(parms))
            {
                Viewer.TakeViewerCoins(wager);
                Viewer.CalculateNewKarma(storeIncident.karmaType, wager);
                MessageHelper.ReplyToUser(
                    Viewer.username,
                    "TKUtils.Wagered.Complete".LocalizeKeyed(
                        storeIncident.label ?? storeIncident.abbreviation,
                        wager,
                        parms.points
                    )
                );
                return;
            }

            MessageHelper.ReplyToUser(
                Viewer.username,
                "TKUtils.FailedParms".LocalizeKeyed(storeIncident.label ?? storeIncident.defName)
            );
        }
    }
}
