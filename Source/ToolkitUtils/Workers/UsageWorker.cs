// MIT License
// 
// Copyright (c) 2022 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
