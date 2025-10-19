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
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ToolkitUtils.Mod.Extensions;
using Verse;

namespace ToolkitUtils.Api;

using Mod = Mod.Mod;

/// <summary>A registry for housing mod definitions used by the mod to facilitate viewer interactions.</summary>
[PublicAPI]
[StaticConstructorOnStartup]
public record ModRegistry(IReadOnlyList<Mod> AllRegistrants) : FrozenRegistry<Mod>(AllRegistrants)
{
    private static readonly Version DefaultVersion = new(major: 1, minor: 0, build: 0);
    private static readonly IReadOnlyList<Mod> Mods;

    static ModRegistry()
    {
        Mods = ModsConfig.ActiveModsInLoadOrder.Select(ModMetaDataExtensions.ToMod).ToList();
    }

    /// <summary>Creates a new instance of a mod registry preloaded with all the active mods.</summary>
    public static ModRegistry CreateDefault() => new(Mods);
}
