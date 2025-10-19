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
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using ToolkitUtils.Core;
using ToolkitUtils.Interactions.Commands.Extensions;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Localization;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PawnFix(ExecutionContext context) : CommandGroup
{
    [Command("fixmypawn")]
    public Task<Result> FixPawnAsync()
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Task.FromResult(Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf()));

        if (pawn.Name is not NameTriple name)
            pawn.Name = new NameSingle(context.Invoker.Name);
        else
            pawn.Name = new NameTriple(name.First ?? string.Empty, context.Invoker.Name, name.Last ?? string.Empty);

        return Task.FromResult<Result>(Result.Ok(TranslationService.Instance.FormatPawnFixSingular(context.Invoker)));
    }
}
