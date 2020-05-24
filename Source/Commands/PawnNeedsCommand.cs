using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnNeedsCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate("TabNeeds".Translate()));
                return;
            }

            if (pawn.needs?.AllNeeds == null)
            {
                twitchMessage.Reply("TKUtils.Responses.PawnNeeds.None".Translate().WithHeader("TabNeeds".Translate()));
                return;
            }

            twitchMessage.Reply(
                string.Join(
                        ", ",
                        pawn.needs.AllNeeds.Select(
                                n => "TKUtils.Formats.KeyValue".Translate(
                                        n.LabelCap,
                                        n.CurLevelPercentage.ToStringPercent()
                                    )
                                    .RawText
                            )
                            .ToArray()
                    )
                    .WithHeader("TabNeeds".Translate())
            );
        }
    }
}
