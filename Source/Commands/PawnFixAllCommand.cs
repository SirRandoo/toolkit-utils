using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;
using Viewers = TwitchToolkit.Viewers;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnFixAllCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
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
            
            twitchMessage.Reply("TKUtils.Responses.PawnFixAll.Relinked".Translate());
        }
    }
}
