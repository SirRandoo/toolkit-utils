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
using Newtonsoft.Json;
using ToolkitUtils.Mod.Products;

#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).

namespace ToolkitUtils.Mod.Serialization.Converters;

internal sealed class ProductConverter : JsonConverter<IProduct>
{
    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, IProduct value, JsonSerializer serializer) => serializer.Serialize(writer, value);

    /// <inheritdoc />
    public override IProduct? ReadJson(JsonReader reader, Type objectType, IProduct existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        try
        {
            return serializer.Deserialize(reader, objectType) as IProduct;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
