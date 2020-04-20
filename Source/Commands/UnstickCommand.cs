using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class UnstickCommand : CommandBase
    {
        public override bool CanExecute(ITwitchCommand twitchCommand)
        {
            if (!base.CanExecute(twitchCommand))
            {
                return false;
            }

            return Purchase_Handler.viewerNamesDoingVariableCommands.Count > 0;
        }
        
        public override void Execute(ITwitchCommand twitchCommand)
        {
            var stuck = Purchase_Handler.viewerNamesDoingVariableCommands.Count;

            Purchase_Handler.viewerNamesDoingVariableCommands = new List<string>();

            twitchCommand.Reply($"Unstuck {stuck.ToString()} viewers.");
        }

        public UnstickCommand(ToolkitChatCommand command) : base(command)
        {
        }
    }
}
