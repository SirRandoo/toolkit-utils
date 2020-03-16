using System.Collections.Generic;

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

            var pawn = GetPawnDestructive(message.User);

            if(pawn.workSettings != null && pawn.workSettings.EverWork)
            {
                var container = new List<string>();
                var priorities = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder;

                foreach(var priority in priorities)
                {
                    container.Add(
                        "TKUtils.Formats.PawnWork.Work".Translate(
                            priority.ToString().Named("NAME"),
                            pawn.workSettings.GetPriority(priority).Named("VALUE")
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
