using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using JetBrains.Annotations;
using Verse;
using Verse.Steam;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class ModItem
    {
        public string Author;
        public string Name;
        public string SteamId;
        public string Version;

        public static ModItem FromMetadata(ModMetaData mod)
        {
            var item = new ModItem {Author = mod.Author, Name = mod.Name};
            Mod handle = LoadedModManager.ModHandles.FirstOrDefault(h => h.Content.PackageId.Equals(mod.PackageId));

            if (handle == null)
            {
                return item;
            }

            item.Version = GetModVersion(handle);
            item.SteamId = GetSteamId(mod, handle);

            return item;
        }

        private static string GetSteamId(ModMetaData metaData, Mod handle)
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

        private static string GetModVersion(Mod handle)
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

            return TryGetAssemblyVersion(assembly, out version)
                ? version
                : handle.GetType().Module.Assembly.GetName().Version.ToString();
        }

        private static bool TryGetManifestVersion(string rootDir, out string version)
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

        private static bool TryGetInfoAssemblyVersion(Assembly assembly, out string version)
        {
            var attribute = (AssemblyInformationalVersionAttribute) Attribute.GetCustomAttribute(
                assembly,
                typeof(AssemblyInformationalVersionAttribute),
                false
            );

            if (attribute == null)
            {
                version = null;
                return false;
            }

            version = attribute.InformationalVersion;
            return true;
        }

        private static bool TryGetAssemblyFileVersion(Assembly assembly, out string version)
        {
            var attribute = (AssemblyFileVersionAttribute) Attribute.GetCustomAttribute(
                assembly,
                typeof(AssemblyFileVersionAttribute),
                false
            );

            if (attribute == null)
            {
                version = null;
                return false;
            }

            version = attribute.Version;
            return true;
        }

        private static bool TryGetAssemblyVersion(Assembly assembly, out string version)
        {
            var attribute = (AssemblyVersionAttribute) Attribute.GetCustomAttribute(
                assembly,
                typeof(AssemblyVersionAttribute),
                false
            );

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
