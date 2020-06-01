using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class UnstickCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            int stuck = Purchase_Handler.viewerNamesDoingVariableCommands.Count;

            Purchase_Handler.viewerNamesDoingVariableCommands = new List<string>();

            twitchMessage.Reply($"Unstuck {stuck.ToString()} viewers.");
        }
    }
}
