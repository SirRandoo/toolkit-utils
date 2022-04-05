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

namespace SirRandoo.ToolkitUtils.Models
{
    public class UserData
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public bool IsModerator { get; set; }
        public bool IsSubscriber { get; set; }
        public bool IsFounder { get; set; }
        public bool IsVip { get; set; }
        public bool IsBroadcaster { get; set; }
        public string DisplayName { get; set; }

        public List<KeyValuePair<string, string>> LastKnownBadges { get; set; } = new List<KeyValuePair<string, string>>();

        public Dictionary<string, UsageRecord<EventItem>> UsedEvents { get; set; }
    }
}
