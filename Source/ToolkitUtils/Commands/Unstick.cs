using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class Unstick : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            int stuck = Purchase_Handler.viewerNamesDoingVariableCommands.Count;

            Purchase_Handler.viewerNamesDoingVariableCommands = new List<string>();

            twitchMessage.Reply("TKUtils.Unstick".Localize(stuck.ToString()));
        }
    }
}
