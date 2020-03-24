using System.Collections.Generic;
using System.Linq;

using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit;
using TwitchToolkit.IRC;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnWorkCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetOrFindPawn(message.User);

            if(pawn.workSettings != null && pawn.workSettings.EverWork)
            {
                var container = new List<string>();
                var priorities = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder;

            if (TkSettings.SortWorkPriorities)
            {
                priorities = priorities.OrderByDescending(p => pawn.workSettings.GetPriority(p))
                    .ThenBy(p => p.naturalPriority)
                    .Reverse();
            }

                foreach(var priority in priorities)
                {
                    var p = pawn.workSettings.GetPriority(priority);

                if (TkSettings.FilterWorkPriorities)
                {
                    if (p <= 0)
                    {
                        continue;
                    }
                }

                    container.Add(
                        "TKUtils.Formats.PawnWork.Work".Translate(
                            priority.ToString().Named("NAME"),
                            p.Named("VALUE")
                        )
                    );
                }

                if(container.Count > 0)
                {
                    SendCommandMessage(
                        "TKUtils.Formats.PawnWork.Base".Translate(
                            string.Join(
                                "TKUtils.Misc.Separators.Inner".Translate(),
                                container
                            ).Named("PRIORITIES")
                        ),
                        message
                    );
                }
            }
            else
            {
                SendCommandMessage(
                    "TKUtils.Responses.PawnWork.None".Translate(),
                    message
                );
            }
        }
    }
}
