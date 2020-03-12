using SirRandoo.ToolkitUtils.Utils;

using TwitchLib.Client.Models;

using TwitchToolkit;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnFixCommand : CommandBase
    {
        public override void RunCommand(ChatMessage message)
        {
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetPawnDestructive(message.Username);

            if(pawn == null)
            {
                SendCommandMessage(
                    "TKUtils.Responses.NoPawn".Translate(),
                    message
                );

                return;
            }

            SendCommandMessage("TKUtils.Responses.PawnFix.Relinked".Translate(), message);
        }
    }
}
