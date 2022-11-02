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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SirRandoo.ToolkitUtils
{
    /// <summary>
    ///     A static class that provides json (de)serialization.
    /// </summary>
    public static class Json
    {
        private static readonly JsonSerializer Serializer;
        private static readonly JsonSerializer PrettySerializer;
        private static readonly DefaultContractResolver DefaultContractResolver = new DictionaryContractResolver { NamingStrategy = new DefaultNamingStrategy() };

        static Json()
        {
            Serializer = new JsonSerializer { ContractResolver = DefaultContractResolver, Converters = { new StringEnumConverter() } };

            PrettySerializer = new JsonSerializer
            {
                Formatting = Formatting.Indented, ContractResolver = DefaultContractResolver, Converters = { new StringEnumConverter() }
            };
        }

        /// <summary>
        ///     Deserializes data from a <see cref="Stream"/> into the associated
        ///     object <see cref="T"/>.
        /// </summary>
        /// <param name="stream">The stream to deserialize from</param>
        /// <typeparam name="T">
        ///     The <see cref="System.Type"/> that should contain the
        ///     deserialized data
        /// </typeparam>
        /// <returns>
        ///     The stream's contents deserialized as the type passed
        ///     (<see cref="T"/>), or <c>null</c> if the object could not be
        ///     serialized into the type
        /// </returns>
        [ItemCanBeNull]
        public static async Task<T> DeserializeAsync<T>([NotNull] Stream stream) where T : class
        {
            using (var reader = new StreamReader(stream))
            {
                return await Serializer.DeserializeAsync(reader, typeof(T)) as T;
            }
        }

        /// <summary>
        ///     Serializes data from <see cref="obj"/> into the associated
        ///     <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        ///     A <see cref="Stream"/> instance that will be written to
        /// </param>
        /// <param name="obj">An object to serialize into the given stream</param>
        /// <param name="pretty">
        ///     Whether the contents of returned value will be
        ///     indented.
        /// </param>
        /// <typeparam name="T">
        ///     The <see cref="System.Type"/> that should contains the data to be
        ///     serialized
        /// </typeparam>
        public static async Task SerializeAsync<T>([NotNull] Stream stream, [NotNull] T obj, bool pretty)
        {
            using (var writer = new StreamWriter(stream))
            {
                await (pretty ? PrettySerializer : Serializer).SerializeAsync(writer, obj);
            }
        }

        /// <summary>
        ///     Serializes data from <see cref="obj"/> into a
        ///     <see cref="string"/>.
        /// </summary>
        /// <param name="obj">An object to serialize into the given stream</param>
        /// <param name="pretty">
        ///     Whether the contents of returned value will be
        ///     indented.
        /// </param>
        /// <typeparam name="T">
        ///     The <see cref="System.Type"/> that should contains the data to be
        ///     serialized
        /// </typeparam>
        /// <returns>The object serialized into a string</returns>
        [ItemNotNull]
        public static async Task<string> SerializeAsync<T>([NotNull] T obj, bool pretty)
        {
            var builder = new StringBuilder();

            using (var writer = new StringWriter(builder))
            {
                await (pretty ? PrettySerializer : Serializer).SerializeAsync(writer, obj);
            }

            return builder.ToString();
        }

        /// <summary>
        ///     Deserializes data from a <see cref="Stream"/> into the associated
        ///     object <see cref="T"/>.
        /// </summary>
        /// <param name="stream">The stream to deserialize from</param>
        /// <typeparam name="T">
        ///     The <see cref="System.Type"/> that should contain the
        ///     deserialized data
        /// </typeparam>
        /// <returns>
        ///     The stream's contents deserialized as the type passed
        ///     (<see cref="T"/>), or <c>null</c> if the object could not be
        ///     serialized into the type
        /// </returns>
        [CanBeNull]
        public static T Deserialize<T>([NotNull] Stream stream) where T : class
        {
            using (var reader = new StreamReader(stream))
            {
                return Serializer.Deserialize(reader, typeof(T)) as T;
            }
        }

        /// <summary>
        ///     Serializes data from <see cref="obj"/> into the associated
        ///     <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        ///     A <see cref="Stream"/> instance that will be written to
        /// </param>
        /// <param name="obj">An object to serialize into the given stream</param>
        /// <param name="pretty">
        ///     Whether the contents of returned value will be
        ///     indented.
        /// </param>
        /// <typeparam name="T">
        ///     The <see cref="System.Type"/> that should contains the data to be
        ///     serialized
        /// </typeparam>
        public static void Serialize<T>([NotNull] Stream stream, [NotNull] T obj, bool pretty)
        {
            using (var writer = new StreamWriter(stream))
            {
                (pretty ? PrettySerializer : Serializer).Serialize(writer, obj);
            }
        }

        /// <summary>
        ///     Serializes data from <see cref="obj"/> into a
        ///     <see cref="string"/>.
        /// </summary>
        /// <param name="obj">An object to serialize into the given stream</param>
        /// <param name="pretty">
        ///     Whether the contents of returned value will be
        ///     indented.
        /// </param>
        /// <typeparam name="T">
        ///     The <see cref="System.Type"/> that should contains the data to be
        ///     serialized
        /// </typeparam>
        /// <returns>The object serialized into a string</returns>
        [NotNull]
        public static string Serialize<T>([NotNull] T obj, bool pretty)
        {
            var builder = new StringBuilder();

            using (var writer = new StringWriter(builder))
            {
                (pretty ? PrettySerializer : Serializer).Serialize(writer, obj);
            }

            return builder.ToString();
        }

        private sealed class DictionaryContractResolver : DefaultContractResolver
        {
            /// <inheritdoc/>
            [NotNull]
            protected override JsonDictionaryContract CreateDictionaryContract([NotNull] Type objectType)
            {
                JsonDictionaryContract contract = base.CreateDictionaryContract(objectType);
                contract.DictionaryKeyResolver = propertyName => propertyName;

                return contract;
            }
        }
    }
}
