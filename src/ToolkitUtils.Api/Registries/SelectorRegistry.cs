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

/// <summary>
///     A registry specifically designed to manage and organize selector instances. This registry provides a
///     specialized environment for selectors that conform to <see cref="ISelector" />, enabling structured access and
///     operations on the selectors universal to the mod's functionalities.
/// </summary>
[PublicAPI]
[StaticConstructorOnStartup]
public record SelectorRegistry(IReadOnlyList<ISelector> AllRegistrants) : FrozenRegistry<ISelector>(AllRegistrants)
{
    private static readonly IReadOnlyList<ISelector> Selectors;

    static SelectorRegistry()
    {
        var container = new List<ISelector>();

        foreach (Type type in typeof(ISelector).AllSubclassesNonAbstract())
        {
            if (Activator.CreateInstance(type) is not ISelector selector) continue;

            container.Add(selector);
        }

        Selectors = container;
    }

    /// <summary>
    ///     Creates a new instance of a selector registry preloaded with all registered <see cref="ISelector" />
    ///     implementations.
    /// </summary>
    /// <returns>A <see cref="SelectorRegistry" /> instance containing all preloaded selectors.</returns>
    public static SelectorRegistry CreateDefault() => new(Selectors);
}
