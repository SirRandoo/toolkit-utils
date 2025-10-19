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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;
using Result = ToolkitUtils.Mod.Result;

namespace ToolkitUtils.Interactions.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class Research : CommandGroup
{
    [Command("research")]
    public async Task<Result> GetResearchAsync(string? query = null)
    {
        if (Current.Game == null) return Result.Fail(TranslationService.Instance.GetNoGameError());

        ResearchProjectDef? currentProject;
        float progress;

        if (string.IsNullOrEmpty(query))
        {
            currentProject = Find.ResearchManager.GetProject();
            progress = await RouterService.Instance.RouteToMainAsync(GetResearchProjectProgress, currentProject);

            return Result.Ok($"{currentProject.LabelCap} -> {progress:P1}");
        }

        ThingDef? item = GetItem(query!);

        if (item == null) return Result.Fail(TranslationService.Instance.FormatInvalidQuery(query!));

        currentProject = item.researchPrerequisites.Find(r => string.Equals(r.label, query, StringComparison.InvariantCultureIgnoreCase));

        if (currentProject == null) return Result.Fail(TranslationService.Instance.GetNotResearchingError());

        progress = await RouterService.Instance.RouteToMainAsync(GetResearchProjectProgress, currentProject);

        var prerequisites = new List<ResearchProjectDef>();
        prerequisites.AddRange(currentProject.prerequisites);
        prerequisites.AddRange(currentProject.hiddenPrerequisites);

        var index = 0;
        var visiblePrerequisites = new string[3];

        for (var i = 0; i < prerequisites.Count; i++)
        {
            ResearchProjectDef projectDef = prerequisites[i];
            float prerequisiteProgress = await RouterService.Instance.RouteToMainAsync(GetResearchProjectProgress, projectDef);

            if (prerequisiteProgress >= 1f) continue;

            visiblePrerequisites[index++] = $"{projectDef.LabelCap} -> {prerequisiteProgress:P1}";

            if (index >= 3) break;
        }

        string response = string.IsNullOrEmpty(visiblePrerequisites[0])
            ? $"{currentProject.LabelCap} -> {progress:P1} | {string.Join(separator: ", ", visiblePrerequisites)}"
            : $"{currentProject.LabelCap} -> {progress:P1}";

        return Result.Ok(response);

        float GetResearchProjectProgress(ResearchProjectDef project) => project.ProgressPercent;
    }

    private static ThingDef? GetItem(string query)
    {
        List<ThingDef> defs = DefDatabase<ThingDef>.AllDefsListForReading;

        for (var i = 0; i < defs.Count; i++)
        {
            ThingDef def = defs[i];

            if (string.Equals(query, def.label, StringComparison.InvariantCultureIgnoreCase)) return def;
        }

        return null;
    }
}
