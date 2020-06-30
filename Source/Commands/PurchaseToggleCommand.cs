using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PurchaseToggleCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            TkSettings.StoreState = !TkSettings.StoreState;

            string response = TkSettings.StoreState ? "Enabled" : "Disabled";
            twitchMessage.Reply($"TKUtils.Responses.StoreState.{response}".Translate());
        }
    }
}
