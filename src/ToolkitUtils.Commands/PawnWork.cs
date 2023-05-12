// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnWork : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".TranslateSimple().WithHeader("TKUtils.PawnWork.Header".TranslateSimple()));

                return;
            }

            if (pawn!.workSettings?.EverWork == false)
            {
                twitchMessage.Reply("TKUtils.PawnWork.None".TranslateSimple().WithHeader("TKUtils.PawnWork.Header".TranslateSimple()));

                return;
            }

            List<KeyValuePair<string, string>> newPriorities = CommandParser.ParseKeyed(twitchMessage.Message);

            if (!newPriorities.NullOrEmpty())
            {
                var builder = new StringBuilder();

                foreach (string change in ProcessChangeRequests(pawn, newPriorities))
                {
                    builder.Append(change);
                    builder.Append(", ");
                }

                if (builder.Length > 0)
                {
                    builder.Remove(builder.Length - 2, 2);
                    twitchMessage.Reply("TKUtils.PawnWork.Changed".Translate(builder.ToString()));

                    return;
                }
            }

            string summary = GetWorkPrioritySummary(pawn);

            if (summary.NullOrEmpty())
            {
                return;
            }

            twitchMessage.Reply(summary.WithHeader("TKUtils.PawnWork.Header".TranslateSimple()));
        }

        [ItemNotNull]
        private static IEnumerable<string> ProcessChangeRequests(Pawn pawn, [NotNull] IEnumerable<KeyValuePair<string, string>> rawChanges)
        {
            List<WorkTypeDef> workTypes = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.Where(w => !pawn.WorkTypeIsDisabled(w)).ToList();

            foreach (KeyValuePair<string, string> pair in rawChanges)
            {
                string key = pair.Key;
                string value = pair.Value;
                WorkTypeDef workType = workTypes.Find(w => w.label.EqualsIgnoreCase(key) || w.defName.EqualsIgnoreCase(key));

                if (workType == null || !int.TryParse(value, out int parsed))
                {
                    continue;
                }

                int old = pawn.workSettings.GetPriority(workType);
                int @new = Mathf.Clamp(parsed, 0, Pawn_WorkSettings.LowestPriority);
                pawn.workSettings.SetPriority(workType, @new);

                yield return $"{workType.label ?? workType.defName}: {old} {ResponseHelper.ArrowGlyph.AltText("->")} {@new}";
            }
        }

        [CanBeNull]
        private static string GetWorkPrioritySummary(Pawn pawn)
        {
            List<WorkTypeDef> priorities = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToList();

            if (TkSettings.SortWorkPriorities)
            {
                priorities = SortPriorities(priorities, pawn);
            }

            List<string> container = priorities.Select(priority => new { priority, p = pawn.workSettings.GetPriority(priority) })
               .Where(t => !TkSettings.FilterWorkPriorities || t.p > 0)
               .Select(t => ResponseHelper.JoinPair(t.priority.LabelCap.NullOrEmpty() ? t.priority.defName.CapitalizeFirst() : t.priority.LabelCap.RawText, t.p.ToString()))
               .ToList();

            return container.Count > 0 ? container.SectionJoin() : null;
        }

        [NotNull]
        private static List<WorkTypeDef> SortPriorities([NotNull] IEnumerable<WorkTypeDef> priorities, Pawn pawn)
        {
            return priorities.OrderByDescending(p => pawn.workSettings.GetPriority(p)).ThenBy(p => p.naturalPriority).Reverse().ToList();
        }
    }
}
