using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IRC;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class InstalledModsDriver : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            message.Reply(
                (
                    TkSettings.VersionedModList
                        ? GetModListStringVersioned()
                        : GetModListString()
                ).WithHeader($"Toolkit v{Toolkit.Mod.Version}")
            );
        }

        private static string GetModListString()
        {
            var container = new List<string>();
            var unversioned = TKUtils.GetModListUnversioned();

            foreach(var mod in unversioned)
            {
                container.Add(TryFavoriteMod(mod));
            }

            return string.Join(
                "TKUtils.Misc.Separators.Inner".Translate(),
                container
            );
        }

        private static string GetModListStringVersioned()
        {
            var container = new List<string>();
            var versioned = TKUtils.GetModListVersioned();

            foreach(var mod in versioned)
            {
                container.Add(
                    "TKUtils.Formats.ModList.Mod".Translate(
                        TryFavoriteMod(mod.Item1).Named("NAME"),
                        mod.Item2.Named("VERSION")
                    )
                );
            }

            return string.Join(
                "TKUtils.Misc.Separators.Inner".Translate(),
                container
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
