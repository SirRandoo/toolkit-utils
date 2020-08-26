using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class ColonistCount : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            if (Find.ColonistBar == null)
            {
                return;
            }

            List<Pawn> colonists = Find.ColonistBar.GetColonistsInOrder();

            if (colonists.Count <= 0)
            {
                twitchMessage.Reply("TKUtils.ColonistCount.None".Localize());
                return;
            }

            twitchMessage.Reply("TKUtils.ColonistCount.Any".Localize(colonists.Count.ToString("N0")));
        }
    }
}
