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
            return string.Join(", ",TkUtils.GetModListUnversioned().Select(TryFavoriteMod).ToArray());
        }

        private static string GetModListStringVersioned()
        {
            var list = TkUtils.GetModListVersioned();

            return string.Join(", ", list.Select(m => $"{TryFavoriteMod(m.Item1)} (v{m.Item2})").ToArray());
        }

        private static string TryFavoriteMod(string mod)
        {
            return !TkSettings.DecorateUtils || !mod.EqualsIgnoreCase(TkUtils.Id)
                ? mod
                : $"{"★".AltText("*")}{mod}";
        }
    }
}
