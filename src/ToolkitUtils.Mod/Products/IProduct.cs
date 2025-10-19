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
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using Verse;

namespace ToolkitUtils.Mod.Products;

public interface IProduct : IIdentifiable
{
    /// <summary>A multiplier that scales how much karmic debt the viewer accrues by purchasing the product.</summary>
    float Weight { get; init; }

    /// <summary>A collection of research projects that must be researched before the product can be purchased by viewers.</summary>
    IReadOnlyList<ResearchProjectDef> ResearchPrerequisites { get; init; }
}

public interface IProduct<TMetadata> : IProduct where TMetadata : IProductMetadata
{
    /// <summary>The metadata of the product. This contains additional, read-only data for the product.</summary>
    TMetadata Metadata { get; init; }
}
