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
using Verse;

namespace ToolkitUtils.Mod.Serialization.Converters;

internal sealed class FlagsConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is not Enum @enum) throw new JsonSerializationException("Type is not a flags enum");

        writer.WriteStartArray();

        foreach (Enum e in Enum.GetValues(value.GetType()))
            if (@enum.HasFlag(e))
                writer.WriteValue(e.ToString());

        writer.WriteEndArray();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JArray array = JArray.Load(reader);

        var container = 0;

        foreach (string v in array)
        {
            if (!Enum.IsDefined(objectType, v)) throw new JsonSerializationException("Flag value isn't defined");

            container |= (int)Enum.Parse(objectType, v);
        }

        return container;
    }

    public override bool CanConvert(Type objectType) => objectType.HasAttribute<FlagsAttribute>();
}
