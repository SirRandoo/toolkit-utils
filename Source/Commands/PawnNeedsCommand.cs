using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IRC;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnNeedsCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetOrFindPawn(message.User);

            if (pawn == null)
            {
                message.Reply("TKUtils.Responses.NoPawn".Translate().WithHeader("TabNeeds".Translate()));
                return;
            }

            var needs = pawn.needs.AllNeeds;

            if (pawn.needs?.AllNeeds == null)
            {
                message.Reply("TKUtils.Responses.PawnNeeds.None".Translate().WithHeader("TabNeeds".Translate()));
                return;
            }

            message.Reply(
                string.Join(
                        ", ",
                        needs.Select(
                                n => (string) "TKUtils.Formats.KeyValue".Translate(
                                    n.LabelCap,
                                    n.CurLevelPercentage.ToStringPercent()
                                )
                            )
                            .ToArray()
                    )
                    .WithHeader("TabNeeds".Translate())
            );
        }
    }
}
