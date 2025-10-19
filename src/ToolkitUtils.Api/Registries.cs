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
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Products;

namespace ToolkitUtils.Api;

[PublicAPI]
public static class Registries
{
    public static readonly ColorRegistry Colors = ColorRegistry.CreateDefault();
    public static readonly SynchronisedRegistry<Viewer> Viewers = new(); // FilePaths.ViewerFile
    public static readonly SynchronisedRegistry<ItemProduct> Items = new();
    public static readonly SynchronisedRegistry<EventProduct> Events = new();
    public static readonly SynchronisedRegistry<TraitProduct> Traits = new();
    public static readonly SynchronisedRegistry<PawnProduct> Pawns = new();
    public static readonly ModRegistry Mods = ModRegistry.CreateDefault();
    public static readonly CompatibilityRegistry Compatibilities = CompatibilityRegistry.CreateDefault();
}
