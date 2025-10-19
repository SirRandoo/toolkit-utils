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
using System.Collections.Concurrent;
using System.Linq;
using Newtonsoft.Json;
using Verse;

namespace ToolkitUtils.Mod.Serialization.Converters;

internal sealed class ModConverter : JsonConverter<Mod>
{
    private static readonly string[] EmptyAuthorsArray = ["Unknown",];
    private static readonly SemanticVersion EmptyVersion = new(Major: 0, Minor: 0, Patch: 0);
    private readonly ConcurrentDictionary<string, Mod> _mods = new();

    public override void WriteJson(JsonWriter writer, Mod value, JsonSerializer serializer)
    {
        writer.WriteValue(value.Id);
    }

    public override Mod ReadJson(JsonReader reader, Type objectType, Mod existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string id = reader.ReadAsString();

        if (_mods.TryGetValue(id, out Mod mod)) return mod;

        ModContentPack? modContentPack = LoadedModManager.RunningModsListForReading.Find(m => string.Equals(m.PackageId, id, StringComparison.OrdinalIgnoreCase));
        mod = modContentPack == null ? new Mod(id, id, EmptyVersion, EmptyAuthorsArray) : Extract(id, modContentPack);

        _mods.TryAdd(id, mod);

        return mod;
    }

    private static Mod Extract(string id, ModContentPack modContentPack)
    {
        string modName = modContentPack.Name ?? id;
        SemanticVersion modVersion = EmptyVersion;
        string[] modAuthors = EmptyAuthorsArray;

        if (modContentPack.ModMetaData == null) return new Mod(id, modName, modVersion, modAuthors);

        modVersion = SemanticVersion.Parse(modContentPack.ModMetaData.ModVersion ?? "0.0.0");
        modAuthors = modContentPack.ModMetaData.Authors.ToArray();

        return new Mod(id, modName, modVersion, modAuthors);
    }
}
