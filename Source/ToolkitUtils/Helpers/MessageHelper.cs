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
using System.Text;
using JetBrains.Annotations;
using ToolkitCore;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class MessageHelper
    {
        public static void Reply([NotNull] this ITwitchMessage m, string message)
        {
            ReplyToUser(m.Username, message);
        }

        public static string WithHeader(this string s, string header)
        {
            return $"【{header}】 {s}".AltText($"[{header}] {s}");
        }

        internal static void ReplyToUser(string user, string message)
        {
            TwitchWrapper.SendChatMessage($"@{user} → {message}");
        }

        internal static void SendConfirmation(string user, string message)
        {
            if (!ToolkitSettings.PurchaseConfirmations)
            {
                return;
            }

            ReplyToUser(user, message);
        }

        internal static string AltText(this string emoji, string alt = null)
        {
            alt ??= $"{emoji}.Text";

            return TkSettings.Emojis ? emoji : alt;
        }

        [CanBeNull]
        internal static ITwitchMessage WithMessage([NotNull] this ITwitchMessage m, string message)
        {
            if (m.WhisperMessage != null)
            {
                return new WhisperMessage(
                    m.WhisperMessage.Badges,
                    m.WhisperMessage.ColorHex,
                    m.WhisperMessage.Color,
                    m.WhisperMessage.Username,
                    m.WhisperMessage.DisplayName,
                    m.WhisperMessage.EmoteSet,
                    m.WhisperMessage.ThreadId,
                    m.WhisperMessage.MessageId,
                    m.WhisperMessage.UserId,
                    m.WhisperMessage.IsTurbo,
                    m.WhisperMessage.BotUsername,
                    message,
                    m.WhisperMessage.UserType
                );
            }

            if (m.ChatMessage != null)
            {
                return new ChatMessage(
                    m.ChatMessage.BotUsername,
                    m.ChatMessage.UserId,
                    m.ChatMessage.Username,
                    m.ChatMessage.DisplayName,
                    m.ChatMessage.ColorHex,
                    m.ChatMessage.Color,
                    m.ChatMessage.EmoteSet,
                    message,
                    m.ChatMessage.UserType,
                    m.ChatMessage.Channel,
                    m.ChatMessage.Id,
                    m.ChatMessage.IsSubscriber,
                    m.ChatMessage.SubscribedMonthCount,
                    m.ChatMessage.RoomId,
                    m.ChatMessage.IsTurbo,
                    m.ChatMessage.IsModerator,
                    m.ChatMessage.IsMe,
                    m.ChatMessage.IsBroadcaster,
                    m.ChatMessage.Noisy,
                    m.ChatMessage.RawIrcMessage,
                    m.ChatMessage.EmoteReplacedMessage,
                    m.ChatMessage.Badges,
                    m.ChatMessage.CheerBadge,
                    m.ChatMessage.Bits,
                    m.ChatMessage.BitsInDollars
                );
            }

            return null;
        }

        public static bool HasBadges([CanBeNull] this ITwitchMessage message, params string[] badges)
        {
            if (message?.ChatMessage?.Badges.NullOrEmpty() == true)
            {
                return false;
            }

            foreach (string badge in badges)
            {
                if (message!.ChatMessage!.Badges.Any(
                    p => p.Key.Equals(badge, StringComparison.InvariantCultureIgnoreCase)
                ))
                {
                    return true;
                }
            }

            return false;
        }

        [NotNull]
        public static string Append(this string s, string text)
        {
            return $"{s}{text}";
        }

        [NotNull]
        public static string AppendWithSpace(this string s, string text)
        {
            return $"{s} {text}";
        }

        [NotNull]
        public static string Insert(this string s, int index, string text)
        {
            return new StringBuilder().Append(s).Insert(index, text).ToString();
        }
    }
}
