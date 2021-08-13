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
using System.Collections.Concurrent;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit;
using TwitchToolkit.Incidents;

namespace SirRandoo.ToolkitUtils
{
    public static class EventHistorian
    {
        private static readonly ConcurrentDictionary<string, EventHistory> History =
            new ConcurrentDictionary<string, EventHistory>();

        public static void ResetHistory()
        {
            History.Clear();
        }

        public static bool IsOnCooldown([NotNull] StoreIncident incident)
        {
            if (!History.TryGetValue(incident.defName, out EventHistory history))
            {
                return false;
            }

            DateTime now = DateTime.Now;
            return history.Data.EventData?.HasGlobalCooldown == true
                   && (now - history.LastUsage).TotalMinutes < history.Data.EventData.GlobalCooldown;
        }

        public static bool IsOnCooldown([NotNull] StoreIncident incident, Viewer viewer)
        {
            if (!History.TryGetValue(incident.defName, out EventHistory history))
            {
                return false;
            }

            DateTime now = DateTime.Now;
            if (history.Data.EventData?.HasGlobalCooldown == true
                && (now - history.LastUsage).TotalMinutes < history.Data.EventData.GlobalCooldown)
            {
                return true;
            }

            if (history.Data.EventData?.HasLocalCooldown == true
                && history.LastViewerUsages.TryGetValue(viewer.username.ToLowerInvariant(), out DateTime lastUsage))
            {
                return (now - lastUsage).TotalMinutes < history.Data.EventData.LocalCooldown;
            }

            return true;
        }

        public static void RegisterUsage([NotNull] StoreIncident incident)
        {
            History.AddOrUpdate(
                incident.defName,
                new EventHistory
                {
                    Incident = incident,
                    Data = Data.Events.Find(e => e.DefName.Equals(incident.defName)),
                    DefName = incident.defName,
                    LastUsage = DateTime.Now
                },
                delegate(string s, EventHistory history)
                {
                    history.LastUsage = DateTime.Now;
                    return history;
                }
            );
        }

        public static void RegisterUsage([NotNull] StoreIncident incident, [NotNull] Viewer viewer)
        {
            if (!History.TryGetValue(incident.defName, out EventHistory history))
            {
                history = new EventHistory
                {
                    DefName = incident.defName,
                    Incident = incident,
                    Data = Data.Events.Find(e => e.DefName.Equals(incident.defName)),
                    LastUsage = DateTime.Now
                };

                History.TryAdd(incident.defName, history);
            }

            history.LastViewerUsages.TryAdd(viewer.username.ToLowerInvariant(), DateTime.Now);
        }
    }
}
