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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using NLog;
using ToolkitUtils.Mod.Logging;

namespace ToolkitUtils.Mod.Serialization;

/// <summary>
///     A sealed class that provides mechanisms for JSON serialization and deserialization. Implements the
///     <see cref="JsonDataSerializer" /> interface to ensure consistency with defined JSON data serialization contracts.
/// </summary>
/// <param name="serializerOptions">Optional settings to customize the behavior of the JSON serialization process.</param>
[PublicAPI]
internal sealed class JsonDataSerializer(JsonSerializerSettings? serializerOptions = null) : IDataSerializer
{
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();
    private readonly JsonSerializer _jsonSerializer = JsonSerializer.CreateDefault(serializerOptions ?? new JsonSerializerSettings());

    /// <inheritdoc />
    public T? Deserialize<T>(Stream stream)
    {
        try
        {
            using (var reader = new StreamReader(stream))
            {
                using (var jReader = new JsonTextReader(reader)) { return _jsonSerializer.Deserialize<T>(jReader)!; }
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, message: "Error deserializing json object from stream");

            return default(T?);
        }
    }

    /// <inheritdoc />
    public void Serialize<T>(Stream stream, [DisallowNull] T data)
    {
        try
        {
            using (var writer = new StreamWriter(stream))
            {
                using (var jWriter = new JsonTextWriter(writer)) { _jsonSerializer.Serialize(jWriter, data, typeof(T)); }
            }
        }
        catch (Exception e) { Logger.Error(e, message: "Error serializing json object into stream"); }
    }

    /// <inheritdoc />
    public async Task<T?> DeserializeAsync<T>(Stream stream)
    {
        try
        {
            using (var reader = new StreamReader(stream))
            {
                using (var jReader = new JsonTextReader(reader)) { return await _jsonSerializer.DeserializeAsync<T>(jReader); }
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, message: "Error deserializing json object from stream");

            return default(T?);
        }
    }

    /// <inheritdoc />
    public async Task SerializeAsync<T>(Stream stream, [DisallowNull] T data)
    {
        try
        {
            await using (var writer = new StreamWriter(stream))
            {
                using (var jWriter = new JsonTextWriter(writer)) { await _jsonSerializer.SerializeAsync(jWriter, data, typeof(T)); }
            }
        }
        catch (Exception e) { Logger.Error(e, message: "Error serializing json object into stream"); }
    }
}
