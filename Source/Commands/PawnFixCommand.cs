using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit.IRC;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnFixCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            var pawn = GetPawnDestructive(message.User);

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
