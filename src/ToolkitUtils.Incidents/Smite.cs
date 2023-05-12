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

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using ToolkitCore;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace ToolkitUtils.Incidents
{
    public class Smite : IncidentVariablesBase
    {
        private Pawn _pawn;

        public override bool CanHappen([NotNull] string msg, [NotNull] Viewer viewer)
        {
            if (!viewer.mod || !viewer.username.EqualsIgnoreCase(ToolkitCoreSettings.channel_username))
            {
                return false;
            }

            string target = msg.Split(' ').Skip(2).FirstOrDefault();

            if (PurchaseHelper.TryGetPawn(target, out _pawn))
            {
                return _pawn?.Spawned == true && _pawn?.Map != null;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.PawnNotFound".Translate(target));

            return false;
        }

        public override void Execute()
        {
            _pawn.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(_pawn.Map, _pawn.Position));
            Viewer.Charge(storeIncident);
        }
    }
}
