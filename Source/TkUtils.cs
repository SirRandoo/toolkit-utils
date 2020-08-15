using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Settings;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    [UsedImplicitly]
    public static class TkUtilsStatic
    {
        static TkUtilsStatic()
        {
            TkSettings.ValidateDynamicSettings();

            TkUtils.Harmony = new HarmonyLib.Harmony("com.sirrandoo.tkutils");
            TkUtils.Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [UsedImplicitly]
    public class TkUtils : Mod
    {
        public const string Id = "ToolkitUtils";
        internal static HarmonyLib.Harmony Harmony;

        public TkUtils(ModContentPack content) : base(content)
        {
            GetSettings<TkSettings>();
            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(TkUtilsWindow)));
        }

        public static void BuildModList()
        {
            List<ModMetaData> running = ModsConfig.ActiveModsInLoadOrder.ToList();

            Data.Mods = running.Where(m => m.Active)
               .Where(mod => !mod.Official)
               .Where(mod => !File.Exists(Path.Combine(mod.RootDir.ToString(), "About/IgnoreMe.txt")))
               .Select(ModItem.FromMetadata)
               .ToArray();
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
