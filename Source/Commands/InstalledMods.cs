using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class InstalledMods : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            twitchMessage.Reply(
                (TkSettings.VersionedModList ? GetModListStringVersioned() : GetModListString()).WithHeader(
                    $"Toolkit v{Toolkit.Mod.Version}"
                )
            );
        }

        private static string GetModListString()
        {
            return Data.Mods.Select(m => m.Name).SectionJoin();
        }

        private static string GetModListStringVersioned()
        {
            return Data.Mods.Select(
                    m => m.Version.NullOrEmpty()
                        ? $"{TryFavoriteMod(m.Name)}"
                        : $"{TryFavoriteMod(m.Name)} (v{m.Version})"
                )
               .SectionJoin();
        }

        private static string TryFavoriteMod(string mod)
        {
            return !TkSettings.DecorateUtils || !mod.EqualsIgnoreCase(TkUtils.Id) ? mod : $"{"★".AltText("*")}{mod}";
        }
    }
}
