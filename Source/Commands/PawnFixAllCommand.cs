using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnFixAllCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            foreach (Viewer viewer in Viewers.All)
            {
                Pawn pawn = GetOrFindPawn(viewer.username);

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
