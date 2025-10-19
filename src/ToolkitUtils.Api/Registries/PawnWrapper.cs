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
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using JetBrains.Annotations;
using Verse;

namespace ToolkitUtils.Api;

/// <summary>
///     The PawnWrapper class is a record designed to wrap a Pawn object, extending the functionality provided by the
///     ThingWrapper class specifically for Pawn instances.
/// </summary>
/// <remarks>
///     This wrapper allows interaction with Pawn instances while providing a consistent interface for identification,
///     leveraging the inheritance from ThingWrapper.
/// </remarks>
[PublicAPI]
public record PawnWrapper(Pawn Pawn) : ThingWrapper<Pawn>(Pawn)
{
    /// <inheritdoc />
    public override string Id => Pawn.LabelShort;
}
