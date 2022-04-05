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
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Workers;
using TwitchToolkit;
using TwitchToolkit.Incidents;

namespace SirRandoo.ToolkitUtils
{
    public static class GlobalUsageService
    {
        private static readonly UsageWorker<EventItem> EventWorker = new UsageWorker<EventItem>();
        private static readonly UsageWorker<CommandItem> CommandWorker = new UsageWorker<CommandItem>();

        public static void RecordUsage(StoreIncident incident)
        {
            EventItem @event = Data.Events.Find(e => string.Equals(e.DefName, incident.defName));

            if (@event == null)
            {
                return;
            }

            EventWorker.RecordUsage(@event);
        }

        public static void RecordUsage(Command command)
        {
            CommandItem c = Data.Commands.Find(i => string.Equals(i.DefName, command.defName));

            if (c == null)
            {
                return;
            }

            CommandWorker.RecordUsage(c);
        }

        public static bool IsOnCooldown(StoreIncident incident)
        {
            EventItem item = Data.Events.Find(e => string.Equals(e.DefName, incident.defName));

            if (item == null)
            {
                return false;
            }

            return false;
        }
    }
}
