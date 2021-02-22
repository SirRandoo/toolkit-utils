// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
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
