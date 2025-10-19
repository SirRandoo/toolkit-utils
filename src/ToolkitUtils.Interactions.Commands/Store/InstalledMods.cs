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
using ToolkitUtils.Api;
using ToolkitUtils.Mod;

namespace ToolkitUtils.Interactions.Commands;

/// <summary>
///     Represents a command group that provides functionality to retrieve and display the list of installed mods in
///     the current execution context.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class InstalledMods(ExecutionContext context) : CommandGroup
{
    private static readonly string StitchedModList = string.Join(separator: ", ", Registries.Mods.AllRegistrants);

    /// <summary>
    ///     Retrieves the list of installed mods and sends it as a reply within the current execution context. The method
    ///     combines the list of all registered mods and sends the result as a reply message to the user who initiated the
    ///     command or operation.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains an <see cref="IResult" /> indicating
    ///     the success or failure status of the operation.
    /// </returns>
    [Command("installedmods")]
    public ValueTask<IResult> GetInstalledModsAsync() => new(Result.Ok(StitchedModList));
}
