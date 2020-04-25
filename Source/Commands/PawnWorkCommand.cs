using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnWorkCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate().WithHeader("TKUtils.Headers.Work".Translate()));
                return;
            }

            if (pawn.workSettings == null || (!pawn.workSettings?.EverWork ?? true))
            {
                twitchMessage.Reply(
                    "TKUtils.Responses.PawnWork.None".Translate().WithHeader("TKUtils.Headers.Work".Translate())
                );
                return;
            }

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
                twitchMessage.Reply(string.Join(", ", container).WithHeader("TKUtils.Headers.Work".Translate()));
            }
        }
    }
}
