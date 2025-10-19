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

internal sealed class SkillGainConverter : JsonConverter<SkillGain>
{
    private const string IdPropertyName = "id";
    private const string NamePropertyName = "name";
    private const string AmountPropertyName = "amount";

    public override bool CanRead => false;

    public override void WriteJson(JsonWriter writer, SkillGain value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(IdPropertyName);
        writer.WriteValue(value.skill.defName);
        writer.WritePropertyName(NamePropertyName);
        writer.WriteValue(value.skill.label);
        writer.WritePropertyName(AmountPropertyName);
        writer.WriteValue(value.amount);
        writer.WriteEndObject();
    }

    public override SkillGain ReadJson(JsonReader reader, Type objectType, SkillGain existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        throw new InvalidOperationException("Reading skill gains is unsupported.");
}
