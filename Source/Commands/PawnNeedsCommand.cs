using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnNeedsCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            if (pawn.needs?.AllNeeds == null)
            {
                twitchMessage.Reply("TKUtils.PawnNeeds.None".Localize().WithHeader("TabNeeds".Localize()));
                return;
            }

            twitchMessage.Reply(
                pawn.needs.AllNeeds.Select(
                        n => ResponseHelper.JoinPair(
                            n.LabelCap,
                            n.CurLevelPercentage.ToStringPercent()
                        )
                    )
                    .SectionJoin()
                    .WithHeader("TabNeeds".Localize())
            );
        }
    }
}
