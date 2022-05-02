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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    public class Sanctuary : IncidentVariablesBase
    {
        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!viewer.CanAfford(storeIncident.cost))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InsufficientBalance".Localize());

                return false;
            }

            if (Current.Game.Maps.Any(m => m.IsPlayerHome))
            {
                return Current.Game.Maps.All(map => !map.GameConditionManager.ConditionIsActive(GameConditionDefOf.Sanctuary));
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoMap".Localize());

            return false;
        }

        public override void Execute()
        {
            List<Map> maps = Current.Game.Maps;

            foreach (Map map in maps)
            {
                if (!map.IsPlayerHome || map.GameConditionManager.ConditionIsActive(GameConditionDefOf.Sanctuary))
                {
                    continue;
                }

                map.GameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(GameConditionDefOf.Sanctuary, Rand.Range(2, 6) * 60000));
            }

            Viewer.Charge(storeIncident);

            Find.LetterStack.ReceiveLetter("TKUtils.SanctuaryLetter.Title".Localize(), "TKUtils.SanctuaryLetter.Description".Localize(), LetterDefOf.PositiveEvent);

            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.Sanctuary.Complete".Localize());
        }
    }
}
