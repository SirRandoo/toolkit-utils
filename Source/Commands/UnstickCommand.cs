using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class UnstickCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            int stuck = Purchase_Handler.viewerNamesDoingVariableCommands.Count;

            Purchase_Handler.viewerNamesDoingVariableCommands = new List<string>();

            twitchMessage.Reply("TKUtils.Unstick".Localize(stuck.ToString()));
        }
    }
}
