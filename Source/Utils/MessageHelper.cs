using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class MessageHelper
    {
        internal static void Reply(this ITwitchCommand m, string message)
        {
            ReplyToUser(m.Username, message);
        }

        internal static void Reply(this ITwitchMessage m, string message)
        {
            ReplyToUser(m.Username, message);
        }

        internal static string WithHeader(this string s, string header)
        {
            return $"【{header}】 {s}";
        }

        internal static string WithHeader(this TaggedString s, string header)
        {
            return $"【{header}】 {s.ToString()}";
        }

        internal static void ReplyToUser(string user, string message)
        {
            CommandBase.SendMessage($"@{user} → {message}");
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
