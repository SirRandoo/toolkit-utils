using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

            TkUtils.ModListCache = TkUtils.GetModListVersioned();
            Logger.Info($"Cached {TkUtils.ModListCache.Length} mods.");

            try
            {
                TkUtils.ShopExpansion = ShopExpansionHelper.LoadData<XmlShop>(ShopExpansionHelper.ExpansionFile);

                Logger.Info($"{TkUtils.ShopExpansion.Traits.Count} traits loaded into the shop.");
                Logger.Info($"{TkUtils.ShopExpansion.Races.Count} races loaded into the shop.");
            }
            catch (Exception)
            {
                if (!File.Exists(ShopExpansionHelper.ExpansionFile))
                {
                    Logger.Warn("If this is your first run with this version of Utils, you can safely ignore this.");
                }
                else
                {
                    Logger.Warn("Could not load shop extension database! Attempting to salvage it...");
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
        internal static Tuple<string, string>[] ModListCache;
        internal static XmlShop ShopExpansion;

        public TkUtils(ModContentPack content) : base(content)
        {
            GetSettings<TkSettings>();
            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(TkUtilsWindow)));
        }

        public static IEnumerable<string> GetModListUnversioned()
        {
            return GetModListVersioned().Select(m => m.Item1);
        }

        public static Tuple<string, string>[] GetModListVersioned()
        {
            if (ModListCache?.Any() ?? false)
            {
                Logger.Debug("Something requested the mod list; we'll use the cached list.");
                return ModListCache;
            }

            var container = new List<Tuple<string, string>>();

            foreach (var content in LoadedModManager.RunningMods)
            {
                if (container.Any(i => i.Item1.Equals(content.Name)))
                {
                    continue;
                }

                var version = "0.0.0.0";
                if (content.assemblies?.loadedAssemblies.Count > 0)
                {
                    var handle = LoadedModManager.ModHandles.FirstOrDefault(h => h.Content == content);

                    version = handle?.GetType().Module.Assembly.GetName().Version.ToString() ?? "0.0.0.0";
                }

                container.Add(
                    new Tuple<string, string>(content.Name, version)
                );
            }

            ModListCache = container.ToArray();

            return ModListCache;
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
