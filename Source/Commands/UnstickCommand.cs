using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.IRC;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class UnstickCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            var stuck = Purchase_Handler.viewerNamesDoingVariableCommands.Count;
            
            Purchase_Handler.viewerNamesDoingVariableCommands = new List<string>();
            
            message.Reply($"Unstuck {stuck.ToString()} viewers.");
        }
    }
}
