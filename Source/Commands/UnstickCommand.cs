using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class UnstickCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var stuck = Purchase_Handler.viewerNamesDoingVariableCommands.Count;

            Purchase_Handler.viewerNamesDoingVariableCommands = new List<string>();

            twitchMessage.Reply($"Unstuck {stuck.ToString()} viewers.");
        }
    }
}
