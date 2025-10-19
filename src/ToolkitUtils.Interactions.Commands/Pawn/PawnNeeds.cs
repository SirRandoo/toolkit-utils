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
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Presentation;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PawnNeeds(ExecutionContext context) : CommandGroup
{
    [Command("mypawnneeds")]
    public ValueTask<Result> GetPawnNeedsAsync()
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return new ValueTask<Result>(Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf()));

        var builder = new StringBuilder();
        builder.Append(TranslationService.Instance.GetTranslation("TabNeeds"));

        if (pawn.needs.AllNeeds is not { Count: > 0, })
        {
            builder.Append(" ");
            builder.Append(TranslationService.Instance.GetTranslation("TKUtils.Errors.NoPawnNeeds"));

            return new ValueTask<Result>(Result.Ok(builder.ToString()));
        }

        for (var i = 0; i < pawn.needs.AllNeeds.Count; i++)
        {
            Need need = pawn.needs.AllNeeds[i];

            builder.Append(RichTextHelper.StripTags(need.LabelCap));
            builder.Append(need.CurLevelPercentage.ToStringPercent());
            builder.Append(", ");
        }

        return new ValueTask<Result>(Result.Ok(builder.ToString(startIndex: 0, builder.Length - 2)));
    }
}
