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
using ToolkitUtils.Interactions.Commands.Extensions;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Localization;

namespace ToolkitUtils.Interactions.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class GiveCoins : CommandGroup
{
    [Command("give")]
    public ValueTask<Result> GiveAsync(Viewer viewer, int amount)
    {
        viewer.AddCoins(amount);

        return new ValueTask<Result>(Result.Ok(TranslationService.Instance.FormatGiveCoins(viewer, amount)));
    }
}
