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
using SirRandoo.ToolkitUtils.Interfaces;

namespace SirRandoo.ToolkitUtils.Models
{
    public class UsageRecord<T> where T : class, IUsageItemBase
    {
        public T Item { get; set; }
        public string DefName { get; set; }
        public DateTime LastUsed { get; set; } = DateTime.MinValue;
        public Dictionary<string, DateTime> LastUserUse { get; set; } = new Dictionary<string, DateTime>();

        private IConfigurableUsageData UsageData => Item.UsageData;

        public bool IsUsable(string user)
        {
            DateTime now = DateTime.Now;

            if (UsageData.HasGlobalCooldown && (now - LastUsed).TotalMinutes < UsageData.GlobalCooldown)
            {
                return false;
            }

            return !UsageData.HasLocalCooldown || !LastUserUse.TryGetValue(user.ToLowerInvariant(), out DateTime value) || !((now - value).TotalMinutes < UsageData.LocalCooldown);
        }

        public void LogUsage(string user)
        {
            DateTime now = DateTime.Now;

            if (UsageData.HasGlobalCooldown)
            {
                LastUsed = now;
            }

            if (UsageData.HasLocalCooldown)
            {
                LastUserUse[user.ToLowerInvariant()] = now;
            }
        }
    }
}
