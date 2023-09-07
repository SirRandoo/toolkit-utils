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
using Newtonsoft.Json;
using SirRandoo.ToolkitUtils.Defs;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using ToolkitCore.Models;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils.Models;

[UsedImplicitly]
public class CommandItem : IUsageItemBase
{
    [CanBeNull] [JsonProperty("data")] public CommandData Data { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("usage")] public string Usage { get; set; }
    [JsonProperty("userLevel")] public UserLevels UserLevel { get; set; }
    [JsonProperty("shortcut")] public bool Shortcut => Data?.IsShortcut ?? false;
    [JsonProperty("defName")] public string DefName { get; set; }
    [JsonIgnore] [CanBeNull] public IConfigurableUsageData UsageData => Data;

    public static CommandItem FromToolkit(Command command)
    {
        var result = new CommandItem { Name = command.LabelCap.RawText, Usage = $"!{command.command}", DefName = command.defName };
        result.PullFromExtension(command);

        if (command.requiresAdmin || command.requiresMod)
        {
            result.UserLevel = UserLevels.Moderator;
        }

        result.Data = new CommandData
        {
            IsShortcut = command.commandDriver.Name.Equals("Buy") && !command.defName.Equals("Buy"),
            Mod = command.TryGetModName(),
            IsBalance = command == CommandDefOf.CheckBalance,
            IsBuy = command == CommandDefOf.Buy
        };

        return result;
    }

    public static CommandItem FromToolkitCore(ToolkitChatCommand command)
    {
        var result = new CommandItem { Name = command.LabelCap.RawText ?? command.commandText, Usage = $"!{command.commandText}", DefName = command.defName };

        result.PullFromExtension(command);

        if (command.requiresBroadcaster || command.requiresMod)
        {
            result.UserLevel = UserLevels.Moderator;
        }

        result.Data = new CommandData { IsShortcut = false, Mod = command.TryGetModName() };

        return result;
    }

    private void PullFromExtension(Def command)
    {
        var extension = command.GetModExtension<CommandExtension>();

        if (extension == null)
        {
            return;
        }

        Description = command.description;
        UserLevel = extension.UserLevel;

        if (extension.Parameters.NullOrEmpty())
        {
            return;
        }

        Usage += " ";
        Usage += string.Join(" ", extension.Parameters.Select(i => i.ToString().ToLowerInvariant()).ToArray());
    }
}
