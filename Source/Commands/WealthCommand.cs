using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class WealthCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            if (Current.Game?.CurrentMap == null)
            {
                return;
            }

            twitchMessage.Reply(
                "TKUtils.Formats.KeyValue".Translate(
                    "TKUtils.Responses.Wealth".TranslateSimple(),
                    Current.Game.CurrentMap.PlayerWealthForStoryteller.ToString("N0")
                )
            );
        }
    }
}
