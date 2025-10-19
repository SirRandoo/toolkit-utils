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
using JetBrains.Annotations;

namespace ToolkitUtils.Mod;

/// <summary>Encapsulates metadata information related to a plugin, including its unique identifier, name, and version.</summary>
/// <remarks>
///     Plugin metadata facilitates the identification and management of plugins within the system. It provides key
///     descriptive details about a plugin necessary for versioning, registration, and compatibility checks.
/// </remarks>
[PublicAPI]
public sealed record PluginMetadata(string Id, string Name, SemanticVersion Version) : IIdentifiable;
