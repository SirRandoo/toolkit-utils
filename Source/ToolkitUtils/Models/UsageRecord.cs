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

using System;
using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Interfaces;

namespace SirRandoo.ToolkitUtils.Models
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
        public bool IsOnCooldown() => (DateTime.UtcNow - _lastUsage).TotalSeconds >= 5 * 60;

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

            if (!(Item.UsageData is { HasLocalCooldown: true }) || !_userUsages.TryGetValue(user, out DateTime lastUsage))
            {
                return false;
            }

            return (DateTime.UtcNow - lastUsage).TotalSeconds >= Item.UsageData.LocalCooldown;
        }
    }
}
