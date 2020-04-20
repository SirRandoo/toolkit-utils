using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnWorkCommand : CommandBase
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
                twitchCommand.Reply("TKUtils.Responses.NoPawn".Translate().WithHeader("TKUtils.Headers.Work".Translate()));
                return false;
            }

            if (pawn.workSettings != null && (!(!pawn.workSettings?.EverWork ?? true)))
            {
                return true;
            }

            twitchCommand.Reply(
                "TKUtils.Responses.PawnWork.None".Translate().WithHeader("TKUtils.Headers.Work".Translate())
            );
            return false;
        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
            var container = new List<string>();
            var priorities = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder;

            if (TkSettings.SortWorkPriorities)
            {
                priorities = priorities.OrderByDescending(p => pawn.workSettings.GetPriority(p))
                    .ThenBy(p => p.naturalPriority)
                    .Reverse();
            }

            foreach (var priority in priorities.ToList())
            {
                var p = pawn.workSettings.GetPriority(priority);

                if (TkSettings.FilterWorkPriorities)
                {
                    if (p <= 0)
                    {
                        continue;
                    }
                }

                container.Add(
                    "TKUtils.Formats.KeyValue".Translate(
                        priority.LabelCap.NullOrEmpty()
                            ? priority.defName.CapitalizeFirst()
                            : priority.LabelCap.RawText,
                        p
                    )
                );
            }

            if (container.Count > 0)
            {
                twitchCommand.Reply(string.Join(", ", container).WithHeader("TKUtils.Headers.Work".Translate()));
            }
        }

        public PawnWorkCommand(ToolkitChatCommand command) : base(command)
        {
        }
    }
}
