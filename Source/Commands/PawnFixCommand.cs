using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnFixCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate());
                return;
            }

            var name = pawn.Name as NameTriple;

            if (name?.Nick != twitchMessage.Username)
            {
                pawn.Name = new NameTriple(name?.First ?? "", twitchMessage.Username, name?.Last ?? "");
            }

            twitchMessage.Reply("TKUtils.Responses.PawnFix.Relinked".Translate());
        }
    }
}
