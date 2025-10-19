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
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Localization;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

[Group("pawn")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PawnSkills(ExecutionContext context) : CommandGroup
{
    [Command("skills")]
    public ValueTask<Result> GetPawnSkillsInformationAsync()
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return new ValueTask<Result>(Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf()));

        var parts = new List<string>();
        var container = new StringBuilder();
        List<SkillRecord> skills = pawn.skills.skills;

        for (var index = 0; index < skills.Count; index++)
        {
            SkillRecord skill = skills[index];

            container.Append(skill.def.LabelCap);
            container.Append(": ");
            container.Append(skill.TotallyDisabled ? UnicodeCharacter.Prohibited.Codepoint : skill.levelInt.ToString("N"));

            parts.Add(container.ToString());

            container.Clear();
        }

        return new ValueTask<Result>(Result.Ok(string.Join(separator: ", ", parts)));
    }
}
