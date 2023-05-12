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

using JetBrains.Annotations;
using RimWorld;
using TwitchToolkit.Store;
using Verse;

namespace ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class Party : IncidentHelper
    {
        private Map _map;
        private GatheringWorker_Party _worker;

        public override bool IsPossible()
        {
            _map = Find.RandomPlayerHomeMap;
            _worker = new GatheringWorker_Party { def = GatheringDefOf.Party };

            return _worker.CanExecute(_map);
        }

        public override void TryExecute()
        {
            _worker.TryExecute(_map);
        }
    }
}
