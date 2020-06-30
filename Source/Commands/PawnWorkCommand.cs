using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnWorkCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply(
                    "TKUtils.Responses.NoPawn".TranslateSimple().WithHeader("TKUtils.Headers.Work".TranslateSimple())
                );
                return;
            }

            if (pawn.workSettings == null || (!pawn.workSettings?.EverWork ?? true))
            {
                twitchMessage.Reply(
                    "TKUtils.Responses.PawnWork.None".TranslateSimple()
                        .WithHeader("TKUtils.Headers.Work".TranslateSimple())
                );
                return;
            }

            var container = new List<string>();
            List<WorkTypeDef> priorities = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToList();

            if (TkSettings.SortWorkPriorities)
            {
                priorities = priorities.OrderByDescending(p => pawn.workSettings.GetPriority(p))
                    .ThenBy(p => p.naturalPriority)
                    .Reverse()
                    .ToList();
            }

            for (int index = priorities.Count - 1; index >= 0; index--)
            {
                WorkTypeDef priority = priorities[index];
                TkSettings.WorkSetting setting =
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

            foreach (WorkTypeDef priority in priorities.ToList())
            {
                int p = pawn.workSettings.GetPriority(priority);

                if (TkSettings.FilterWorkPriorities)
                {
                    if (p <= 0)
                    {
                        continue;
                    }
                }

                container.Add(
                    ResponseHelper.JoinPair(
                        priority.LabelCap.NullOrEmpty()
                            ? priority.defName.CapitalizeFirst()
                            : priority.LabelCap.RawText,
                        p.ToString()
                    )
                );
            }

            if (container.Count > 0)
            {
                twitchMessage.Reply(container.SectionJoin().WithHeader("TKUtils.Headers.Work".TranslateSimple()));
            }
        }
    }
}
