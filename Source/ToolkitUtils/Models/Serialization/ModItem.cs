// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using JetBrains.Annotations;
using Verse;
using Verse.Steam;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class ModItem
    {
        [DataMember(Name = "author")] public string Author { get; set; }
        [DataMember(Name = "name")] public string Name { get; set; }
        [DataMember(Name = "steamId")] public string SteamId { get; set; }
        [DataMember(Name = "version")] public string Version { get; set; }

        [NotNull]
        public static ModItem FromMetadata([NotNull] ModMetaData mod)
        {
            var item = new ModItem { Name = mod.Name };

        #if RW12
            item.Author = mod.Author;
        #else
            item.Author = mod.AuthorsString;
        #endif

            Mod handle = LoadedModManager.ModHandles.FirstOrDefault(h => h.Content.PackageId.Equals(mod.PackageId));

            if (handle == null)
            {
                return item;
            }

            item.Version = GetModVersion(handle);
            item.SteamId = GetSteamId(mod, handle);

            return item;
        }

        [NotNull]
        private static string GetSteamId([NotNull] ModMetaData metaData, Mod handle)
        {
            if (metaData.SteamAppId > 0)
            {
                return metaData.SteamAppId.ToString();
            }

            WorkshopItemHook hook = metaData.GetWorkshopItemHook();

            if (hook.PublishedFileId.m_PublishedFileId > 0)
            {
                return hook.PublishedFileId.ToString();
            }

            string steamIdFile = Path.Combine(metaData.RootDir.ToString(), "About/PublishedFileId.txt");

            return File.Exists(steamIdFile) ? File.ReadAllText(steamIdFile) : string.Empty;
        }

        [CanBeNull]
        private static string GetModVersion([NotNull] Mod handle)
        {
            if (TryGetManifestVersion(handle.Content.RootDir, out string version))
            {
                return version;
            }

            var assembly = Assembly.GetAssembly(handle.GetType());

            if (assembly == null)
            {
                return null;
            }

            if (TryGetInfoAssemblyVersion(assembly, out version))
            {
                return version;
            }

            if (TryGetAssemblyFileVersion(assembly, out version))
            {
                return version;
            }

            return TryGetAssemblyVersion(assembly, out version) ? version : handle.GetType().Module.Assembly.GetName().Version.ToString();
        }

        [ContractAnnotation("=> true,version:notnull; => false,version:null")]
        private static bool TryGetManifestVersion([NotNull] string rootDir, out string version)
        {
            string manifestFile = Path.Combine(rootDir, "About/Manifest.xml");

            if (!File.Exists(manifestFile))
            {
                version = null;

                return false;
            }

            using var reader = new XmlTextReader(manifestFile);
            reader.ReadToFollowing("version");

            if (!reader.Name.Equals("version"))
            {
                version = null;

                return false;
            }

            version = reader.ReadElementContentAsString();
            reader.Close();

            return true;
        }

        [ContractAnnotation("=> true,version:notnull; => false,version:null")]
        private static bool TryGetInfoAssemblyVersion([NotNull] Assembly assembly, out string version)
        {
            var attribute = (AssemblyInformationalVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyInformationalVersionAttribute), false);

            if (attribute == null)
            {
                version = null;

                return false;
            }

            version = attribute.InformationalVersion;

            return true;
        }

        [ContractAnnotation("=> true,version:notnull; => false,version:null")]
        private static bool TryGetAssemblyFileVersion([NotNull] Assembly assembly, out string version)
        {
            var attribute = (AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyFileVersionAttribute), false);

            if (attribute == null)
            {
                version = null;

                return false;
            }

            version = attribute.Version;

            return true;
        }

        [ContractAnnotation("=> true,version:notnull; => false,version:null")]
        private static bool TryGetAssemblyVersion([NotNull] Assembly assembly, out string version)
        {
            var attribute = (AssemblyVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyVersionAttribute), false);

            if (attribute == null)
            {
                version = null;

                return false;
            }

            version = attribute.Version;

            return true;
        }
    }
}
