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

                    if (badge == null)
                    {
                        Logger.Warn($"Could not check badge \"{type}\"");
                        continue;
                    }

                    if (badge.Equals("vip"))
                    {
                        viewer.vip = true;
                    }
                    else if (badge.Equals("founder"))
                    {
                        viewer.subscriber = true;
                    }
                    else if (badge.Equals("moderator")
                             || badge.Equals("broadcaster")
                             || badge.Equals("admin")
                             || badge.Equals("staff")
                             || badge.Equals("global_mod"))
                    {
                        viewer.mod = true;
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
