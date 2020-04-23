using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.Settings;
using UnityEngine;
using Verse;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedType.Global

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    public static class TkUtilsStatic
    {
        static TkUtilsStatic()
        {
            TkUtils.Harmony = new HarmonyLib.Harmony("com.sirrandoo.tkutils");
            TkUtils.Harmony.PatchAll(Assembly.GetExecutingAssembly());

            TkUtils.BuildModList();
            TkLogger.Info($"Cached {TkUtils.ModListCache.Length} mods.");

            try
            {
                TkUtils.ShopExpansion = ShopExpansionHelper.LoadData<XmlShop>(ShopExpansionHelper.ExpansionFile);

                TkLogger.Info($"{TkUtils.ShopExpansion.Traits.Count} traits loaded into the shop.");
                TkLogger.Info($"{TkUtils.ShopExpansion.Races.Count} races loaded into the shop.");
            }
            catch (Exception)
            {
                if (File.Exists(ShopExpansionHelper.ExpansionFile))
                {
                    TkLogger.Warn("Could not load shop extension database! Attempting to salvage it...");
                    ShopExpansionHelper.TrySalvageData();
                }
            }

            TkUtils.ShopExpansion ??= new XmlShop();
            TkUtils.ShopExpansion.Traits ??= new List<XmlTrait>();
            TkUtils.ShopExpansion.Races ??= new List<XmlRace>();

            ShopExpansionHelper.TryMigrateData();
            ShopExpansionHelper.ValidateExpansionData();

            ShopExpansionHelper.DumpCommands();
            ShopExpansionHelper.DumpModList();
        }
    }

    public class TkUtils : Mod
    {
        public const string Id = "ToolkitUtils";
        internal static HarmonyLib.Harmony Harmony;
        internal static ModDump[] ModListCache;
        internal static XmlShop ShopExpansion;

        public TkUtils(ModContentPack content) : base(content)
        {
            GetSettings<TkSettings>();
            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(TkUtilsWindow)));
        }

        public static void BuildModList()
        {
            var jsonMods = new List<ModDump>();
            var running = ModsConfig.ActiveModsInLoadOrder.ToList();

            foreach (var mod in running.Where(m => m.Active))
            {
                if (mod.Official)
                {
                    continue;
                }

                string version = null;
                string steamId = null;

                var handle = LoadedModManager.ModHandles.FirstOrDefault(h => h.Content.PackageId.Equals(mod.PackageId));

                if (handle != null)
                {
                    version = handle.GetType().Module.Assembly.GetName().Version.ToString();
                }

                if (version == null)
                {
                    var manifestFile = Path.Combine(mod.RootDir.ToString(), "About/Manifest.xml");

                    if (File.Exists(manifestFile))
                    {
                        using var reader = new XmlTextReader(manifestFile);
                        reader.ReadToFollowing("version");

                        if (reader.Name.Equals("version"))
                        {
                            version = reader.ReadElementContentAsString();
                            reader.Close();
                        }
                    }
                }

                if (mod.SteamAppId > 0)
                {
                    steamId = mod.SteamAppId.ToString();
                }

                if (steamId.NullOrEmpty())
                {
                    var hook = mod.GetWorkshopItemHook();
                    var property = hook.GetType().GetProperty("PublishedFileId");
                    steamId = property?.GetValue(hook)?.ToString();
                }

                if (steamId.NullOrEmpty())
                {
                    var publishedFile = Path.Combine(mod.RootDir.ToString(), "About/PublishedFileId.txt");

                    if (File.Exists(publishedFile))
                    {
                        steamId = File.ReadAllText(publishedFile);
                    }
                }

                jsonMods.Add(
                    new ModDump {author = mod.Author, name = mod.Name, version = version, steamId = steamId}
                );
            }

            ModListCache = jsonMods.ToArray();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            TkSettings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return Id;
        }
    }
}
