using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;
using Viewers = TwitchToolkit.Viewers;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnFixAllCommand : CommandBase
    {
        public PawnFixAllCommand(ToolkitChatCommand command) : base(command)
        {
        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
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
            
            twitchCommand.Reply("TKUtils.Responses.PawnFixAll.Relinked".Translate());
        }
    }
}
