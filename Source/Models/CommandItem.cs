using System.Linq;
using JetBrains.Annotations;
using ToolkitCore.Models;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class CommandItem
    {
        [CanBeNull] public CommandData Data;
        public string Description;
        public string Name;
        public string Usage;
        public UserLevels UserLevel;

        public bool Shortcut => Data?.IsShortcut ?? false;

        public static CommandItem FromToolkit(Command command)
        {
            var result = new CommandItem {Name = command.LabelCap.RawText, Usage = $"!{command.command}"};
            result.PullFromExtension(command);

            if (command.requiresAdmin || command.requiresMod)
            {
                result.UserLevel = UserLevels.Moderator;
            }

            result.Data = new CommandData
            {
                IsShortcut = command.commandDriver.Name.Equals("Buy") && !command.defName.Equals("Buy")
            };

            return result;
        }

        public static CommandItem FromToolkitCore(ToolkitChatCommand command)
        {
            var result = new CommandItem {Name = command.LabelCap.RawText, Usage = $"!{command.commandText}"};
            result.PullFromExtension(command);

            if (command.requiresBroadcaster || command.requiresMod)
            {
                result.UserLevel = UserLevels.Moderator;
            }

            result.Data = new CommandData {IsShortcut = false};

            return result;
        }

        private void PullFromExtension(Def command)
        {
            var extension = command.GetModExtension<CommandExtension>();

            if (extension == null)
            {
                return;
            }

            Description = extension.Description;
            UserLevel = extension.UserLevel;

            if (extension.Parameters.NullOrEmpty())
            {
                return;
            }

            Usage += " ";
            Usage += string.Join(" ", extension.Parameters.Select(i => i.ToString().ToLowerInvariant()).ToArray());
        }
    }
}
