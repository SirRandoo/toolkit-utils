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
namespace ToolkitUtils.Mod.Serialization;

/// <summary>Specifies the available data formats for serialization and deserialization operations.</summary>
public enum DataFormat
{
    /// <summary>Represents the TOML data format.</summary>
    /// <remarks>
    ///     This format is used for serializing and deserializing data in the TOML (Tom's Obvious, Minimal Language)
    ///     format, which is commonly utilized for configuration files and supports hierarchical data serialization.
    /// </remarks>
    Toml,

    /// <summary>Represents the JSON data format.</summary>
    /// <remarks>
    ///     This format is used for serializing and deserializing data in the JSON (JavaScript Object Notation) format, a
    ///     lightweight data interchange format that is easy for humans to read and write and easy for machines to parse and
    ///     generate.
    /// </remarks>
    Json,
}
