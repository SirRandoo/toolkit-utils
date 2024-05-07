// MIT License
//
// Copyright (c) 2022 SirRandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SirRandoo.ToolkitUtils;

/// <summary>
///     A static class that provides json (de)serialization.
/// </summary>
public static class Json
{
    private static readonly JsonSerializer? Serializer;
    private static readonly JsonSerializer? PrettySerializer;
    private static readonly DefaultContractResolver? DefaultContractResolver;
    private static readonly bool MinifyOverride;
    private static readonly bool MinificationOverridden;
    private static readonly bool SerializationDisabled;

    static Json()
    {
        DefaultContractResolver = CreateDefaultDictionaryContractResolver();
        Serializer = CreateDefaultJsonSerializer();
        PrettySerializer = CreateDefaultPrettySerializer();

        if (Serializer == null)
        {
            TkUtils.Logger.Warn("Minified json serializer wasn't properly created. Switching to 'indented' may resolve this issue.");

            MinifyOverride = true;
            MinificationOverridden = true;
        }

        if (PrettySerializer == null)
        {
            TkUtils.Logger.Warn("Indented json serializer wasn't properly created. Switching to 'indented' may resolve this issue.");

            MinifyOverride = false;
            MinificationOverridden = true;
        }

        if (PrettySerializer == null && Serializer == null)
        {
            TkUtils.Logger.Error("Both serializers weren't properly created! Your settings won't be saved between sessions.");
            SerializationDisabled = true;
        }
    }

    private static JsonSerializer? CreateDefaultPrettySerializer()
    {
        try
        {
            return new JsonSerializer { Formatting = Formatting.Indented, ContractResolver = DefaultContractResolver!, Converters = { new StringEnumConverter() } };
        }
        catch (Exception e)
        {
            TkUtils.Logger.Error("Could not create indented json serializer :: Your settings will NOT be saved between sessions! Switching to 'minified' may help.", e);

            return null;
        }
    }

    private static JsonSerializer? CreateDefaultJsonSerializer()
    {
        try
        {
            return new JsonSerializer { ContractResolver = DefaultContractResolver!, Converters = { new StringEnumConverter() } };
        }
        catch (Exception e)
        {
            TkUtils.Logger.Error("Could not create minified json serializer :: Your settings will NOT be saved between sessions!", e);

            return null;
        }
    }

    private static DictionaryContractResolver? CreateDefaultDictionaryContractResolver()
    {
        try
        {
            return new DictionaryContractResolver { NamingStrategy = new DefaultNamingStrategy() };
        }
        catch (Exception e)
        {
            TkUtils.Logger.Error("Could not create dictionary contract resolver :: Command, event, and item settings may be lost between sessions.", e);

            return null;
        }
    }

    /// <summary>
    ///     Deserializes data from a <see cref="Stream" /> into the associated
    ///     object <see cref="T" />.
    /// </summary>
    /// <param name="stream">The stream to deserialize from</param>
    /// <typeparam name="T">The <see cref="System.Type" /> that should contain the deserialized data</typeparam>
    /// <returns>
    ///     The stream's contents deserialized as the type passed (<see cref="T" />), or <c>null</c> if the
    ///     object could not be serialized into the type
    /// </returns>
    public static async Task<T?> DeserializeAsync<T>(Stream stream) where T : class
    {
        if (SerializationDisabled)
        {
            return default;
        }

        using (var reader = new StreamReader(stream))
        {
            return await Serializer!.DeserializeAsync(reader, typeof(T)) as T;
        }
    }

    /// <summary>
    ///     Serializes data from <see cref="obj" /> into the associated <see cref="Stream" />.
    /// </summary>
    /// <param name="stream">A <see cref="Stream" /> instance that will be written to</param>
    /// <param name="obj">An object to serialize into the given stream</param>
    /// <param name="pretty">Whether the contents of returned value will be indented.</param>
    /// <typeparam name="T">The <see cref="System.Type" /> that should contain the data to be serialized</typeparam>
    public static async Task SerializeAsync<T>(Stream stream, [DisallowNull] T obj, bool pretty)
    {
        if (SerializationDisabled)
        {
            return;
        }

        JsonSerializer? serializer = pretty ? PrettySerializer : Serializer;

        if (MinificationOverridden)
        {
            serializer = MinifyOverride ? PrettySerializer : Serializer;
        }

        if (serializer == null)
        {
            return;
        }

        using (var writer = new StreamWriter(stream))
        {
            await serializer.SerializeAsync(writer, obj);
        }
    }

    /// <summary>
    ///     Deserializes data from a <see cref="Stream" /> into the associated object <see cref="T" />.
    /// </summary>
    /// <param name="stream">The stream to deserialize from</param>
    /// <typeparam name="T">The <see cref="System.Type" /> that should contain the deserialized data</typeparam>
    /// <returns>
    ///     The stream's contents deserialized as the type passed (<see cref="T" />), or <c>null</c> if the
    ///     object could not be serialized into the type
    /// </returns>
    public static T? Deserialize<T>(Stream stream) where T : class
    {
        if (SerializationDisabled)
        {
            return default;
        }

        JsonSerializer? serializer = Serializer;

        if (MinificationOverridden)
        {
            serializer = MinifyOverride ? PrettySerializer : Serializer;
        }

        if (serializer == null)
        {
            return default;
        }

        using (var reader = new StreamReader(stream))
        {
            return serializer.Deserialize(reader, typeof(T)) as T;
        }
    }

    /// <summary>
    ///     Serializes data from <see cref="obj" /> into the associated <see cref="Stream" />.
    /// </summary>
    /// <param name="stream">A <see cref="Stream" /> instance that will be written to</param>
    /// <param name="obj">An object to serialize into the given stream</param>
    /// <param name="pretty">Whether the contents of returned value will be indented.</param>
    /// <typeparam name="T">The <see cref="System.Type" /> that should contain the data to be serialized</typeparam>
    public static void Serialize<T>(Stream stream, [DisallowNull] T obj, bool pretty)
    {
        if (SerializationDisabled)
        {
            return;
        }

        JsonSerializer? serializer = pretty ? PrettySerializer : Serializer;

        if (MinificationOverridden)
        {
            serializer = MinifyOverride ? PrettySerializer : Serializer;
        }

        if (serializer == null)
        {
            return;
        }

        using (var writer = new StreamWriter(stream))
        {
            serializer.Serialize(writer, obj);
        }
    }

    private sealed class DictionaryContractResolver : DefaultContractResolver
    {
        /// <inheritdoc />
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            JsonDictionaryContract contract = base.CreateDictionaryContract(objectType);
            contract.DictionaryKeyResolver = propertyName => propertyName;

            return contract;
        }
    }
}
