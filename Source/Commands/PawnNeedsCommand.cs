using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnNeedsCommand : CommandBase
    {
        private Pawn pawn;
        
        public override bool CanExecute(ITwitchCommand twitchCommand)
        {
            if (!base.CanExecute(twitchCommand))
            {
                return false;
            }

            pawn = GetOrFindPawn(twitchCommand.Message);

            if (pawn == null)
            {
                twitchCommand.Reply("TKUtils.Responses.NoPawn".Translate("TabNeeds".Translate()));
                return false;
            }

            if (pawn.needs?.AllNeeds != null)
            {
                return true;
            }

            twitchCommand.Reply("TKUtils.Responses.PawnNeeds.None".Translate().WithHeader("TabNeeds".Translate()));
            return false;
        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
            twitchCommand.Reply(
                string.Join(
                    ", ",
                    pawn.needs.AllNeeds.Select(
                        n => "TKUtils.Formats.KeyValue".Translate(n.LabelCap, n.CurLevelPercentage.ToStringPercent()).RawText
                    ).ToArray()
                ).WithHeader("TabNeeds".Translate())
            );
        }

        public PawnNeedsCommand(ToolkitChatCommand command) : base(command)
        {
        }
    }
}
