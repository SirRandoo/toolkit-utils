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
using Newtonsoft.Json.Linq;

namespace ToolkitUtils.Mod.Serialization.Converters;

internal sealed class IntegerRangeConverter : JsonConverter<IntegerRange>
{
    private const string MaximumPropertyName = "maximum";
    private const string MinimumPropertyName = "minimum";

    public override void WriteJson(JsonWriter writer, IntegerRange value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(MinimumPropertyName);
        writer.WriteValue(value.Minimum);
        writer.WritePropertyName(MaximumPropertyName);
        writer.WriteValue(value.Maximum);
        writer.WriteEndObject();
    }

    public override IntegerRange ReadJson(JsonReader reader, Type objectType, IntegerRange existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType is not JsonToken.StartObject) throw new JsonSerializationException("Expected start object");

        JObject obj = JObject.Load(reader);
        var minimum = obj.Property(MinimumPropertyName).Value<int>();
        var maximum = obj.Property(MaximumPropertyName).Value<int>();

        return new IntegerRange(minimum, maximum);
    }
}
