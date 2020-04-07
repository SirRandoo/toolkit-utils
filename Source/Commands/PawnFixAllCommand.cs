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
                GetOrFindPawn(viewer.username);
            }

            message.Reply("TKUtils.Responses.PawnFixAll.Relinked".Translate());
        }
    }
}
