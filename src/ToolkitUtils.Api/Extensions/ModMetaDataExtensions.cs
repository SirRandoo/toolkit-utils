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
using System.IO;
using System.Reflection;
using System.Xml;
using HarmonyLib;
using Verse;

namespace ToolkitUtils.Api.Extensions;

/// <summary>
///     A collection of extension methods for reading information stored within <see cref="ModMetaData" /> instances
///     that isn't normally accessible by conventional means. Additionally, this class also provides extensions for easily
///     getting information in a safe way.
/// </summary>
public static class ModMetaDataExtensions
{
    private static readonly FieldInfo MetaInternalField = AccessTools.Field(typeColonName: "Verse.ModMetaData:meta");
    private static readonly FieldInfo MetaInternalAuthorField = AccessTools.Field(typeColonName: "Verse.ModMetaData.ModMetaDataInternal:author");
    private static readonly FieldInfo MetaInternalAuthorsField = AccessTools.Field(typeColonName: "Verse.ModMetaData.ModMetaDataInternal:authors");

    /// <summary>Returns an array containing the authors for a given mod.</summary>
    /// <param name="metaData">A metadata instance containing the information for the mod, like the authors.</param>
    /// <returns>
    ///     This method will always return an array, even if there are no authors. Should a mod not have any authors the
    ///     array will only contain the value "Anonymous".
    /// </returns>
    public static string[] GetAuthors(this ModMetaData metaData)
    {
        object @internal = MetaInternalField.GetValue(obj: metaData);
        string author = MetaInternalAuthorField.GetValue(obj: @internal) as string ?? "Anonymous";
        string[] authors = MetaInternalAuthorsField.GetValue(obj: @internal) as string[] ?? [];

        var container = new string[authors.Length + 1];
        container[0] = author;

        for (var i = 0; i < authors.Length; i++) container[i + 1] = authors[i];

        return container;
    }

    /// <summary>Returns the name of the given mod.</summary>
    /// <param name="metaData">A metadata instance containing the information for the mod, like its name.</param>
    /// <returns>
    ///     Returns the name of the given mod. The string returned will either be the mod's name on the Steam workshop,
    ///     the mod's name as defined within the mod's About.xml file, or the mod's package id.
    /// </returns>
    public static string GetName(this ModMetaData metaData)
    {
        string workshopName = metaData.GetWorkshopName();

        if (!string.IsNullOrEmpty(value: workshopName)) return workshopName;

        return !string.IsNullOrEmpty(value: metaData.Name) ? metaData.Name : metaData.PackageId;
    }

    /// <summary>Returns the version of the mod.</summary>
    /// <param name="metaData">A metadata instance containing the information for the mod, like its mod version.</param>
    /// <returns>
    ///     Returns the version of the mod if available in the mod's about file, or through the first assembly containing
    ///     a <see cref="Mod" /> subclass.
    /// </returns>
    public static string GetVersion(this ModMetaData metaData)
    {
        if (!string.IsNullOrEmpty(value: metaData.ModVersion)) return metaData.ModVersion;

        string manifestVersion = metaData.GetManifestVersion();

        if (!string.IsNullOrEmpty(value: manifestVersion)) return manifestVersion;

        foreach (Verse.Mod mod in LoadedModManager.ModHandles)
        {
            if (mod.Content.ModMetaData == metaData) return mod.GetType().Assembly.GetVersion();
        }

        return string.Empty;
    }

    private static string GetManifestVersion(this ModMetaData metaData)
    {
        string url = Path.Combine(metaData.RootDir.ToString(), path2: "About/Manifest.xml");

        if (!File.Exists(path: url)) return string.Empty;

        using (var reader = new XmlTextReader(url: url))
        {
            reader.ReadToFollowing(name: "version");

            return reader.ReadElementContentAsString();
        }
    }

    /// <summary>Retrieves the mod's id from the disk, or from the workshop api.</summary>
    /// <param name="metaData">
    ///     A metadata instance containing the information for the mod which is used by RimWorld for api
    ///     requests.
    /// </param>
    public static ulong GetWorkshopId(this ModMetaData metaData)
    {
        // We'll prefer to do a disk operation first before making an api
        // request as some mod authors publish the file id alongside their
        // mod when they release a version to Steam.
        string fileIdFile = Path.Combine(metaData.RootDir.ToString(), path2: "PublishedFileId.txt");

        if (!File.Exists(path: fileIdFile)) return metaData.GetPublishedFileId().m_PublishedFileId;

        string contents = File.ReadAllText(path: fileIdFile);

        return ulong.TryParse(contents, out ulong id) ? id : metaData.GetPublishedFileId().m_PublishedFileId;
    }
}
