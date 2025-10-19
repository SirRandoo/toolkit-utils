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
using HarmonyLib;
using JetBrains.Annotations;
using NLog;
using Tomlet;
using ToolkitUtils.Mod.Logging;

namespace ToolkitUtils.Mod.Serialization;

/// <summary>
///     The <c>TomlDataSerializer</c> class provides methods for serializing and deserializing data to and from TOML
///     format.
/// </summary>
/// <remarks>
///     This class implements the <see cref="TomlDataSerializer" /> interface and provides functionality for both
///     synchronous and asynchronous operations.
/// </remarks>
[PublicAPI]
internal sealed class TomlDataSerializer : IDataSerializer
{
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();

    /// <inheritdoc />
    public T? Deserialize<T>(Stream stream)
    {
        try
        {
            using (var reader = new StreamReader(stream)) { return TomletMain.To<T>(reader.ReadToEnd()); }
        }
        catch (Exception e)
        {
            Logger.Error(e, message: "Could not deserialize toml string");

            return default(T?);
        }
    }

    /// <inheritdoc />
    public void Serialize<T>(Stream stream, [DisallowNull] T data)
    {
        try
        {
            using (var writer = new StreamWriter(stream)) { writer.Write(TomletMain.TomlStringFrom(data)); }
        }
        catch (Exception e) { Logger.Error(e, message: "Could not serialize {Type} to toml string", typeof(T).FullDescription()); }
    }

    /// <inheritdoc />
    public async Task<T?> DeserializeAsync<T>(Stream stream)
    {
        try
        {
            using (var reader = new StreamReader(stream)) { return TomletMain.To<T>(await reader.ReadToEndAsync()); }
        }
        catch (Exception e)
        {
            Logger.Error(e, message: "Could not deserialize toml string");

            return default(T?);
        }
    }

    /// <inheritdoc />
    public async Task SerializeAsync<T>(Stream stream, [DisallowNull] T data)
    {
        try
        {
            await using (var writer = new StreamWriter(stream)) { await writer.WriteAsync(TomletMain.TomlStringFrom(data)); }
        }
        catch (Exception e) { Logger.Error(e, message: "Could not serialize {Type} to toml string", typeof(T).FullDescription()); }
    }
}
