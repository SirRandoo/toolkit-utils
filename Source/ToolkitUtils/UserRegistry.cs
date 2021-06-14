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
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public static class UserRegistry
    {
        private static readonly Dictionary<string, UserData> UserData = new Dictionary<string, UserData>();

        [CanBeNull]
        public static UserData GetData([NotNull] string username)
        {
            return !UserData.TryGetValue(username.ToLowerInvariant(), out UserData state) ? null : state;
        }

        [CanBeNull]
        public static UserData UpdateData([NotNull] ITwitchMessage message)
        {
            if (message.ChatMessage?.Username.NullOrEmpty() == true)
            {
                return null;
            }

            var data = new UserData
            {
                Username = message!.ChatMessage!.Username,
                IsBroadcaster = message.HasBadges("broadcaster"),
                IsFounder = message.HasBadges("founder", "broadcaster"),
                IsModerator = message.HasBadges("moderator", "broadcaster", "global_mod", "staff", "admin"),
                IsSubscriber = message.HasBadges("subscriber", "founder", "broadcaster"),
                IsVip = message.HasBadges("vip", "broadcaster"),
                LastKnownBadges = message.ChatMessage.Badges
            };

            UserData[message!.ChatMessage!.Username.ToLowerInvariant()] = data;
            return data;
        }

        public static bool DeleteData([NotNull] string username)
        {
            return UserData.Remove(username.ToLowerInvariant());
        }
    }
}
