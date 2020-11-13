using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class UnstickMe : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            if (!Purchase_Handler.viewerNamesDoingVariableCommands.Contains(twitchMessage.Username.ToLowerInvariant()))
            {
                return;
            }

            if (Find.TickManager?.Paused ?? true)
            {
                twitchMessage.Reply("TKUtils.Paused".Localize());
                return;
            }

            if (Purchase_Handler.viewerNamesDoingVariableCommands.Remove(twitchMessage.Username.ToLowerInvariant()))
            {
                twitchMessage.Reply("TKUtils.UnstickMe".Localize());
            }
        }
    }
}
