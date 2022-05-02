// ToolkitUtils
// Copyright (C) 2022  SirRandoo
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
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;

namespace SirRandoo.ToolkitUtils.Workers
{
    /// <summary>
    ///     A generic class for managing ratelimited items.
    /// </summary>
    /// <typeparam name="T">The item class the records are for</typeparam>
    public class UsageWorker<T> where T : class, IUsageItemBase
    {
        private readonly Dictionary<string, UsageRecord<T>> _records = new Dictionary<string, UsageRecord<T>>();

        private UsageRecord<T> GetRecord([NotNull] T item)
        {
            if (!_records.TryGetValue(item.DefName, out UsageRecord<T> record))
            {
                _records[item.DefName] = record = new UsageRecord<T> { Item = item };
            }

            return record;
        }

        /// <summary>
        ///     Whether the given item is on a global cooldown.
        /// </summary>
        /// <param name="item">The item to check</param>
        public bool IsOnCooldown([NotNull] T item) => _records.TryGetValue(item.DefName, out UsageRecord<T> record) && record.IsOnCooldown();

        /// <summary>
        ///     Whether the given item is on cooldown for the user.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <param name="user">The user to check</param>
        public bool IsOnCooldown([NotNull] T item, string user) => _records.TryGetValue(item.DefName, out UsageRecord<T> record) && record.IsOnCooldown(user);

        /// <summary>
        ///     Records a usage for the given item.
        /// </summary>
        /// <param name="item">The item to record a usage for</param>
        public void RecordUsage([NotNull] T item)
        {
            GetRecord(item).RecordUsage();
        }

        /// <summary>
        ///     Records a usage for the given user on the given item.
        /// </summary>
        /// <param name="item">The item to record a usage for</param>
        /// <param name="user">The user to record a usage for</param>
        public void RecordUsage([NotNull] T item, string user)
        {
            GetRecord(item).RecordUsage(user);
        }
    }
}
