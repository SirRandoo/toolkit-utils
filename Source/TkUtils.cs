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

            Logger.Info("Building mod list cache...");
            TkUtils.ModListCache = TkUtils.GetModListVersioned();
            Logger.Info($"Built mod list cache ({TkUtils.ModListCache.Length.ToString()} mods loaded)");

            try
            {
                TkUtils.ShopExpansion = ShopExpansionHelper.LoadData<XmlShop>(ShopExpansionHelper.ExpansionFile);
                Logger.Info(
                    $"Loaded {TkUtils.ShopExpansion.Traits.Count} traits and {TkUtils.ShopExpansion.Races.Count} races."
                );
            }
            catch (Exception)
            {
                Logger.Warn("Could not load shop extension database!");

                if (!File.Exists(ShopExpansionHelper.ExpansionFile))
                {
                    Logger.Warn("If this is your first run with this version of Utils, you can safely ignore this.");
                }
                else
                {
                    ShopExpansionHelper.TrySalvageData();
                }
            }

            if (TkUtils.ShopExpansion == null)
            {
                TkUtils.ShopExpansion = new XmlShop();
            }

            if (TkUtils.ShopExpansion.Traits == null)
            {
                TkUtils.ShopExpansion.Traits = new List<XmlTrait>();
            }

            if (TkUtils.ShopExpansion.Races == null)
            {
                TkUtils.ShopExpansion.Races = new List<XmlRace>();
            }

            ShopExpansionHelper.TryMigrateData();
            ShopExpansionHelper.ValidateExpansionData();
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

            foreach (var handle in LoadedModManager.ModHandles)
            {
                if (container.Any(i => i.Item1.Equals(handle.Content.Name)))
                {
                    continue;
                }

                var version = handle.GetType().Module.Assembly.GetName().Version.ToString();

                container.Add(
                    new Tuple<string, string>(handle.Content.Name, version)
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
