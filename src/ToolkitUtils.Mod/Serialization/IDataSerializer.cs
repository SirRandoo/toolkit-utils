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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace ToolkitUtils.Mod.Serialization;

/// <summary>
///     Defines a serializer interface for serializing and deserializing data streams to and from objects. Suitable
///     for Dependency Injection to abstract different serialization formats like JSON, XML, etc.
/// </summary>
public interface IDataSerializer
{
    /// <summary>Deserializes the data from a stream into an object of type <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of the object being deserialized.</typeparam>
    /// <param name="stream">The stream containing the serialized data.</param>
    /// <returns>
    ///     The deserialized object of type <typeparamref name="T" />, or the default value of <typeparamref name="T" />
    ///     if deserialization fails.
    /// </returns>
    T? Deserialize<T>(Stream stream);

    /// <summary>Serializes an object of type <typeparamref name="T" /> to a stream.</summary>
    /// <typeparam name="T">The type of the object being serialized.</typeparam>
    /// <param name="stream">The stream to which the object is serialized.</param>
    /// <param name="data">The object to serialize. Must not be null.</param>
    void Serialize<T>(Stream stream, [DisallowNull] T data);

    /// <summary>Asynchronously deserializes the data from a stream into an object of type <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of the object being deserialized.</typeparam>
    /// <param name="stream">The stream containing the serialized data.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the deserialized object of type
    ///     <typeparamref name="T" />, or the default value if deserialization fails.
    /// </returns>
    Task<T?> DeserializeAsync<T>(Stream stream);

    /// <summary>Asynchronously serializes an object of type <typeparamref name="T" /> to a stream.</summary>
    /// <typeparam name="T">The type of the object being serialized.</typeparam>
    /// <param name="stream">The stream to which the object is serialized.</param>
    /// <param name="data">The object to serialize. Must not be null.</param>
    /// <returns>A task representing the asynchronous serialization operation.</returns>
    Task SerializeAsync<T>(Stream stream, [DisallowNull] T data);
}
