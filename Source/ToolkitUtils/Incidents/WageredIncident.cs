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
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class WageredIncident : IncidentVariablesBase
    {
        private static readonly Dictionary<string, IWageredIncidentData> Data;
        private IWageredIncidentData data;
        private IncidentParms parms;
        private int wager;
        private IncidentWorker worker;

        static WageredIncident()
        {
            Data = new Dictionary<string, IWageredIncidentData>
            {
                { "Raid", new RaidIncidentData() },
                { "DropRaid", new DropRaidIncidentData() },
                { "SapperRaid", new SapperRaidIncidentData() },
                { "SiegeRaid", new SiegeRaidIncidentData() },
                { "MechanoidRaid", new MechanoidRaidIncidentData() },
                { "Infestation", new InfestationIncidentData() },
                { "ManhunterPack", new ManhunterPackIncidentData() },
                { "Predators", new PredatorsIncidentData() },
                { "RandomDisease", new RandomDiseaseIncidentData() }
            };
        }

        public override bool CanHappen(string msg, Viewer viewer)
        {
            if (!Data.TryGetValue(storeIncident.defName, out data))
            {
                return false;
            }

            if (!data.UseStoryteller)
            {
                string rawPoints = CommandFilter.Parse(msg).Skip(2).FirstOrDefault();

                if (rawPoints.NullOrEmpty() || !VariablesHelpers.PointsWagerIsValid(rawPoints, viewer, ref wager, ref storeIncident))
                {
                    return false;
                }
            }

            Map map = Find.RandomPlayerHomeMap;
            worker = Activator.CreateInstance(data.WorkerClass) as IncidentWorker;

            if (worker == null || map == null)
            {
                return false;
            }

            parms = StorytellerUtility.DefaultParmsNow(data.ResolveCategory(worker, storeIncident), map);

            if (!data.UseStoryteller)
            {
                parms.points = IncidentHelper_PointsHelper.RollProportionalGamePoints(storeIncident, wager, parms.points);
            }

            parms.forced = true;
            data.DoExtraSetup(worker, parms, storeIncident);

            return worker.CanFireNow(parms);
        }

        public override void Execute()
        {
            if (worker.TryExecute(parms))
            {
                Viewer.TakeViewerCoins(data.UseStoryteller ? storeIncident.cost : wager);
                Viewer.CalculateNewKarma(storeIncident.karmaType, wager);

                string name = storeIncident.label ?? storeIncident.abbreviation;
                var points = parms.points.ToString("N3");

                MessageHelper.ReplyToUser(
                    Viewer.username,
                    data.UseStoryteller ? "TKUtils.Wagered.Storyteller".LocalizeKeyed(name, points) : "TKUtils.Wagered.Complete".LocalizeKeyed(name, wager.ToString("N0"), points)
                );

                return;
            }

            MessageHelper.ReplyToUser(Viewer.username, "TKUtils.FailedParms".LocalizeKeyed(storeIncident.label ?? storeIncident.abbreviation));
        }
    }
}
