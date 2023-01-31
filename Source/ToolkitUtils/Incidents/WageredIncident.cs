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
    public class WageredIncident : IncidentVariablesBase
    {
        private static readonly Dictionary<string, IWageredIncidentData> Data = new Dictionary<string, IWageredIncidentData>
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
        private IWageredIncidentData _data;
        private IncidentParms _params;
        private int _wager;
        private IncidentWorker _worker;

        public override bool CanHappen(string msg, Viewer viewer)
        {
            if (!Data.TryGetValue(storeIncident.defName, out _data))
            {
                return false;
            }

            if (!_data.UseStoryteller)
            {
                string rawPoints = CommandFilter.Parse(msg).Skip(2).FirstOrDefault();

                if (rawPoints.NullOrEmpty() || !VariablesHelpers.PointsWagerIsValid(rawPoints, viewer, ref _wager, ref storeIncident))
                {
                    return false;
                }
            }

            Map map = Find.RandomPlayerHomeMap;
            _worker = Activator.CreateInstance(_data.WorkerClass) as IncidentWorker;

            if (_worker == null || map == null)
            {
                return false;
            }

            _params = StorytellerUtility.DefaultParmsNow(_data.ResolveCategory(_worker, storeIncident), map);

            if (!_data.UseStoryteller)
            {
                _params.points = IncidentHelper_PointsHelper.RollProportionalGamePoints(storeIncident, _wager, _params.points);
            }

            _params.forced = true;
            _data.DoExtraSetup(_worker, _params, storeIncident);

            return _worker.CanFireNow(_params);
        }

        public override void Execute()
        {
            if (_worker.TryExecute(_params))
            {
                Viewer.TakeViewerCoins(_data.UseStoryteller ? storeIncident.cost : _wager);
                Viewer.CalculateNewKarma(storeIncident.karmaType, _wager);

                string name = storeIncident.label ?? storeIncident.abbreviation;
                var points = _params.points.ToString("N3");

                MessageHelper.ReplyToUser(
                    Viewer.username,
                    _data.UseStoryteller
                        ? "TKUtils.Wagered.Storyteller".LocalizeKeyed(name, points)
                        : "TKUtils.Wagered.Complete".LocalizeKeyed(name, _wager.ToString("N0"), points)
                );

                return;
            }

            MessageHelper.ReplyToUser(Viewer.username, "TKUtils.FailedParms".LocalizeKeyed(storeIncident.label ?? storeIncident.abbreviation));
        }
    }
}
