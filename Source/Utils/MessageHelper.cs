using TwitchToolkit.IRC;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class MessageHelper
    {
        internal static void Reply(this IRCMessage m, string message)
        {
            ReplyToUser(m.User, message);
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
            CommandBase.SendMessage($@"{user} → {message}");
        }

        internal static string AltText(this string emoji, string alt = null)
        {
            if (alt == null)
            {
                alt = $"{emoji}.Text";
            }

            return TkSettings.Emojis ? emoji : alt;
        }
    }
}
