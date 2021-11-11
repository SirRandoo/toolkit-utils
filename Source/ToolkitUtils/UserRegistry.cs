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
        private static readonly List<UserData> UserData = new List<UserData>();

        [CanBeNull]
        public static UserData GetData([NotNull] ITwitchMessage message)
        {
            if (message.ChatMessage != null)
            {
                return GetData(message.ChatMessage.UserId ?? message.ChatMessage.Username);
            }

            return message.WhisperMessage != null ? GetData(message.WhisperMessage.UserId ?? message.WhisperMessage.Username) : null;
        }

        [CanBeNull]
        public static UserData GetData([NotNull] string idOrName)
        {
            return idOrName.NullOrEmpty()
                ? null
                : UserData.Find(d => d.Id?.Equals(idOrName) == true || d.Username?.EqualsIgnoreCase(idOrName) == true || d.DisplayName?.EqualsIgnoreCase(idOrName) == true);

        }

        [ContractAnnotation("=> false,data:null; => true,data:notnull")]
        public static bool TryGetData([NotNull] string idOrName, out UserData data)
        {
            data = GetData(idOrName);
            return data != null;
        }

        [ContractAnnotation("=> false,data:null; => true,data:notnull")]
        public static bool TryGetData([NotNull] ITwitchMessage message, out UserData data)
        {
            data = GetData(message);
            return data != null;
        }

        [NotNull]
        public static UserData UpdateData([NotNull] ITwitchMessage message)
        {
            UserData data = GetData(message);

            if (data == null)
            {
                UserData.Add(data = new UserData { Id = message.ChatMessage?.UserId ?? message.WhisperMessage?.UserId });

                if (data.Id.NullOrEmpty())
                {
                    LogHelper.Warn($"Could not get the user id for {message.Username}. Things will not work as excepted.");
                }
            }

            if (message.ChatMessage?.BotUsername.EqualsIgnoreCase("puppeteer") == true)
            {
                data.DisplayName = message.ChatMessage?.DisplayName ?? message.WhisperMessage?.DisplayName ?? message.Username?.CapitalizeFirst();
            }
            else
            {
                data.Username = message.Username;
                data.DisplayName = message.ChatMessage?.DisplayName ?? message.WhisperMessage?.DisplayName ?? message.Username.CapitalizeFirst();
            }

            data.IsBroadcaster = message.HasBadges("broadcaster");
            data.IsFounder = message.HasBadges("founder", "broadcaster");
            data.IsModerator = message.HasBadges("moderator", "global_mod", "staff", "admin", "broadcaster");
            data.IsSubscriber = message.HasBadges("subscriber", "founder", "broadcaster");
            data.IsVip = message.HasBadges("vip", "broadcaster");
            data.LastKnownBadges = message.ChatMessage?.Badges;
            return data;
        }

        public static bool DeleteData([NotNull] string idOrName)
        {
            UserData data = GetData(idOrName);

            return data != null && UserData.Remove(data);
        }
    }
}
