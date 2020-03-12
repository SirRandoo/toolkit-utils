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
            if(!CommandsHandler.AllowCommand(message)) return;

            var pawn = GetPawnDestructive(message.User);

            if(pawn.needs != null)
            {
                SendCommandMessage(
                    "TKUtils.Formats.PawnNeeds.Base".Translate(
                        string.Join(
                            "TKUtils.Misc.Separators.Inner".Translate(),
                            pawn.needs.AllNeeds
                                .Select(n => "TKUtils.Formats.PawnNeeds.Need".Translate(
                                    n.LabelCap.Named("NEED"),
                                    string.Format("{0:P2}", n.CurLevelPercentage).Named("PERCENT")
                                ))
                        ).Named("NEEDS")
                    ),
                    message
                );
            }
            else
            {
                SendCommandMessage(
                    "TKUtils.Responses.PawnNeeds.None".Translate(),
                    message
                );
            }
        }
    }
}
