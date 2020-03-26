using System;
using System.Linq;
using HarmonyLib;
using TwitchToolkit;
using TwitchToolkit.IRC;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(ViewerUpdater), "ParseCommand")]
    public static class ViewerUpdater_ParseCommand
    {
        [HarmonyPostfix]
        public static void ParseCommand(IRCMessage msg)
        {
            try
            {
                var badges = msg.Parameters
                    .FirstOrDefault(p => p.Key.Equals("badges"));

                if (!badges.Key.Equals("badges") || badges.Value.NullOrEmpty())
                {
                    return;
                }

                var viewer = Viewers.GetViewer(msg.User);
                var value = badges.Value;

                foreach (var type in value.Split(','))
                {
                    var badge = type.Split('/').FirstOrDefault();

                    switch (badge)
                    {
                        case null:
                            Logger.Warn($"Could not check badge \"{type}\"");
                            continue;
                        case "vip":
                            viewer.vip = true;
                            break;
                        case "founder":
                            viewer.subscriber = true;
                            break;
                        case "moderator":
                        case "broadcaster":
                        case "admin":
                        case "staff":
                        case "global_mod":
                            viewer.mod = true;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Founder patch failed", e);
            }
        }
    }
}
