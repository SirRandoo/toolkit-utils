using System.Linq;

using SirRandoo.ToolkitUtils.Utils;

using TwitchLib.Client.Models;

using TwitchToolkit;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnWorkCommand : CommandBase
    {
        public override void RunCommand(ChatMessage message)
        {
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetPawnDestructive(message.Username);

            if(pawn.workSettings != null && pawn.workSettings.EverWork)
            {
                var segments = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder
                    .Select(w => "TKUtils.Formats.PawnWork.Work".Translate(
                        w.ToString().Named("NAME"),
                        pawn.workSettings.GetPriority(w).Named("VALUE")
                    )).ToArray();

                if(segments != null)
                {
                    SendCommandMessage(
                        "TKUtils.Formats.PawnWork.Base".Translate(
                            string.Join(
                                "TKUtils.Misc.Separators.Inner".Translate(),
                                segments
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
