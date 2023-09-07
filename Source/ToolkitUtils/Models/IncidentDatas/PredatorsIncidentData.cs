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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.IncidentSettings;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Workers;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.Models.IncidentDatas;

public class PredatorsIncidentData : IWageredIncidentData
{
    private readonly List<string> _predators;

    public PredatorsIncidentData()
    {
        _predators = new List<string>
        {
            "Bear_Grizzly",
            "Bear_Polar",
            "Rhinoceros",
            "Elephant",
            "Megasloth",
            "Thrumbo"
        };
    }

    public bool UseStoryteller => Predators.Storyteller;
    public Type WorkerClass => typeof(AnimalSpawnWorker);

    public IncidentCategoryDef ResolveCategory(IncidentWorker worker, StoreIncident incident) => IncidentCategoryDefOf.ThreatSmall;

    public void DoExtraSetup(IncidentWorker worker, IncidentParms @params, StoreIncident incident)
    {
        if (!(worker is AnimalSpawnWorker spawnWorker))
        {
            return;
        }

        if (!_predators.TryRandomElement(out string predator))
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
        spawnWorker.Text = "ManhunterPackArrived".LocalizeKeyed(spawnWorker.AnimalDef.GetLabelPlural());
    }
}