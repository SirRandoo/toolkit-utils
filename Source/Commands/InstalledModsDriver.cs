using System.Linq;

using SirRandoo.ToolkitUtils.Utils;

using TwitchLib.Client.Models;

using TwitchToolkit;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class InstalledModsDriver : CommandBase
    {
        public static string GetModListString()
        {
            return string.Join(
                "TKUtils.Misc.Separators.Inner".Translate(),
                TKUtils.GetModListUnversioned().Select(m => TryFavoriteMod(m))
            );
        }

        public static string GetModListStringVersioned()
        {
            return string.Join(
                "TKUtils.Misc.Separators.Inner".Translate(),
                TKUtils.GetModListVersioned().Select(m =>
                {
                    return "TKUtils.Formats.ModList.Mod".Translate(
                        TryFavoriteMod(m.Key).Named("NAME"),
                        m.Value.Named("VERSION")
                    );
                })
            );
        }

        public override void RunCommand(ChatMessage message)
        {
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            Log("Preparing mod list...");

            SendCommandMessage(
                "TKUtils.Formats.ModList.Base".Translate(
                    Toolkit.Mod.Version.Named("VERSION"),
                    (TKSettings.VersionedModList ? GetModListStringVersioned() : GetModListString()).Named("MODS")
                ),
                message
            );
        }

        private static string TryFavoriteMod(string mod)
        {
            if(mod.EqualsIgnoreCase(TKUtils.ID))
            {
                return GetTranslatedEmoji(
                    "TKUtils.Misc.Decorators.Favorite",
                    "TKUtils.Misc.Decorators.Favorite.Text"
                ).Translate(
                    mod.Named("DECORATING")
                );
            }
            else
            {
                return mod;
            }
        }
    }
}
