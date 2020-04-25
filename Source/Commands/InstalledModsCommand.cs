using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class InstalledModsCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            twitchMessage.Reply(
                (
                    TkSettings.VersionedModList
                        ? GetModListStringVersioned()
                        : GetModListString()
                ).WithHeader($"Toolkit v{Toolkit.Mod.Version}")
            );
        }

        private static string GetModListString()
        {
            return string.Join(
                ", ",
                TkUtils.ModListCache
                    .Select(m => m.name)
                    .ToArray()
            );
        }

        private static string GetModListStringVersioned()
        {
            return string.Join(
                ", ",
                TkUtils.ModListCache
                    .Select(m => $"{TryFavoriteMod(m.name)} (v{m.version ?? "?"})")
                    .ToArray()
            );
        }

        private static string TryFavoriteMod(string mod)
        {
            return !TkSettings.DecorateUtils || !mod.EqualsIgnoreCase(TkUtils.Id)
                ? mod
                : $"{"★".AltText("*")}{mod}";
        }
    }
}
