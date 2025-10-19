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
using JetBrains.Annotations;
using ToolkitUtils.Mod.Presentation;
using Verse;

namespace ToolkitUtils.Api;

/// <summary>A registry for housing mutators used by the mod to facilitate user interactions.</summary>
[PublicAPI]
[StaticConstructorOnStartup]
public record MutatorRegistry(IReadOnlyList<IMutator> AllRegistrants) : FrozenRegistry<IMutator>(AllRegistrants)
{
    private static readonly IReadOnlyList<IMutator> Mutators;

    static MutatorRegistry()
    {
        var container = new List<IMutator>();

        foreach (Type type in typeof(IMutator).AllSubclassesNonAbstract())
        {
            if (Activator.CreateInstance(type) is not IMutator selector) continue;

            container.Add(selector);
        }

        Mutators = container;
    }

    /// <summary>Creates a new instance of a mutator registry preloaded with all mutator implementations.</summary>
    public static MutatorRegistry CreateDefault() => new(Mutators);
}
