using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
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
                twitchMessage.Reply(
                    "TKUtils.Responses.NoPawn".Translate().WithHeader("TKUtils.Headers.Work".Translate())
                );
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
            var priorities = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToList();

            if (TkSettings.SortWorkPriorities)
            {
                priorities = priorities.OrderByDescending(p => pawn.workSettings.GetPriority(p))
                    .ThenBy(p => p.naturalPriority)
                    .Reverse()
                    .ToList();
            }

            for (var index = priorities.Count - 1; index >= 0; index--)
            {
                var priority = priorities[index];
                var setting =
                    TkSettings.WorkSettings.FirstOrDefault(p => p.WorkTypeDef.EqualsIgnoreCase(priority.defName));

                if (setting == null)
                {
                    continue;
                }

                if (setting.Enabled)
                {
                    continue;
                }

                try
                {
                    priorities.RemoveAt(index);
                }
                catch (IndexOutOfRangeException)
                {
                }
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
