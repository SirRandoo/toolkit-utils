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

internal sealed class DefConverter : JsonConverter
{
    private const string IdPropertyName = "id";
    private const string NamePropertyName = "name";

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var def = (Def)value;

        writer.WriteStartObject();
        writer.WritePropertyName(IdPropertyName);
        writer.WriteValue(def.defName);
        writer.WritePropertyName(NamePropertyName);
        writer.WriteValue(def.label);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType is not JsonToken.StartObject) throw new JsonSerializationException("Expected start object");

        JObject obj = JObject.Load(reader);
        var defName = obj.Property(IdPropertyName).Value<string>();

        Def? def = GenDefDatabase.GetDef(objectType, defName, errorOnFail: false);

        return def ?? throw new JsonSerializationException($"Could not find def with name {defName}");
    }

    public override bool CanConvert(Type objectType) => typeof(Def).IsAssignableFrom(objectType);
}
