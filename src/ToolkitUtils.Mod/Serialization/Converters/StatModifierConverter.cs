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
using RimWorld;

namespace ToolkitUtils.Mod.Serialization.Converters;

internal sealed class StatModifierConverter : JsonConverter<StatModifier>
{
    private const string IdPropertyName = "id";
    private const string NamePropertyName = "name";
    private const string AmountPropertyName = "amount";

    public override bool CanRead => false;

    public override void WriteJson(JsonWriter writer, StatModifier value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(IdPropertyName);
        writer.WriteValue(value.stat.defName);
        writer.WritePropertyName(NamePropertyName);
        writer.WriteValue(value.stat.label);
        writer.WritePropertyName(AmountPropertyName);
        writer.WriteValue(value.value);
        writer.WriteEndObject();
    }

    public override StatModifier ReadJson(JsonReader reader, Type objectType, StatModifier existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        throw new InvalidOperationException("Reading stat modifiers is unsupported.");
}
