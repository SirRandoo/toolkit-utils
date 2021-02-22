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
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using ToolkitCore;
using TwitchLib.Client.Models;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(TwitchWrapper), nameof(TwitchWrapper.SendChatMessage))]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class SendChatMessagePatch
    {
        private const int MessageLimit = 500;

        [HarmonyAfter("net.pardeike.harmony.Puppeteer")]
        public static bool Prefix(string message)
        {
            if (message.NullOrEmpty())
            {
                return false;
            }

            message = message.Replace("@", "");
            JoinedChannel channel = TwitchWrapper.Client.GetJoinedChannel(ToolkitCoreSettings.channel_username);
            foreach (string segment in SplitMessages(message))
            {
                TwitchWrapper.Client.SendMessage(channel, segment);
            }

            return false;
        }

        private static IEnumerable<string> SplitMessages(string message)
        {
            if (message.Length < MessageLimit)
            {
                yield return message;
                yield break;
            }

            string[] words = message.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var builder = new StringBuilder();
            var chars = 0;

            foreach (string word in words)
            {
                if (chars + word.Length <= MessageLimit - 3)
                {
                    builder.Append($"{word} ");
                    chars += word.Length + 1;
                }
                else
                {
                    builder.Append("...");
                    yield return builder.ToString();
                    builder.Clear();
                    chars = 0;
                }
            }

            if (builder.Length <= 0)
            {
                yield break;
            }

            yield return builder.ToString();
            builder.Clear();
        }
    }
}
