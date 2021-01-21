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
