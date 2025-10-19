// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Interactions.Commands. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Domain.Settings;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

[Group("pawn")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PawnWork(WorkSettings settings, ExecutionContext context) : CommandGroup
{
    [Command("work")]
    public async Task<Result> ExecuteAsync(params string[] workChanges)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());
        if (pawn.workSettings is not { EverWork: true, }) return Result.Fail(TranslationService.Instance.GetTranslation("TKUtils.Errors.IncapableOfWork"));

        if (workChanges.Length > 0)
        {
            var builder = new StringBuilder();

            foreach (string? change in ProcessChangeRequests(pawn, workChanges))
            {
                builder.Append(change);
                builder.Append(", ");
            }

            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 2, length: 2);

                return Result.Ok($"The following work priorities have been changed: {builder}".MarkNotTranslated());
            }
        }

        string? summary = GetWorkPrioritySummary(pawn);

        if (summary.NullOrEmpty()) return Result.Fail(new Translation("No work summary could be given.".MarkNotTranslated()));

        return Result.Ok(
            TranslationService.Instance.GetTranslation("TKUtils.Responses.PawnWork.WorkSummaryResponse").Format(
                new
                {
                    Summary = summary,
                }
            )
        );
    }

    private static IEnumerable<string> ProcessChangeRequests(Pawn pawn, string[] args)
    {
        List<WorkTypeDef> workTypes = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.Where(w => !pawn.WorkTypeIsDisabled(w)).ToList();

        for (var index = 0; index < args.Length; index++)
        {
            string argument = args[index];
            int indexOf = argument.IndexOf('=');

            if (indexOf == -1) continue;

            string key = argument[indexOf..];
            string value = argument[(indexOf + 1)..];

            WorkTypeDef? workType = workTypes.Find(w => w.label.EqualsIgnoreCase(key) || w.defName.EqualsIgnoreCase(key));

            if (workType == null || !int.TryParse(value, out int parsed)) continue;

            int old = pawn.workSettings.GetPriority(workType);
            int @new = Mathf.Clamp(parsed, min: 0, Pawn_WorkSettings.LowestPriority);
            pawn.workSettings.SetPriority(workType, @new);

            yield return $"{workType.label ?? workType.defName}: {old} {UnicodeCharacter.RightArrow} {@new}";
        }
    }

    private string? GetWorkPrioritySummary(Pawn pawn)
    {
        List<WorkTypeDef> priorities = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToList();

        if (settings.ShouldSortTypes) priorities = SortPriorities(priorities, pawn);

        List<string> container = priorities.Select(priority => new
                                                {
                                                    priority, p = pawn.workSettings.GetPriority(priority),
                                                }
                                            ).Where(t => !settings.ShouldFilterTypes || t.p > 0)
                                           .Select(t => $"{(string.IsNullOrEmpty(t.priority.label) ? t.priority.defName : t.priority.LabelCap)}: {t.p.ToString()}")
                                           .ToList();

        return container.Count > 0 ? string.Join(separator: ", ", container) : null;
    }

    private static List<WorkTypeDef> SortPriorities(IEnumerable<WorkTypeDef> priorities, Pawn pawn)
    {
        return priorities.OrderByDescending(p => pawn.workSettings.GetPriority(p)).ThenBy(p => p.naturalPriority).Reverse().ToList();
    }
}
