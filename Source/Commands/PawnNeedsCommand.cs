using System.Collections.Generic;

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
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetOrFindPawn(message.User);

            if(pawn.needs != null)
            {
                var needs = pawn.needs.AllNeeds;
                var container = new List<string>();

                foreach(var need in needs)
                {
                    container.Add(
                        "TKUtils.Formats.PawnNeeds.Need".Translate(
                            need.LabelCap.Named("NEED"),
                            GenText.ToStringPercent(need.CurLevelPercentage).Named("PERCENT")
                        )
                    );
                }

                SendCommandMessage(
                    "TKUtils.Formats.PawnNeeds.Base".Translate(
                        string.Join(
                            "TKUtils.Misc.Separators.Inner".Translate(),
                            container
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
