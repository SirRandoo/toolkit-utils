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
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    public class CommandRecord
    {
        public string DefName { get; set; }
        public Command Command { get; set; }
        public CommandItem Data { get; set; }
        public DateTime LastUsage { get; set; }

        public ConcurrentDictionary<string, DateTime> LastViewerUsages { get; } =
            new ConcurrentDictionary<string, DateTime>();

        public bool IsRateLimited(Viewer viewer)
        {
            if (Data.Data == null)
            {
                return true;
            }

            DateTime now = DateTime.Now;
            TimeSpan lastUsage = now - LastUsage;
            if (Data.Data.HasGlobalCooldown && lastUsage.TotalMinutes < Data.Data.GlobalCooldown)
            {
                return false;
            }

            return !Data.Data.HasLocalCooldown
                   || !LastViewerUsages.TryGetValue(viewer.username.ToLowerInvariant(), out DateTime dateTime)
                   || !((dateTime - now).TotalMinutes < Data.Data.LocalCooldown);
        }
    }
}
