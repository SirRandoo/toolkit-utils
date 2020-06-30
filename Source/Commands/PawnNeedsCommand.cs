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
                twitchMessage.Reply("TKUtils.Responses.NoPawn".TranslateSimple());
                return;
            }

            if (pawn.needs?.AllNeeds == null)
            {
                twitchMessage.Reply(
                    "TKUtils.Responses.PawnNeeds.None".TranslateSimple().WithHeader("TabNeeds".TranslateSimple())
                );
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
                    .WithHeader("TabNeeds".TranslateSimple())
            );
        }
    }
}
