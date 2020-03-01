using System;
using System.Linq;

using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit;
using TwitchToolkit.IRC;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class InstalledModsDriver : CommandBase
    {
        public static string GetModListString() => string.Join(", ", TKUtils.GetModListUnversioned());

        public static string GetModListStringVersioned()
        {
            return string.Join(", ",
                TKUtils.GetModListVersioned().Select(m =>
                {
                    return "TKUtils.Responses.ModListedFormat".Translate(
                        m.Key.Named("NAME"),
                        m.Value.Named("VERSION")
                    );
                })
            );
        }

        public override void RunCommand(IRCMessage message)
        {
            Log.Message($"{TKUtils.ID} :: Preparing mod list....");

            try
            {
                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        message.User.Named("VIEWER"),
                        "TKUtils.Responses.ModListFormat".Translate(
                            Toolkit.Mod.Version.Named("VERSION"),
                            (TKSettings.VersionedModList ? GetModListStringVersioned() : GetModListString()).Named("MODS")
                        ).Named("MESSAGE")
                    ),
                    CommandsHandler.SendToChatroom(message)
                );
            }
            catch(Exception e)
            {
                Log.Message($"{TKUtils.ID} :: Message prep failed with exception: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
