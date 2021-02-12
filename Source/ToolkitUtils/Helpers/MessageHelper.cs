using ToolkitCore;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class MessageHelper
    {
        public static void Reply(this ITwitchMessage m, string message)
        {
            ReplyToUser(m.Username, message);
        }

        public static string WithHeader(this string s, string header)
        {
            return $"【{header}】 {s}".AltText($"[{header}] {s}");
        }

        internal static void ReplyToUser(string user, string message, bool bypassPuppeteer = false)
        {
            if (!bypassPuppeteer)
            {
                TwitchWrapper.SendChatMessage($"@{user} → {message}");
            }
            else
            {
                TwitchWrapper.Client?.SendMessage(ToolkitCoreSettings.channel_username, $"@{user} → {message}");
            }
        }

        internal static string AltText(this string emoji, string alt = null)
        {
            alt ??= $"{emoji}.Text";

            return TkSettings.Emojis ? emoji : alt;
        }

        internal static ITwitchMessage WithMessage(this ITwitchMessage m, string message)
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
    }
}
