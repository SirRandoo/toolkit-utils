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
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using JetBrains.Annotations;
using ToolkitUtils.Mod;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Api;

/// <summary>A registry for housing colors used by the mod to facilitate color-based interaction.</summary>
[PublicAPI]
[StaticConstructorOnStartup]
public record ColorRegistry(IReadOnlyList<NamedColor> AllRegistrants) : FrozenRegistry<NamedColor>(AllRegistrants)
{
    private static readonly IReadOnlyList<NamedColor> ColorLibrary;

    static ColorRegistry()
    {
        var container = new List<NamedColor>();
        var serializer = new XmlSerializer(typeof(Library));

        for (var i = 0; i < FilePaths.DataFolderLoadPaths.Count; i++)
        {
            string colorFilePath = Path.Combine(FilePaths.DataFolderLoadPaths[i], path2: "Color.xml");

            if (!File.Exists(colorFilePath)) continue;

            using (var stream = new FileStream(colorFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, FileOptions.SequentialScan))
            {
                if (serializer.Deserialize(stream) is not Library library) continue;

                for (var j = 0; j < library.Colors.Length; j++)
                {
                    Entry colorEntry = library.Colors[j];

                    if (!ColorUtility.TryParseHtmlString(colorEntry.Value, out Color color)) continue;

                    container.Add(new NamedColor(colorEntry.Name, color));
                }
            }
        } 

        ColorLibrary = container;
    }

    /// <summary>Creates a default instance of the ColorRegistry using a predefined set of colors.</summary>
    /// <returns>A new ColorRegistry instance initialized with the color library.</returns>
    public static ColorRegistry CreateDefault() => new(ColorLibrary);

    [XmlRoot("ColorLibrary")]
    private sealed class Library
    {
        [XmlArrayItem("Color")] public Entry[] Colors { get; set; } = [];
    }

    private sealed class Entry
    {
        [XmlAttribute("Name")] public string Name { get; set; }
        [XmlAttribute("Value")] public string Value { get; set; }
    }
}
