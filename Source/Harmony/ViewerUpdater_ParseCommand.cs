using System;
using System.Linq;

using HarmonyLib;

using SirRandoo.ToolkitUtils.Utils;

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
                    .Where(p => p.Key.Equals("badges"))
                    .FirstOrDefault();

                if(badges.Key.Equals("badges") && !badges.Value.NullOrEmpty())
                {
                    var viewer = Viewers.GetViewer(msg.User);
                    var value = badges.Value;

                    foreach(var type in value.Split(','))
                    {
                        var pair = type.Split('/');
                        var badge = pair.FirstOrDefault();
                        var version = pair.LastOrDefault();

                        if(badge.Equals("vip"))
                        {
                            viewer.vip = true;
                        }
                        else if(badge.Equals("founder"))
                        {
                            viewer.subscriber = true;
                        }
                        else if(badge.Equals("moderator") || badge.Equals("broadcaster") || badge.Equals("admin") || badge.Equals("staff") || badge.Equals("global_mod"))
                        {
                            viewer.mod = true;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                CommandBase.Error($"Founder patch failed with exception: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
