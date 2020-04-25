using TwitchLib.Client.Interfaces;
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
    }
}
