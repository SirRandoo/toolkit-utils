// MIT License
// 
// Copyright (c) 2023 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.IO;
using System.Reflection;
using System.Xml;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace ToolkitUtils.Api.Extensions
{
    public static class ModMetaDataExtensions
    {
        private static readonly FieldInfo MetaInternalField = AccessTools.Field("Verse.ModMetaData:meta");
        private static readonly FieldInfo MetaInternalAuthorField = AccessTools.Field("Verse.ModMetaData.ModMetaDataInternal:author");
        private static readonly FieldInfo MetaInternalAuthorsField = AccessTools.Field("Verse.ModMetaData.ModMetaDataInternal:authors");

        /// <summary>
        ///     Returns an array containing the authors for a given mod.
        /// </summary>
        /// <param name="metaData">
        ///     A metadata instance containing the information
        ///     for the mod, like the authors.
        /// </param>
        /// <returns>
        ///     This method will always return an array, even if there are no
        ///     authors. Should a mod not have any authors the array will only
        ///     contain the value "Anonymous".
        /// </returns>
        [NotNull]
        public static string[] GetAuthors(this ModMetaData metaData)
        {
            object @internal = MetaInternalField.GetValue(metaData);
            string author = MetaInternalAuthorField.GetValue(@internal) as string ?? "Anonymous";
            string[] authors = MetaInternalAuthorsField.GetValue(@internal) as string[] ?? Array.Empty<string>();

            var container = new string[authors.Length + 1];
            container[0] = author;

            for (var i = 0; i < authors.Length; i++)
            {
                container[i + 1] = authors[i];
            }

            return container;
        }

        /// <summary>
        ///     Returns the name of the given mod.
        /// </summary>
        /// <param name="metaData">
        ///     A metadata instance containing the information
        ///     for the mod, like its name.
        /// </param>
        /// <returns>
        ///     Returns the name of the given mod. The string returned will
        ///     either be the mod's name on the Steam workshop, the mod's name as
        ///     defined within the mod's About.xml file, or the mod's package id.
        /// </returns>
        public static string GetName([NotNull] this ModMetaData metaData)
        {
            string workshopName = metaData.GetWorkshopName();

            if (!string.IsNullOrEmpty(workshopName))
            {
                return workshopName;
            }

            return !string.IsNullOrEmpty(metaData.Name) ? metaData.Name : metaData.PackageId;
        }

        /// <summary>
        ///     Returns the version of the mod.
        /// </summary>
        /// <param name="metaData">
        ///     A metadata instance containing the information
        ///     for the mod, like its mod version.
        /// </param>
        /// <returns>
        ///     Returns the version of the mod if available in the mod's about
        ///     file, or through the first assembly containing a
        ///     <see cref="Mod"/> subclass.
        /// </returns>
        public static string GetVersion([NotNull] this ModMetaData metaData)
        {
            if (!string.IsNullOrEmpty(metaData.ModVersion))
            {
                return metaData.ModVersion;
            }

            string manifestVersion = metaData.GetManifestVersion();

            if (!string.IsNullOrEmpty(manifestVersion))
            {
                return manifestVersion;
            }

            foreach (Mod mod in LoadedModManager.ModHandles)
            {
                if (mod.Content.ModMetaData == metaData)
                {
                    return mod.GetType().Assembly.GetVersion();
                }
            }

            return null;
        }

        private static string GetManifestVersion([NotNull] this ModMetaData metaData)
        {
            string url = Path.Combine(metaData.RootDir.ToString(), "About/Manifest.xml");

            if (!File.Exists(url))
            {
                return null;
            }

            using (var reader = new XmlTextReader(url))
            {
                reader.ReadToFollowing("version");

                return reader.ReadElementContentAsString();
            }
        }

        /// <summary>
        ///     Retrieves the mod's id from the disk, or from the workshop api.
        /// </summary>
        /// <param name="metaData">
        ///     A metadata instance containing the information
        ///     for the mod which is used by RimWorld for api requests.
        /// </param>
        public static ulong GetWorkshopId([NotNull] this ModMetaData metaData)
        {
            // We'll prefer to do a disk operation first before making an api
            // request as some mod authors publish the file id alongside their
            // mod when they release a version to Steam.
            string fileIdFile = Path.Combine(metaData.RootDir.ToString(), "PublishedFileId.txt");

            if (!File.Exists(fileIdFile))
            {
                return metaData.GetPublishedFileId().m_PublishedFileId;
            }

            string contents = File.ReadAllText(fileIdFile);

            return ulong.TryParse(contents, out ulong id) ? id : metaData.GetPublishedFileId().m_PublishedFileId;
        }
    }
}
