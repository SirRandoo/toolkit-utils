using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IRC;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnFixCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetOrFindPawn(message.User);

            if (pawn != null)
            {
                var name = pawn.Name as NameTriple;

                if (name?.Nick != message.User)
                {
                    pawn.Name = new NameTriple(name?.First ?? "", message.User, name?.Last ?? "");
                }
            }

            message.Reply(
                pawn == null
                    ? "TKUtils.Responses.NoPawn".Translate()
                    : "TKUtils.Responses.PawnFix.Relinked".Translate()
            );
        }
    }
}
