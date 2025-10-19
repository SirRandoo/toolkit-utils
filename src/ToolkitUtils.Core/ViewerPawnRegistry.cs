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
// with ToolkitUtils.Core. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using JetBrains.Annotations;
using ToolkitUtils.Api;
using ToolkitUtils.Mod;
using Verse;

namespace ToolkitUtils.Core;

[PublicAPI]
public static class ViewerPawnRegistry
{
    private static readonly SynchronisedRegistry<ViewerPawn> Registry = new();

    public static IReadOnlyList<ViewerPawn> AllRegistrants => Registry.AllRegistrants;

    public static Pawn? Get(string id) => Registry.Get(id)?.Pawn;

    public static void Register(string id, Pawn pawn)
    {
        if (Registry.Get(id) is {} registered)
        {
            Registry.Unregister(registered);
            Registry.Register(
                registered with
                {
                    Pawn = pawn,
                }
            );

            return;
        }

        Registry.Register(new ViewerPawn(id, pawn));
    }

    public static void Unregister(string id)
    {
        if (Registry.Get(id) is {} registered) Registry.Unregister(registered);
    }

    public sealed record ViewerPawn(string Id, Pawn Pawn) : IIdentifiable
    {
        /// <inheritdoc />
        public string Id { get; init; } = Id;

        /// <inheritdoc />
        public string Name { get; init; } = Id;
    }
}
