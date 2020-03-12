using SirRandoo.ToolkitUtils.Utils;

using TwitchLib.Client.Models;

using TwitchToolkit;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnFixAllCommand : CommandBase
    {
        public override void RunCommand(ChatMessage message)
        {
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            foreach(var viewer in Viewers.All)
            {
                GetPawnDestructive(viewer.username);
            }

            SendCommandMessage(
                "TKUtils.Responses.PawnFixAll.Relinked".Translate(),
                message
            );
        }
    }
}
