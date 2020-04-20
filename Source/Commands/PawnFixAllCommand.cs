using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IRC;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnFixAllCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            foreach (var viewer in Viewers.All)
            {
                var pawn = GetOrFindPawn(viewer.username);

                if (pawn == null)
                {
                    continue;
                }

                var name = pawn.Name as NameTriple;

                if (name?.Nick != viewer.username)
                {
                    pawn.Name = new NameTriple(name?.First ?? "", viewer.username, name?.Last ?? "");
                }
            }

            message.Reply("TKUtils.Responses.PawnFixAll.Relinked".Translate());
        }
    }
}
