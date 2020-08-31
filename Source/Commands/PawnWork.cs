using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnWork : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize().WithHeader("TKUtils.PawnWork.Header".Localize()));
                return;
            }

            if (pawn.workSettings == null || (!pawn.workSettings?.EverWork ?? true))
            {
                twitchMessage.Reply(
                    "TKUtils.PawnWork.None".Localize().WithHeader("TKUtils.PawnWork.Header".Localize())
                );
                return;
            }

            List<WorkTypeDef> priorities = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToList();

            if (TkSettings.SortWorkPriorities)
            {
                priorities = SortPriorities(priorities, pawn);
            }

            HideDisabledWork(priorities);

            List<string> container = priorities.ToList()
               .Select(priority => new {priority, p = pawn.workSettings.GetPriority(priority)})
               .Where(t => !TkSettings.FilterWorkPriorities || t.p > 0)
               .Select(
                    t => ResponseHelper.JoinPair(
                        t.priority.LabelCap.NullOrEmpty()
                            ? t.priority.defName.CapitalizeFirst()
                            : t.priority.LabelCap.RawText,
                        t.p.ToString()
                    )
                )
               .ToList();

            if (container.Count > 0)
            {
                twitchMessage.Reply(container.SectionJoin().WithHeader("TKUtils.PawnWork.Header".Localize()));
            }
        }

        private static void HideDisabledWork(IList<WorkTypeDef> priorities)
        {
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
                catch (IndexOutOfRangeException) { }
            }
        }

        private static List<WorkTypeDef> SortPriorities(List<WorkTypeDef> priorities, Pawn pawn)
        {
            priorities = priorities.OrderByDescending(p => pawn.workSettings.GetPriority(p))
               .ThenBy(p => p.naturalPriority)
               .Reverse()
               .ToList();
            return priorities;
        }
    }
}
