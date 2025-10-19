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
using System.Linq;
using Verse;

namespace ToolkitUtils.Mod.Extensions;

/// <summary>Provides extension methods for working with <see cref="ModMetaData" /> objects.</summary>
/// <remarks>
///     This class contains utility methods to facilitate conversion and interaction between
///     <see cref="ModMetaData" /> and domain-level representations such as <see cref="Mod" />.
/// </remarks>
public static class ModMetaDataExtensions
{
    /// <summary>Converts a <see cref="ModMetaData" /> instance to a <see cref="Mod" /> instance.</summary>
    /// <param name="metadata">The <see cref="ModMetaData" /> instance containing the mod's metadata.</param>
    /// <returns>A <see cref="Mod" /> instance created from the metadata.</returns>
    public static Mod ToMod(this ModMetaData metadata) =>
        new(
            metadata.PackageId,
            metadata.Name,
            string.IsNullOrWhiteSpace(metadata.ModVersion) ? SemanticVersion.Zero : SemanticVersion.Parse(metadata.ModVersion),
            metadata.Authors.ToArray()
        );
}
