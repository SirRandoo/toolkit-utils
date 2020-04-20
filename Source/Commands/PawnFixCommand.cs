using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnFixCommand : CommandBase
    {
        private Pawn pawn;

        public override bool CanExecute(ITwitchCommand twitchCommand)
        {
            if (!base.CanExecute(twitchCommand))
            {
                return false;
            }

            pawn = GetOrFindPawn(twitchCommand.Message);

            if (pawn != null)
            {
                return true;
            }

            twitchCommand.Reply("TKUtils.Responses.NoPawn".Translate());
            return false;
        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
            var name = pawn.Name as NameTriple;

            if (name?.Nick != twitchCommand.Username)
            {
                pawn.Name = new NameTriple(name?.First ?? "", twitchCommand.Username, name?.Last ?? "");
            }

            twitchCommand.Reply("TKUtils.Responses.PawnFix.Relinked".Translate());
        }

        public PawnFixCommand(ToolkitChatCommand command) : base(command)
        {
        }
    }
}
