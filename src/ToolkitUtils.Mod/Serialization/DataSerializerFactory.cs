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
using System;

namespace ToolkitUtils.Mod.Serialization;

/// <summary>
///     A factory responsible for dynamically resolving <see cref="IDataSerializer" /> instances based on requested
///     data formats, such as JSON or TOML.
/// </summary>
public static class DataSerializerFactory
{
    private static readonly JsonDataSerializer JsonSerializerInstance = new();
    private static readonly TomlDataSerializer TomlSerializerInstance = new();

    /// <summary>Resolves an <see cref="IDataSerializer" /> instance based on the specified <see cref="DataFormat" />.</summary>
    /// <param name="format">The data format for which the serializer instance is required.</param>
    /// <returns>An instance of <see cref="IDataSerializer" /> appropriate for the specified <see cref="DataFormat" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided <paramref name="format" /> is not supported.</exception>
    public static IDataSerializer GetSerializer(DataFormat format)
    {
        return format switch
        {
            DataFormat.Toml => TomlSerializerInstance,
            DataFormat.Json => JsonSerializerInstance,
            var _           => throw new ArgumentOutOfRangeException(nameof(format), format, $"Data format '{format}' is not supported."),
        };
    }
}
