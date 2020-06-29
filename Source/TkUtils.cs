using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Incidents;
using TwitchToolkit.Settings;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;
using Verse.Steam;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedType.Global

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    public static class TkUtilsStatic
    {
        static TkUtilsStatic()
        {
            TkSettings.ValidateDynamicSettings();

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
            ShopExpansionHelper.DumpShopExpansion();

            List<StoreIncident> incidents = DefDatabase<StoreIncident>.AllDefsListForReading;
            var wereChanges = false;

            foreach (StoreIncident incident in incidents.Where(
                i => i.defName == "BuyPawn"
                     || i.defName == "AddTrait"
                     || i.defName == "RemoveTrait"
            ))
            {
                if (incident.cost <= 1)
                {
                    continue;
                }

                incident.cost = 1;
                wereChanges = true;
            }

            if (wereChanges)
            {
                TkLogger.Info("Changing prices for overwritten events...");
                Store_IncidentEditor.UpdatePriceSheet();
            }


            try
            {
                // ReSharper disable once StringLiteralTypo
                if (ModLister.GetActiveModWithIdentifier("sickboywi.medieval.vanilla") != null)
                {
                    TkLogger.Warn("Medieval - Vanilla detected!");
                }
            }
            catch (Exception e)
            {
                TkLogger.Error("Could not sanitize Toolkit's store inventory!", e);
            }
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
            List<ModMetaData> running = ModsConfig.ActiveModsInLoadOrder.ToList();

            foreach (ModMetaData mod in running.Where(m => m.Active))
            {
                if (mod.Official)
                {
                    continue;
                }

                if (File.Exists(Path.Combine(mod.RootDir.ToString(), "About/IgnoreMe.txt")))
                {
                    continue;
                }

                Mod handle = LoadedModManager.ModHandles.FirstOrDefault(h => h.Content.PackageId.Equals(mod.PackageId));
                Assembly assembly = null;
                string version = null;
                string steamId = null;
                string manifestFile = Path.Combine(mod.RootDir.ToString(), "About/Manifest.xml");

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

                if (version == null && handle != null)
                {
                    assembly = Assembly.GetAssembly(handle.GetType());

                    var attribute = (AssemblyInformationalVersionAttribute) Attribute.GetCustomAttribute(
                        assembly,
                        typeof(AssemblyInformationalVersionAttribute),
                        false
                    );

                    version = attribute?.InformationalVersion;
                }

                if (version == null && handle != null)
                {
                    var attribute = (AssemblyFileVersionAttribute) Attribute.GetCustomAttribute(
                        assembly,
                        typeof(AssemblyFileVersionAttribute),
                        false
                    );

                    version = attribute?.Version;
                }

                if (version == null && handle != null)
                {
                    var attribute = (AssemblyVersionAttribute) Attribute.GetCustomAttribute(
                        assembly,
                        typeof(AssemblyVersionAttribute),
                        false
                    );

                    version = attribute?.Version ?? handle.GetType().Module.Assembly.GetName().Version.ToString();
                }

                if (mod.SteamAppId > 0)
                {
                    steamId = mod.SteamAppId.ToString();
                }

                if (steamId.NullOrEmpty())
                {
                    WorkshopItemHook hook = mod.GetWorkshopItemHook();
                    PropertyInfo property = hook.GetType().GetProperty("PublishedFileId");
                    steamId = property?.GetValue(hook)?.ToString();
                }

                if (steamId.NullOrEmpty())
                {
                    string publishedFile = Path.Combine(mod.RootDir.ToString(), "About/PublishedFileId.txt");

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
