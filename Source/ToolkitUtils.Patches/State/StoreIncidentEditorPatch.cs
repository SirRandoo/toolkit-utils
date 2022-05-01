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
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class StoreIncidentEditorPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Store_IncidentEditor), nameof(Store_IncidentEditor.UpdatePriceSheet));
        }

        private static void Prefix()
        {
            foreach (StoreIncident incident in DefDatabase<StoreIncident>.AllDefs)
            {
                if (incident.cost <= 1)
                {
                    continue;
                }

                EventTypes type = incident.GetModExtension<EventExtension>()?.EventType ?? EventTypes.Default;

                if (type == EventTypes.Default || type == EventTypes.Variable)
                {
                    continue;
                }

                incident.cost = 1;
            }
        }

        private static void Postfix()
        {
            if (TkSettings.Offload)
            {
                Task.Run(
                    async () =>
                    {
                        await Data.SaveEventDataAsync(Paths.EventDataFilePath);
                    }
                );
            }
            else
            {
                Data.SaveEventData(Paths.EventDataFilePath);
            }
        }
    }
}
