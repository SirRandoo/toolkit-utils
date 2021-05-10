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

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class Smite : IncidentVariablesBase
    {
        private Pawn pawn;

        public override bool CanHappen([NotNull] string msg, Viewer viewer)
        {
            string target = msg.Split(' ').Take(2).FirstOrDefault();

            if (PurchaseHelper.TryGetPawn(target, out pawn))
            {
                return pawn?.Spawned == true && pawn?.Map != null;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.PawnNotFound".LocalizeKeyed(target));
            return false;
        }

        public override void Execute()
        {
            pawn.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(pawn.Map, pawn.Position));
            Viewer.Charge(storeIncident);
        }
    }
}
