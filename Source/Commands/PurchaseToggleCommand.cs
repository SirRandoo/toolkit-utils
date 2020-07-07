using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PurchaseToggleCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            TkSettings.StoreState = !TkSettings.StoreState;

            string response = TkSettings.StoreState ? "Enabled" : "Disabled";
            twitchMessage.Reply($"TKUtils.ToggleStore.{response}".Localize());
        }
    }
}
