// ToolkitUtils
// Copyright (C) 2022 SirRandoo
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
using ToolkitUtils.Interfaces;

namespace ToolkitUtils.Models
{
    /// <summary>
    ///     An class for recording, and retrieving usage data on ratelimited
    ///     mechanics.
    /// </summary>
    public class UsageRecord<T> where T : class, IUsageItemBase
    {
        private readonly Dictionary<string, DateTime> _userUsages = new Dictionary<string, DateTime>();
        private DateTime _lastUsage = DateTime.MinValue;

        /// <summary>
        ///     The item being ratelimited.
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        ///     The def name of the item being ratelimited.
        /// </summary>
        public string DefName => Item.DefName;

        /// <summary>
        ///     Records a usage of the given <see cref="Item"/>.
        /// </summary>
        public void RecordUsage()
        {
            _lastUsage = DateTime.UtcNow;
        }

        /// <summary>
        ///     Records a usage of the given <see cref="Item"/> for the user.
        /// </summary>
        /// <param name="user">The user that used <see cref="Item"/></param>
        public void RecordUsage(string user)
        {
            RecordUsage();

            if (Item.UsageData?.HasLocalCooldown == true)
            {
                _userUsages[user] = DateTime.UtcNow;
            }
        }

        /// <summary>
        ///     Whether <see cref="Item"/> is on cooldown.
        /// </summary>
        public bool IsOnCooldown() => Item.UsageData.HasGlobalCooldown && (DateTime.UtcNow - _lastUsage).TotalSeconds < Item.UsageData.GlobalCooldown;

        /// <summary>
        ///     Whether <see cref="Item"/> is on cooldown for the given user.
        /// </summary>
        /// <param name="user">The user to check against</param>
        public bool IsOnCooldown(string user)
        {
            if (IsOnCooldown())
            {
                return true;
            }

            if (Item.UsageData == null || !Item.UsageData.HasLocalCooldown || !_userUsages.TryGetValue(user, out DateTime lastUsage))
            {
                return false;
            }

            return (DateTime.UtcNow - lastUsage).TotalSeconds < Item.UsageData.LocalCooldown;
        }
    }
}
