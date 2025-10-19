// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Interactions.Incidents. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using TwitchToolkit.Store;
using Verse;

namespace ToolkitUtils.Interactions.Incidents;

[UsedImplicitly]
public class Incident : IncidentHelper
{
    private static readonly Dictionary<string, IIncidentData> Data = new()
    {
        {
            "TraderCaravanArrival", new TraderCaravanIncidentData()
        },
        {
            "OrbitalTraderArrival", new OrbitalTraderIncidentData()
        },
    };

    private IncidentParms _params;
    private IncidentWorker _worker;

    public override bool IsPossible()
    {
        if (!Data.TryGetValue(storeIncident.defName, out IIncidentData data)) return false;

        _worker = Activator.CreateInstance(data.WorkerClass) as IncidentWorker;
        Map map = Find.RandomPlayerHomeMap;

        if (map == null || _worker == null) return false;

        _params = StorytellerUtility.DefaultParmsNow(data.ResolveCategory(_worker, storeIncident), map);
        _params.forced = true;

        data.DoExtraSetup(_worker, _params, storeIncident);

        return _worker.CanFireNow(_params);
    }

    public override void TryExecute()
    {
        _worker.TryExecute(_params);
    }
}
