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
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ToolkitUtils.Mod;

/// <summary>Represents a mod entity with unique identifier, name, version, and authors.</summary>
/// <remarks>
///     Instances of <c>Mod</c> encapsulate identifying information about a mod, including its unique identifier,
///     display name, version represented by <c>SemanticVersion</c>, and a list of authors.
/// </remarks>
[PublicAPI]
[JsonObject(MemberSerialization.OptIn)]
public sealed record Mod(
    [property: JsonProperty("id")] string Id,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("version")] SemanticVersion Version,
    [property: JsonProperty("authors")] IReadOnlyList<string> Authors
) : IIdentifiable;
